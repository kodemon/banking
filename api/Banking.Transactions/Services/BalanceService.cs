using Banking.Transactions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Transactions;

internal class BalanceService(TransactionsDbContext context) : IBalanceService
{
    public async Task<long> GetBalanceAsync(Guid participantId)
    {
        var entries = await context.JournalEntries
            .Where(je => je.ParticipantId == participantId)
            .Join(
                context.Transactions,
                je => je.TransactionId,
                t => t.Id,
                (je, t) => new { je.Type, t.Amount }
            )
            .ToListAsync();

        return entries.Sum(e => e.Type == JournalEntryType.Debit
            ? e.Amount
            : -e.Amount);
    }
}