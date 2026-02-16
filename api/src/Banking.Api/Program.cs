using Banking.Application.Interfaces;
using Banking.Application.Services;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Repositories;
using Banking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database

builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services

builder.Services.AddScoped<PrincipalAttributesBuilder>();
builder.Services.AddScoped<UserService>();

// Controllers

builder.Services.AddControllers();

// OpenAPI

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
