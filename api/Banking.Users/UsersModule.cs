using Banking.Shared.Database;
using Banking.Users.Database;
using Banking.Users.Interfaces;
using Banking.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Users;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services)
    {
        SetupDatabase(services);
        SetupCQRS(services);

        return services;
    }

    public static void SetupDatabase(IServiceCollection services)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("users"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void SetupCQRS(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UsersModule).Assembly));
    }
}
