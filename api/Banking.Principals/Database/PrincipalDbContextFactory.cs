using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Principals.Database;

internal class PrincipalDbContextFactory : IDesignTimeDbContextFactory<PrincipalDbContext>
{
    public PrincipalDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PrincipalDbContext>()
            .UseSqlite(SQLiteConnection.Load("principals"))
            .Options;

        return new PrincipalDbContext(options);
    }
}
