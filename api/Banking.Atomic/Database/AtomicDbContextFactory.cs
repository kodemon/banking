using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Atomic.Persistence;

internal class AtomicDbContextFactory : IDesignTimeDbContextFactory<AtomicDbContext>
{
    public AtomicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AtomicDbContext>()
            .UseSqlite(SQLiteConnection.Load("atomic"))
            .Options;

        return new AtomicDbContext(options);
    }
}
