using Banking.Users.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Users;

/*
 |--------------------------------------------------------------------------------
 | Users Module
 |--------------------------------------------------------------------------------
 |
 | Single public entry point for the Banking.Users module. The host calls
 | AddUsersModule() with a connection string and knows nothing about what's
 | inside.
 |
 | UsersDbContext is scoped to this module — no other module or the host can
 | access it. Migrations are managed from within this project.
 |
 */

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<UserService>();

        return services;
    }
}