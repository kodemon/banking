namespace Banking.Transactions;

/*
 |--------------------------------------------------------------------------------
 | IBalanceService
 |--------------------------------------------------------------------------------
 |
 | Public interface exposed by Banking.Transactions for cross-module balance
 | queries. Banking.Accounts depends on this to calculate account balances
 | without reaching into the JournalEntries table directly.
 |
 */

public interface IBalanceService
{
    Task<long> GetBalanceAsync(Guid participantId);
}