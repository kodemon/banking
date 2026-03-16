using System.Reflection;
using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Api.Identity;
using Banking.Atomic;
using Banking.OCSF;
using Banking.Principals;
using Banking.Transactions;
using Banking.Users;
using Cerbos.Sdk.Builder;
using Fido2NetLib;
using Microsoft.AspNetCore.Http.Features;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

/*
 |--------------------------------------------------------------------------------
 | Configuration
 |--------------------------------------------------------------------------------
 */

var connString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrWhiteSpace(connString))
{
    throw new Exception("No valid postgres connection configuration found");
}

var cerbosTarget =
    builder.Configuration["Cerbos:Target"] ?? throw new Exception("Cerbos target not configured");
var rabbitHost =
    builder.Configuration["RabbitMq:Host"] ?? throw new Exception("RabbitMq:Host not configured");
var rabbitUser =
    builder.Configuration["RabbitMq:User"] ?? throw new Exception("RabbitMq:User not configured");
var rabbitPassword =
    builder.Configuration["RabbitMq:Password"]
    ?? throw new Exception("RabbitMq:Password not configured");

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

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IFido2>(_ => new Fido2(
    new Fido2Configuration
    {
        ServerDomain = builder.Configuration["Fido2:ServerDomain"] ?? "localhost",
        ServerName = builder.Configuration["Fido2:ServerName"] ?? "Banking",
        Origins = (builder.Configuration["Fido2:Origins"] ?? "https://localhost;http://localhost")
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Trim())
            .ToHashSet(),
    }
));

builder.Services.AddAtomicService(connString);
builder.Services.AddOcsfModule(connString, rabbitHost, rabbitUser, rabbitPassword);
builder.Services.AddAccountsModule(connString);
builder.Services.AddPrincipalsModule(connString);
builder.Services.AddTransactionsModule(connString);
builder.Services.AddUsersModule(connString);

/*
 |--------------------------------------------------------------------------------
 | Identity
 |--------------------------------------------------------------------------------
 |
 | AuthMiddleware runs on every request.  It reads the "zitadel_session" cookie,
 | calls Zitadel's Session v2 API to validate it, then resolves the application
 | Principal via MediatR and stores IAuth in context.Items.
 |
 | Retrieve downstream with: AuthMiddleware.GetAuth(httpContext)
 |
 */

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(_ =>
    CerbosClientBuilder.ForTarget(cerbosTarget).WithPlaintext().Build()
);
builder.Services.AddScoped(sp =>
    AuthContext.GetAuth(
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
    app.MapOpenApi("/api/openapi/{documentName}.json");
    app.MapScalarApiReference(
        "/api/scalar",
        options =>
        {
            options.Title = "Banking";
            options.Theme = ScalarTheme.Saturn;
            options.OpenApiRoutePattern = "/api/openapi/{documentName}.json";
        }
    );
}
else
{
    app.UseHttpsRedirection();
}

app.UseRollbackRegistrations();
app.UseExceptionHandler();

app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.Run();
