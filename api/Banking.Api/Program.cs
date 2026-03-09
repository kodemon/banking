using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Api.Identity;
using Banking.Atomic;
using Banking.Principals;
using Banking.Transactions;
using Banking.Users;
using Cerbos.Sdk.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

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
 | Resolves an IAuth session per-request by:
 |   1. Extracting the Zitadel JWT "sub" claim and issuer from the authenticated
 |      HttpContext user.
 |   2. Looking up the matching Principal via IPrincipalRepository using the
 |      issuer as the provider key and "sub" as the external identity ID.
 |   3. Wrapping the result in an Auth instance and registering it as scoped IAuth.
 |
 | If the JWT has no "sub" claim, or no matching Principal exists in the database,
 | a 401 UnauthorizedException is thrown, which AppExceptionHandler maps to a
 | RFC 9457 problem details response.
 |
 */

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(_ =>
    CerbosClientBuilder.ForTarget(cerbosTarget).WithPlaintext().Build()
);
builder.Services.AddScoped<IAuthResolver, ZitadelAuthResolver>();
builder.Services.AddScoped(sp =>
{
    var httpContext =
        sp.GetRequiredService<IHttpContextAccessor>().HttpContext
        ?? throw new InvalidOperationException("No HttpContext available");

    return AuthMiddleware.GetAuth(httpContext);
});

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

app.UseAuthentication();
app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
