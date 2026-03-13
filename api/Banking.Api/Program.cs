using System.Reflection;
using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Api.Identity;
using Banking.Api.Identity.Bff;
using Banking.Atomic;
using Banking.Principals;
using Banking.Transactions;
using Banking.Users;
using Cerbos.Sdk.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrWhiteSpace(connString))
{
    throw new Exception("No valid postgres connection configuration found");
}

var cerbosTarget =
    builder.Configuration["Cerbos:Target"] ?? throw new Exception("Cerbos target not configured");

/*
 |--------------------------------------------------------------------------------
 | MediatR License
 |--------------------------------------------------------------------------------
 */

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.LicenseKey = builder.Configuration["MediatR:LicenseKey"];
});

/*
 |--------------------------------------------------------------------------------
 | Valkey (session store)
 |--------------------------------------------------------------------------------
 |
 | StackExchange.Redis is a drop-in client for Valkey.
 | Connection string format: "localhost:6379" or "valkey:6379,password=secret"
 |
 */

var valkeyConnString =
    builder.Configuration.GetConnectionString("Valkey")
    ?? throw new Exception("No valid Valkey connection configuration found");

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(valkeyConnString)
);

builder.Services.AddScoped<IBffSessionStore, ValkeySessionStore>();
builder.Services.AddScoped<IPkceStore, ValkeyPkceStore>();

/*
 |--------------------------------------------------------------------------------
 | Zitadel token client (BFF code exchange + refresh)
 |--------------------------------------------------------------------------------
 */

builder.Services.AddHttpClient<IZitadelTokenClient, ZitadelTokenClient>();

/*
 |--------------------------------------------------------------------------------
 | Authentication
 |--------------------------------------------------------------------------------
 */

Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority =
            builder.Configuration["Zitadel:Authority"]
            ?? throw new Exception("Zitadel authority missing");

        var audience =
            builder.Configuration["Zitadel:Audience"]
            ?? throw new Exception("Zitadel audience missing");

        options.Authority = authority;
        options.Audience = audience;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authority,
            ValidAudience = audience,
            NameClaimType = "preferred_username",
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        };
    });

builder.Services.AddAuthorization();

/*
 |--------------------------------------------------------------------------------
 | Modules
 |--------------------------------------------------------------------------------
 */

builder
    .Services.AddControllers()
    .AddApplicationPart(typeof(UsersModule).Assembly)
    .AddApplicationPart(typeof(AccountsModule).Assembly)
    .AddApplicationPart(typeof(PrincipalsModule).Assembly)
    .AddApplicationPart(typeof(TransactionsModule).Assembly)
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
    });

builder.Services.AddAtomicService(connString);
builder.Services.AddAccountsModule(connString);
builder.Services.AddPrincipalsModule(connString);
builder.Services.AddTransactionsModule(connString);
builder.Services.AddUsersModule(connString);

/*
 |--------------------------------------------------------------------------------
 | Identity
 |--------------------------------------------------------------------------------
 |
 | BffMiddleware runs first: reads the session cookie, fetches tokens from Valkey,
 | refreshes if expiring, and injects "Authorization: Bearer {access_token}".
 |
 | UseAuthentication() then validates the injected JWT normally.
 |
 | AuthMiddleware resolves the Principal from the validated JWT claims, creating
 | one via MediatR if it does not yet exist, then stores IAuth in context.Items.
 | Retrieve it downstream via AuthMiddleware.GetAuth(httpContext).
 |
 */

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(_ =>
    CerbosClientBuilder.ForTarget(cerbosTarget).WithPlaintext().Build()
);
builder.Services.AddScoped<IAuth>(sp =>
    AuthMiddleware.GetAuth(
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext
            ?? throw new InvalidOperationException("No HttpContext available")
    )
);

/*
 |--------------------------------------------------------------------------------
 | Exception Handling
 |--------------------------------------------------------------------------------
 */

builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

/*
 |--------------------------------------------------------------------------------
 | OpenAPI
 |--------------------------------------------------------------------------------
 */

builder.Services.AddOpenApi("v1");

/*
 |--------------------------------------------------------------------------------
 | Application
 |--------------------------------------------------------------------------------
 */

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRollbackRegistrations();
app.UseExceptionHandler();

app.UseMiddleware<AuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
