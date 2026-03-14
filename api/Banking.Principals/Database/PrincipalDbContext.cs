using Banking.Principals.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Database;

internal class PrincipalDbContext(DbContextOptions<PrincipalDbContext> options) : DbContext(options)
{
    internal DbSet<Principal> Principals => Set<Principal>();
    internal DbSet<PrincipalIdentity> PrincipalIdentities => Set<PrincipalIdentity>();
    internal DbSet<PrincipalRole> PrincipalRoles => Set<PrincipalRole>();
    internal DbSet<PrincipalSession> PrincipalSessions => Set<PrincipalSession>();
    internal DbSet<PrincipalPasskeyCredential> PrincipalPasskeyCredentials =>
        Set<PrincipalPasskeyCredential>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("principals");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrincipalDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
