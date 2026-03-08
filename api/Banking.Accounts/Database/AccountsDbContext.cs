using Banking.Accounts.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Accounts.Database;

internal class AccountsDbContext : DbContext
{
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options)
        : base(options) { }

    internal DbSet<Account> Accounts => Set<Account>();
    internal DbSet<AccountHolder> AccountHolders => Set<AccountHolder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
