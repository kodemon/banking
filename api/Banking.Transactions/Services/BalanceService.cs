using Banking.Transactions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Transactions;

internal class BalanceService(TransactionsDbContext context) : IBalanceService
{
    public async Task<long> GetBalanceAsync(Guid accountId)
    {
        var entries = await context.JournalEntries
            .Where(je => je.AccountId == accountId)
            .ToListAsync();

        return entries.Sum(je => je.Type == JournalEntryType.Debit
            ? je.Amount
            : -je.Amount);
    }
}