using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Atomic.Persistence;

internal class AtomicDbContextFactory : IDesignTimeDbContextFactory<AtomicDbContext>
{
    public AtomicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AtomicDbContext>()
            .UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=banking")
            .Options;

        return new AtomicDbContext(options);
    }
}
