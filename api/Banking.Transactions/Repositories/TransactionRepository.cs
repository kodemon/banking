using Banking.Transactions.Database;
using Banking.Transactions.Interfaces;
using Banking.Transactions.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Transactions;

internal class TransactionRepository(TransactionsDbContext context) : ITransactionRepository
{
    public DbSet<Transaction> transactions = context.Transactions;

    public async Task AddAsync(Transaction transaction)
    {
        await transactions.AddAsync(transaction);
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await transactions
            .Include(t => t.JournalEntries)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetAllByParticipantAsync(Guid participantId)
    {
        return await transactions
            .Include(t => t.JournalEntries)
            .Where(t => t.JournalEntries.Any(je => je.ParticipantId == participantId))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
