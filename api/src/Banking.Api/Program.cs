using Banking.Api.Exceptions;
using Banking.Application.Interfaces;
using Banking.Application.Services;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Repositories;
using Banking.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

/*
 |--------------------------------------------------------------------------------
 | Databases
 |--------------------------------------------------------------------------------
 */

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

/*
 |--------------------------------------------------------------------------------
 | Repositories
 |--------------------------------------------------------------------------------
 */

builder.Services.AddScoped<IUserRepository, UserRepository>();

/*
 |--------------------------------------------------------------------------------
 | Services
 |--------------------------------------------------------------------------------
 */

builder.Services.AddScoped<PrincipalAttributesBuilder>();
builder.Services.AddScoped<UserService>();

/*
 |--------------------------------------------------------------------------------
 | Controllers
 |--------------------------------------------------------------------------------
 */

builder.Services.AddControllers();

/*
 |--------------------------------------------------------------------------------
 | Exception Handling
 |--------------------------------------------------------------------------------
 |
 | Adds the Banking.Application exception handler for exceptions thrown or
 | returned in the Banking.Application layer.
 |
 | We also extend the result with Instance, requestId and traceId details to
 | present as much debugging details as possible without exposing system trace
 | logs.
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

// ### Development

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// ### Middleware

app.UseExceptionHandler();
app.UseHttpsRedirection();

// ### Controllers

app.MapControllers();

// ### Start

app.Run();
