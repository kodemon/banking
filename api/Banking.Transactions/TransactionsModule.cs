using Banking.Transactions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Transactions;

public static class TransactionsModule
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services)
    {
        var moduleDirectory = Path.GetDirectoryName(
            typeof(TransactionsModule).Assembly.Location)!;

        var dbPath = Path.Combine(moduleDirectory, "banking-transactions.db");

        services.AddDbContext<TransactionsDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
        );

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<TransactionService>();
        services.AddScoped<IBalanceService, BalanceService>();

        return services;
    }
}