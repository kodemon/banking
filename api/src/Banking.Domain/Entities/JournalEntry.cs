using Banking.Domain.Enums;

namespace Banking.Domain.Entities;

/*
    Journal Entry

    Implements double-entry bookkeeping of transactions for an account. Every transaction should result
    in two entries, one credit, and one debit. This tracks who is sending and receiving funds
    for a given transaction.

    For example:

        John (AccountId Y) is sending Jane (AccountId Z) an amount of 100 in a Transfer Transaction (TransactionId A).
        The following journal entries would be logged.

        JournalEntry
            Id: guid
            AccountId: Y
            TransactionId: A
            Type: Credit

        JournalEntry
            Id: guid
            AccountId: Z
            TransactionId: A
            Type: Debit

    The credit records money that flows out of an account, and debit records money that flows into an account.
    The value amount of the transfer is recorded in the Transaction, so when performing a balance calculation
    we fetch all the journal entries for an account with related reference to the amount from the transaction
    and sum all the entries based on the type.

    The type of transaction does not matter and is used more for filtering, or identifying purposes. What defines
    account balance is the journal entries.
 */

public class JournalEntry
{
    public Guid Id { get; init; }

    public Guid AccountId { get; init; }
    public Account Account { get; set; }

    public Guid TransactionId { get; init; }
    public Transaction Transaction { get; set; }

    public JournalEntryType Type { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
