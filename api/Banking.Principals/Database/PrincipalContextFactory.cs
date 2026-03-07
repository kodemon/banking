using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Principals.Database;

public class PrincipalContextFactory : IDesignTimeDbContextFactory<PrincipalContext>
{
    public PrincipalContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PrincipalContext>()
            .UseNpgsql("connection_string")
            .Options;

        return new PrincipalContext(options);
    }
}
