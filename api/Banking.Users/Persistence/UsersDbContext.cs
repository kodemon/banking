using Microsoft.EntityFrameworkCore;

namespace Banking.Users.Persistence;

/*
 |--------------------------------------------------------------------------------
 | Users DbContext
 |--------------------------------------------------------------------------------
 |
 | Owns only the entities that belong to the Banking.Users module.
 | No other module's entities are visible here — cross-module data access
 | goes through each module's public API, never through a shared query.
 |
 | This context and its migrations travel with the module. If Banking.Users
 | is ever extracted to its own service, this file and Persistence/Migrations/
 | come along unchanged.
 |
 */

internal class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

    internal DbSet<User> Users => Set<User>();
    internal DbSet<UserEmail> UserEmails => Set<UserEmail>();
    internal DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}