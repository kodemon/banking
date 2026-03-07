using Banking.Shared.AccessControl;
using Banking.Shared.Database;
using Banking.Users.AccessControl;
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
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("users"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<UserService>();

        // ### Access Control

        services.AddSingleton<IAccessAttributeResolver, UserAccessAttributeResolver>();

        return services;
    }
}
