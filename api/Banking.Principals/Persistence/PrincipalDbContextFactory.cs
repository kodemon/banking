using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Principal.Persistence;

/*
 |--------------------------------------------------------------------------------
 | PrincipalDbContextFactory
 |--------------------------------------------------------------------------------
 |
 | Used by EF Core tooling (dotnet ef migrations add) to instantiate the
 | context at design time without needing the full host to be running.
 |
 */

internal class PrincipalDbContextFactory : IDesignTimeDbContextFactory<PrincipalDbContext>
{
    public PrincipalDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PrincipalDbContext>()
            .UseSqlite("Data Source=banking-principals.db")
            .Options;

        return new PrincipalDbContext(options);
    }
}