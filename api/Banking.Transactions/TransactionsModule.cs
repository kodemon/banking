using Banking.Transactions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Transactions;

public static class TransactionsModule
{
    public static IServiceCollection AddTransactionsModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<TransactionsDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<TransactionService>();
        services.AddScoped<IBalanceService, BalanceService>();

        return services;
    }
}