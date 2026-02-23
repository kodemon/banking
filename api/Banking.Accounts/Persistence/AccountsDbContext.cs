using Microsoft.EntityFrameworkCore;

namespace Banking.Accounts.Persistence;

internal class AccountsDbContext : DbContext
{
    public AccountsDbContext(DbContextOptions<AccountsDbContext> options) : base(options) { }

    internal DbSet<Account> Accounts => Set<Account>();
    internal DbSet<PersonalAccountHolder> PersonalAccountHolders => Set<PersonalAccountHolder>();
    internal DbSet<BusinessAccountHolder> BusinessAccountHolders => Set<BusinessAccountHolder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}