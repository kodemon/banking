using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Events;
using Banking.Principal;
using Banking.Shared.Database;
using Banking.Transactions;
using Banking.Users;
using JasperFx;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Sqlite;

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
 |
 | AddEventsModule() must come before UseWolverine() so that EventsDbContext
 | is registered in DI when Wolverine wires its EF Core integration.
 |
 | Domain modules use plain AddDbContext — their DbContexts are read-only
 | from the perspective of Wolverine (handlers write to them directly via
 | repository/DbContext, not through Wolverine's outbox).
 |
 */

builder.Services.AddEventsModule();
builder.Services.AddAccountsModule();
builder.Services.AddPrincipalsModule(builder.Configuration);
builder.Services.AddTransactionsModule();
builder.Services.AddUsersModule();

// IPrincipalProvisioner is defined in Banking.Principals (no circular dep)
// but implemented here where IMessageBus and ZitadelMetadataService are available.
// Must be registered after AddPrincipalsModule() which registers the interface.
builder.Services.AddScoped<
    Banking.Principal.AccessControl.IPrincipalProvisioner,
    Banking.Api.PrincipalProvisioner
>();

/*
 |--------------------------------------------------------------------------------
 | Wolverine
 |--------------------------------------------------------------------------------
 |
 | UseWolverine() scans all assemblies in the solution for handler classes.
 | It finds:
 |   - Banking.Api/Handlers/*CommandHandler  — command → event → publish
 |   - Banking.Users/Handlers/*Handler       — event → write read model
 |   - Banking.Principals/Handlers/*Handler
 |   - Banking.Accounts/Handlers/*Handler
 |   - Banking.Transactions/Handlers/*Handler
 |
 | PersistMessagesWithSqlite() creates Wolverine's outbox/inbox/dead-letter
 | tables in events.db (same file as EventsDbContext, different tables).
 |
 | UseEntityFrameworkCoreTransactions() wraps each handler invocation in an
 | EF Core transaction using the DbContext that is registered for the handler's
 | assembly. For Banking.Api command handlers this is EventsDbContext. For
 | domain handlers this is each domain's own DbContext.
 |
 | AutoApplyTransactions() removes the need to call SaveChangesAsync() manually
 | in handlers — Wolverine commits the transaction after the handler returns.
 |
 */

builder.Host.UseWolverine(opts =>
{
    // Durable outbox + saga persistence in events.db
    opts.PersistMessagesWithSqlite(SQLiteConnection.Load("events"));

    // Wrap every handler in an EF Core transaction scoped to the right DbContext
    opts.UseEntityFrameworkCoreTransactions();

    // Auto-commit EF Core changes after each handler — no SaveChangesAsync needed
    opts.Policies.AutoApplyTransactions();

    // Discover handlers in all referenced assemblies (Banking.Users, Banking.Accounts, etc.)
    opts.Discovery.IncludeAssembly(typeof(Program).Assembly); // Banking.Api — command handlers + sagas
    opts.Discovery.IncludeAssembly(typeof(UsersModule).Assembly);
    opts.Discovery.IncludeAssembly(typeof(AccountsModule).Assembly);
    opts.Discovery.IncludeAssembly(typeof(PrincipalsModule).Assembly);
    opts.Discovery.IncludeAssembly(typeof(TransactionsModule).Assembly);
});

/*
 |--------------------------------------------------------------------------------
 | Controllers
 |--------------------------------------------------------------------------------
 |
 | All controllers live in Banking.Api. InternalControllerFeatureProvider
 | is required so ASP.NET Core discovers internal controllers — keeping all
 | domain types internal prevents accidental cross-module coupling.
 |
 */

builder
    .Services.AddControllers()
    .ConfigureApplicationPartManager(m =>
        m.FeatureProviders.Add(new InternalControllerFeatureProvider())
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
        context.ProblemDetails.Extensions.TryAdd("correlationId", activity?.Id);
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

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunJasperFxCommands(args);
