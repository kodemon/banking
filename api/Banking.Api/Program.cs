using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Api.Identity;
using Banking.Atomic;
using Banking.Principals;
using Banking.Transactions;
using Banking.Users;
using Cerbos.Sdk;
using Cerbos.Sdk.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
        options.Authority = builder.Configuration["Zitadel:Authority"];
        options.Audience = builder.Configuration["Zitadel:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Zitadel:Authority"],
            ValidAudience = builder.Configuration["Zitadel:Audience"],
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

builder.Services.AddAtomicService();
builder.Services.AddAccountsModule();
builder.Services.AddPrincipalsModule();
builder.Services.AddTransactionsModule();
builder.Services.AddUsersModule();

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
    CerbosClientBuilder.ForTarget(builder.Configuration["Cerbos:Target"]!).WithPlaintext().Build()
);
builder.Services.AddScoped<IAuthResolver, ZitadelAuthResolver>();
builder.Services.AddScoped(sp =>
    AuthMiddleware.GetAuth(sp.GetRequiredService<IHttpContextAccessor>().HttpContext!)
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

app.UseAuthentication();
app.UseMiddleware<AuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
