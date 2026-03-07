using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.AtomicFlow.Persistence;

internal class AtomicFlowDbContextFactory : IDesignTimeDbContextFactory<AtomicFlowDbContext>
{
    public AtomicFlowDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AtomicFlowDbContext>()
            .UseSqlite(SQLiteConnection.Load("atomic-flow"))
            .Options;

        return new AtomicFlowDbContext(options);
    }
}
