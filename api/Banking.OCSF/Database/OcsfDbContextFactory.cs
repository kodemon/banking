using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.OCSF.Database;

internal class OcsfDbContextFactory : IDesignTimeDbContextFactory<OcsfDbContext>
{
    public OcsfDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OcsfDbContext>()
            .UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=banking")
            .Options;

        return new OcsfDbContext(options);
    }
}
