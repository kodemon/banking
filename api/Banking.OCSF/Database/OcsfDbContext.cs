using Banking.OCSF.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Banking.OCSF.Database;

internal class OcsfDbContext(DbContextOptions<OcsfDbContext> options) : DbContext(options)
{
    internal DbSet<AuditLogEntry> AuditLog => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ocsf");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OcsfDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}