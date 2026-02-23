using Microsoft.EntityFrameworkCore;
using Banking.Infrastructure.Persistence.Configurations;
using Banking.Domain.Accounts;
using Banking.Domain.Access;
using Banking.Domain.Identity;
using Banking.Domain.Transactions;

namespace Banking.Infrastructure.Persistence;

public class BankingDbContext : DbContext
{
    public BankingDbContext(DbContextOptions<BankingDbContext> options) : base(options) { }

    public DbSet<PrincipalAttribute> PrincipalAttributes => Set<PrincipalAttribute>();

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<BusinessAccountHolder> BusinessAccountHolders => Set<BusinessAccountHolder>();
    public DbSet<PersonalAccountHolder> PersonalAccountHolders => Set<PersonalAccountHolder>();

    public DbSet<User> Users => Set<User>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<UserEmail> UserEmails => Set<UserEmail>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessAddress> BusinessAddresses => Set<BusinessAddress>();
    public DbSet<BusinessEmail> BusinessEmails => Set<BusinessEmail>();
    public DbSet<BusinessPhone> BusinessPhones => Set<BusinessPhone>();

    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PrincipalAttributeConfiguration());

        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessAccountHolderConfiguration());
        modelBuilder.ApplyConfiguration(new PersonalAccountHolderConfiguration());

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserAddressConfiguration());
        modelBuilder.ApplyConfiguration(new UserEmailConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessAddressConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessEmailConfiguration());
        modelBuilder.ApplyConfiguration(new BusinessPhoneConfiguration());

        modelBuilder.ApplyConfiguration(new JournalEntryConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}