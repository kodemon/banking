using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Accounts.Persistence;

/*
 |--------------------------------------------------------------------------------
 | Accounts DbContext Factory
 |--------------------------------------------------------------------------------
 |
 | Used exclusively by EF Core tooling. Allows migrations to be managed
 | directly from the Banking.Accounts project without a running host:
 |
 |   dotnet ef migrations add InitialAccounts --project Banking.Accounts --output-dir Persistence/Migrations
 |   dotnet ef database update --project Banking.Accounts
 |
 | The connection string here is for tooling only. At runtime the connection
 | string comes from the host via AddAccountsModule().
 |
 */

internal class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
{
    public AccountsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AccountsDbContext>()
            .UseSqlServer("Server=localhost,1433;Database=Banking;User Id=sa;Password=!Password1;TrustServerCertificate=True;")
            .Options;

        return new AccountsDbContext(options);
    }
}