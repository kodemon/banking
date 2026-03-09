using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Principals.Database;

internal class PrincipalDbContextFactory : IDesignTimeDbContextFactory<PrincipalDbContext>
{
    public PrincipalDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PrincipalDbContext>()
            .UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=banking")
            .Options;

        return new PrincipalDbContext(options);
    }
}
