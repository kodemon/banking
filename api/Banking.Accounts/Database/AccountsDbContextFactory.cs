using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Accounts.Database;

internal class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
{
    public AccountsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AccountsDbContext>()
            .UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=banking")
            .Options;

        return new AccountsDbContext(options);
    }
}
