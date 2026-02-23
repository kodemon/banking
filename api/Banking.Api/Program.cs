using Banking.Accounts;
using Banking.Api;
using Banking.Api.Exceptions;
using Banking.Transactions;
using Banking.Users;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

/*
 |--------------------------------------------------------------------------------
 | Modules
 |--------------------------------------------------------------------------------
 |
 | Each module is self-contained — it registers its own DbContext, repository,
 | and services internally. The host only needs to pass the connection string.
 |
 | Controllers are internal to each module assembly. AddApplicationPart tells
 | ASP.NET Core to discover and register them at startup.
 |
 */

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(UsersModule).Assembly)
    .AddApplicationPart(typeof(AccountsModule).Assembly)
    .AddApplicationPart(typeof(TransactionsModule).Assembly)
    .ConfigureApplicationPartManager(manager =>
    {
        manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
    });

builder.Services.AddUsersModule(connectionString);
builder.Services.AddAccountsModule(connectionString);
builder.Services.AddTransactionsModule(connectionString);

/*
 |--------------------------------------------------------------------------------
 | Exception Handling
 |--------------------------------------------------------------------------------
 |
 | AppExceptionHandler maps domain and application exceptions from any module
 | to RFC 9457 compliant problem details responses.
 |
 | ProblemDetails is extended with Instance, requestId, and traceId to provide
 | as much debugging context as possible without exposing internal stack traces.
 |
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
 | OpenAPI / Swagger
 |--------------------------------------------------------------------------------
 */

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Banking API",
        Version = "v1"
    });
});

/*
 |--------------------------------------------------------------------------------
 | Application
 |--------------------------------------------------------------------------------
 */

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();