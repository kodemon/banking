using Banking.Events.Persistence;
using Banking.Events.Repositories.Resources;
using Banking.Principals.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Database;

public class PrincipalContext(DbContextOptions<PrincipalContext> options) : DbContext(options)
{
    public DbSet<Principal> Principals => Set<Principal>();
    public DbSet<PrincipalIdentity> PrincipalIdentities => Set<PrincipalIdentity>();
    public DbSet<PrincipalRole> PrincipalRoles => Set<PrincipalRole>();
    public DbSet<PrincipalAttribute> PrincipalAttributes => Set<PrincipalAttribute>();

    public DbSet<StoredEvent> Events => Set<StoredEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrincipalContext).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoredEventConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
