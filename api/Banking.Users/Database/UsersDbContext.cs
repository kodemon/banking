using Banking.Users.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Users.Database;

internal class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options) { }

    internal DbSet<User> Users => Set<User>();
    internal DbSet<UserEmail> UserEmails => Set<UserEmail>();
    internal DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("users");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
