using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Banking.Infrastructure.Persistence.Configurations;

namespace Banking.Infrastructure.Persistence;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountHolder> Identities => Set<AccountHolder>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // ### Core Entities

        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHolderConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new JournalEntryConfiguration());

        // ### AccountHolder Metadata Entities

        modelBuilder.ApplyConfiguration(new AccountHolderIndividualIdentityConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHolderBusinessIdentityConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHolderAddressConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHolderEmailConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHolderPhoneConfiguration());

        base.OnModelCreating(modelBuilder);

    }
}