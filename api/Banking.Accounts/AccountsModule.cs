using Banking.Accounts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Accounts;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services)
    {
        var moduleDirectory = Path.GetDirectoryName(
            typeof(AccountsModule).Assembly.Location)!;

        var dbPath = Path.Combine(moduleDirectory, "banking-accounts.db");

        services.AddDbContext<AccountsDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
        );

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<AccountService>();

        return services;
    }
}