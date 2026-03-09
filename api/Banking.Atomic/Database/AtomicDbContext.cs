using Banking.Atomic.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Atomic.Persistence;

internal class AtomicDbContext(DbContextOptions<AtomicDbContext> options) : DbContext(options)
{
    internal DbSet<AtomicRecord> AtomicRecords => Set<AtomicRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("atomic");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AtomicDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
