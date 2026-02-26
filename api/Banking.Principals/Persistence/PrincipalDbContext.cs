using Microsoft.EntityFrameworkCore;

namespace Banking.Principal.Persistence;

/*
 |--------------------------------------------------------------------------------
 | Principal DbContext
 |--------------------------------------------------------------------------------
 |
 | Owns only the entities that belong to the Banking.Principal module.
 | No other module's entities are visible here — cross-module data access
 | goes through each module's public API, never through a shared query.
 |
 */

internal class PrincipalDbContext : DbContext
{
    public PrincipalDbContext(DbContextOptions<PrincipalDbContext> options) : base(options) { }

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