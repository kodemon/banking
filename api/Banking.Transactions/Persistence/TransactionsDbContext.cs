using Microsoft.EntityFrameworkCore;

namespace Banking.Transactions.Persistence;

/*
 |--------------------------------------------------------------------------------
 | Transactions DbContext
 |--------------------------------------------------------------------------------
 |
 | Owns Transaction and JournalEntry. The JournalEntry table is shared with
 | Banking.Accounts which maintains its own local read model (AccountJournalEntry)
 | mapping to the same table. Transactions writes, Accounts reads.
 |
 */

internal class TransactionsDbContext : DbContext
{
    public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options) : base(options) { }

    internal DbSet<Transaction> Transactions => Set<Transaction>();
    internal DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}