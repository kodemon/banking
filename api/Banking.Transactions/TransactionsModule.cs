using Banking.Transactions.Database;
using Banking.Transactions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Transactions;

public static class TransactionsModule
{
    public static IServiceCollection AddTransactionsModule(
        this IServiceCollection services,
        string pgConnection
    )
    {
        SetupDatabase(services, pgConnection);

        return services;
    }

    private static void SetupDatabase(IServiceCollection services, string pgConnection)
    {
        services.AddDbContext<TransactionsDbContext>(options =>
            options.UseNpgsql(
                pgConnection,
                pgOptions => pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
        services.AddScoped<ITransactionRepository, TransactionRepository>();
    }
}
