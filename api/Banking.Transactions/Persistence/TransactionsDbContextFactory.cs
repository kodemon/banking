using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Transactions.Persistence;

/*
 |--------------------------------------------------------------------------------
 | Transactions DbContext Factory
 |--------------------------------------------------------------------------------
 |
 | Used exclusively by EF Core tooling. Allows migrations to be managed
 | directly from the Banking.Transactions project without a running host:
 |
 |   dotnet ef migrations add InitialTransactions --project Banking.Transactions --output-dir Persistence/Migrations
 |   dotnet ef database update --project Banking.Transactions
 |
 | The connection string here is for tooling only. At runtime the connection
 | string comes from the host via AddTransactionsModule().
 |
 */

internal class TransactionsDbContextFactory : IDesignTimeDbContextFactory<TransactionsDbContext>
{
    public TransactionsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TransactionsDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=Banking;User Id=sa;Password=!Password1;TrustServerCertificate=True;")
            .Options;

        return new TransactionsDbContext(options);
    }
}