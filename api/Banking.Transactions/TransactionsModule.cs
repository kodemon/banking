using Banking.Shared.Database;
using Banking.Transactions.Database;
using Banking.Transactions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Transactions;

public static class TransactionsModule
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services)
    {
        services.AddDbContext<TransactionsDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("transactions"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<TransactionService>();
        services.AddScoped<IBalanceService, BalanceService>();

        return services;
    }
}
