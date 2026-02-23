using Banking.Transactions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Transactions;

internal class TransactionRepository(TransactionsDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction)
    {
        await context.Transactions.AddAsync(transaction);
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await context.Transactions
            .Include(t => t.JournalEntries)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetAllByParticipantAsync(Guid participantId)
    {
        return await context.Transactions
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