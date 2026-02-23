using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Transactions;

/*
 |--------------------------------------------------------------------------------
 | Transaction [Aggregate Root]
 |--------------------------------------------------------------------------------
 |
 | A Transaction is the single source of truth for an amount movement. It owns
 | two JournalEntry records — one Credit (money out of an account) and one Debit
 | (money into an account) — implementing double-entry bookkeeping.
 |
 | The Transaction aggregate is responsible for creating its journal entry pair.
 | AccountId references are plain Guids — no navigation properties into
 | Banking.Accounts.
 |
 */

internal class Transaction
{
    public Guid Id { get; init; }

    public TransactionType Type { get; init; }
    public TransactionStatus Status { get; private set; }

    public string ReferenceNumber { get; private set; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public long Amount { get; init; }
    public Currency Currency { get; init; } = null!;

    private readonly List<JournalEntry> _journalEntries = new();
    public IReadOnlyCollection<JournalEntry> JournalEntries => _journalEntries.AsReadOnly();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private Transaction()
    {
        Currency = null!;
        Description = null!;
    }

    private Transaction(
        TransactionType type,
        string description,
        long amount,
        Currency currency)
    {
        Id = Guid.NewGuid();
        Type = type;
        Status = TransactionStatus.Pending;
        Description = description;
        Amount = amount;
        Currency = currency;
        ReferenceNumber = GenerateReferenceNumber();
    }

    /*
     |--------------------------------------------------------------------------------
     | Factory Methods
     |--------------------------------------------------------------------------------
     |
     | Each transaction type enforces which accounts are involved. A Transfer
     | requires both a source and destination account. A Deposit only has a
     | destination. A Withdrawal only has a source.
     |
     */

    public static Transaction CreateDeposit(
        Guid destinationAccountId,
        long amount,
        Currency currency,
        string description)
    {
        var transaction = new Transaction(TransactionType.Deposit, description, amount, currency);
        transaction._journalEntries.Add(JournalEntry.Debit(transaction.Id, destinationAccountId, amount));
        return transaction;
    }

    public static Transaction CreateWithdrawal(
        Guid sourceAccountId,
        long amount,
        Currency currency,
        string description)
    {
        var transaction = new Transaction(TransactionType.Withdrawal, description, amount, currency);
        transaction._journalEntries.Add(JournalEntry.Credit(transaction.Id, sourceAccountId, amount));
        return transaction;
    }

    public static Transaction CreateTransfer(
        Guid sourceAccountId,
        Guid destinationAccountId,
        long amount,
        Currency currency,
        string description)
    {
        if (sourceAccountId == destinationAccountId)
            throw new InvalidAggregateOperationException("Source and destination accounts must be different");

        var transaction = new Transaction(TransactionType.Transfer, description, amount, currency);
        transaction._journalEntries.Add(JournalEntry.Credit(transaction.Id, sourceAccountId, amount));
        transaction._journalEntries.Add(JournalEntry.Debit(transaction.Id, destinationAccountId, amount));
        return transaction;
    }

    public static Transaction CreateFee(
        Guid accountId,
        long amount,
        Currency currency,
        string description)
    {
        var transaction = new Transaction(TransactionType.Fee, description, amount, currency);
        transaction._journalEntries.Add(JournalEntry.Credit(transaction.Id, accountId, amount));
        return transaction;
    }

    public static Transaction CreateInterest(
        Guid accountId,
        long amount,
        Currency currency,
        string description)
    {
        var transaction = new Transaction(TransactionType.Interest, description, amount, currency);
        transaction._journalEntries.Add(JournalEntry.Debit(transaction.Id, accountId, amount));
        return transaction;
    }

    /*
     |--------------------------------------------------------------------------------
     | Status Transitions
     |--------------------------------------------------------------------------------
     */

    public void Complete()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidAggregateOperationException($"Cannot complete a transaction with status {Status}");
        Status = TransactionStatus.Completed;
    }

    public void Fail()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidAggregateOperationException($"Cannot fail a transaction with status {Status}");
        Status = TransactionStatus.Failed;
    }

    public void Reverse()
    {
        if (Status != TransactionStatus.Completed)
            throw new InvalidAggregateOperationException("Only completed transactions can be reversed");
        Status = TransactionStatus.Reversed;
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers
     |--------------------------------------------------------------------------------
     */

    private static string GenerateReferenceNumber()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
    }
}

/*
 |--------------------------------------------------------------------------------
 | Journal Entry
 |--------------------------------------------------------------------------------
 |
 | Owned by Transaction. AccountId is a plain Guid — no navigation property
 | into Banking.Accounts. The Banking.Accounts module maintains its own local
 | read model (AccountJournalEntry) mapping to this same table for balance
 | calculations.
 |
 */

internal class JournalEntry
{
    public Guid Id { get; init; }

    public Guid TransactionId { get; init; }

    // Plain Guid — no navigation property into Banking.Accounts
    public Guid AccountId { get; init; }

    public JournalEntryType Type { get; init; }
    public long Amount { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private JournalEntry() { }

    internal static JournalEntry Credit(Guid transactionId, Guid accountId, long amount) => new()
    {
        Id = Guid.NewGuid(),
        TransactionId = transactionId,
        AccountId = accountId,
        Type = JournalEntryType.Credit,
        Amount = amount
    };

    internal static JournalEntry Debit(Guid transactionId, Guid accountId, long amount) => new()
    {
        Id = Guid.NewGuid(),
        TransactionId = transactionId,
        AccountId = accountId,
        Type = JournalEntryType.Debit,
        Amount = amount
    };
}

/*
 |--------------------------------------------------------------------------------
 | Enums
 |--------------------------------------------------------------------------------
 */

internal enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    Fee,
    Interest,
}

internal enum TransactionStatus
{
    Pending,
    Failed,
    Reversed,
    Completed,
}

internal enum JournalEntryType
{
    Credit,
    Debit,
}