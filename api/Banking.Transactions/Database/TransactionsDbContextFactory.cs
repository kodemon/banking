using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Transactions.Database;

internal class TransactionsDbContextFactory : IDesignTimeDbContextFactory<TransactionsDbContext>
{
    public TransactionsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=banking")
            .Options;

        return new TransactionsDbContext(options);
    }
}
