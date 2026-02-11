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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new AccountHolderConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}