using Banking.Accounts.Database;
using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories;
using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Accounts;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services)
    {
        SetupDatabase(services);
        SetupCQRS(services);

        return services;
    }

    private static void SetupDatabase(IServiceCollection services)
    {
        services.AddDbContext<AccountsDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("accounts"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
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
