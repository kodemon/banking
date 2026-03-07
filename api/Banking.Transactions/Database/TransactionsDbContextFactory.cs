using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Transactions.Database;

internal class TransactionsDbContextFactory : IDesignTimeDbContextFactory<TransactionsDbContext>
{
    public TransactionsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseSqlite(SQLiteConnection.Load("transactions"))
            .Options;

        return new TransactionsDbContext(options);
    }
}
