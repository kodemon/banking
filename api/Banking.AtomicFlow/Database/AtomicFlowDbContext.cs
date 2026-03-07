using Banking.AtomicFlow.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.AtomicFlow.Persistence;

internal class AtomicFlowDbContext(DbContextOptions<AtomicFlowDbContext> options)
    : DbContext(options)
{
    internal DbSet<AtomicFlowRecord> AtomicFlowRecords => Set<AtomicFlowRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtomicFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
