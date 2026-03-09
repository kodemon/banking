using Banking.Accounts.Database;
using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Accounts;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(
        this IServiceCollection services,
        string pgConnection
    )
    {
        SetupDatabase(services, pgConnection);
        SetupCQRS(services);

        return services;
    }

    private static void SetupDatabase(IServiceCollection services, string pgConnection)
    {
        services.AddDbContext<AccountsDbContext>(options =>
            options.UseNpgsql(
                pgConnection,
                pgOptions => pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
        services.AddScoped<IAccountRepository, AccountRepository>();
    }

    private static void SetupCQRS(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(AccountsModule).Assembly)
        );
    }
}
