using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Principal;
using Banking.Transactions;
using Banking.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

/*
 |--------------------------------------------------------------------------------
 | Authentication
 |--------------------------------------------------------------------------------
 */

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

/*
 |--------------------------------------------------------------------------------
 | Modules
 |--------------------------------------------------------------------------------
 */

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(UsersModule).Assembly)
    .AddApplicationPart(typeof(AccountsModule).Assembly)
    .AddApplicationPart(typeof(PrincipalsModule).Assembly)
    .AddApplicationPart(typeof(TransactionsModule).Assembly)
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
    });

builder.Services.AddUsersModule();
builder.Services.AddAccountsModule();
builder.Services.AddPrincipalsModule();
builder.Services.AddTransactionsModule();

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
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
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
    app.MapOpenApi();                  // serves spec at /openapi/v1.json
    app.MapScalarApiReference();       // UI at /scalar/v1
}
else
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();

app.UseAuthentication();    // ← validate Zitadel JWT + trigger claims transformation
app.UseAuthorization();     // ← enforce [Authorize] attributes

app.MapControllers();

app.Run();