using Banking.Domain.ValueObjects;

namespace Banking.Domain.Transactions;

/*
    Transaction

    Represents specific Transaction data connected to two accompanying JournalEntry records.
    The base information required by a JournalEntry is what is used to determine ones account
    balance which would be the Amount. We record the Amount in the Transaction as a single
    source of truth where the sender and receiver is seperately recorded in a JournalEntry.
 */

public class Transaction
{
    public Guid Id { get; init; }

    public TransactionType Type { get; init; }
    public required TransactionStatus Status { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;
    public required string Description { get; set; }

    public required long Amount { get; init; }
    public required Currency Currency { get; init; }

    public ICollection<JournalEntry> JournalEntries { get; set; } = new List<JournalEntry>();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    Fee,
    Interest,
}

public enum TransactionStatus
{
    Pending,
    Failed,
    Reversed,
    Completed,
}
