using Banking.Users.Database;
using Banking.Users.Interfaces;
using Banking.Users.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Users;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        string pgConnection
    )
    {
        SetupDatabase(services, pgConnection);
        SetupCQRS(services);

        return services;
    }

    public static void SetupDatabase(IServiceCollection services, string pgConnection)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(
                pgConnection,
                pgOptions => pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void SetupCQRS(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UsersModule).Assembly));
    }
}
