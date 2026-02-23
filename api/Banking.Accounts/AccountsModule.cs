using Banking.Accounts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Accounts;

public static class AccountsModule
{
    public static IServiceCollection AddAccountsModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AccountsDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<AccountService>();

        return services;
    }
}