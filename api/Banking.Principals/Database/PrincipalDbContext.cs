using Banking.Principals.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Database;

internal class PrincipalDbContext(DbContextOptions<PrincipalDbContext> options) : DbContext(options)
{
    internal DbSet<Principal> Principals => Set<Principal>();
    internal DbSet<PrincipalIdentity> PrincipalIdentities => Set<PrincipalIdentity>();
    internal DbSet<PrincipalRole> PrincipalRoles => Set<PrincipalRole>();
    internal DbSet<PrincipalAttribute> PrincipalAttributes => Set<PrincipalAttribute>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrincipalDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
