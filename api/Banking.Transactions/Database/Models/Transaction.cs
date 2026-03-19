using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Transactions.Enums;

namespace Banking.Transactions.Database.Models;

internal class Transaction
{
    public Guid Id { get; init; }

    public TransactionType Type { get; init; }
    public TransactionStatus Status { get; private set; }

    public string ReferenceNumber { get; private set; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public long Amount { get; init; }
    public Currency Currency { get; init; } = null!;

    public readonly List<JournalEntry> JournalEntries = new();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private Transaction()
    {
        Currency = null!;
        Description = null!;
    }

    private Transaction(TransactionType type, string description, long amount, Currency currency)
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
     */

    public static Transaction CreateDeposit(
        Guid destinationParticipantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = new Transaction(TransactionType.Deposit, description, amount, currency);
        transaction.JournalEntries.Add(
            JournalEntry.Debit(transaction.Id, destinationParticipantId)
        );
        return transaction;
    }

    public static Transaction CreateWithdrawal(
        Guid sourceParticipantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = new Transaction(
            TransactionType.Withdrawal,
            description,
            amount,
            currency
        );
        transaction.JournalEntries.Add(JournalEntry.Credit(transaction.Id, sourceParticipantId));
        return transaction;
    }

    public static Transaction CreateTransfer(
        Guid sourceParticipantId,
        Guid destinationParticipantId,
        long amount,
        Currency currency,
        string description
    )
    {
        if (sourceParticipantId == destinationParticipantId)
            throw new InvalidAggregateOperationException(
                "Source and destination participants must be different"
            );

        var transaction = new Transaction(TransactionType.Transfer, description, amount, currency);
        transaction.JournalEntries.Add(JournalEntry.Credit(transaction.Id, sourceParticipantId));
        transaction.JournalEntries.Add(
            JournalEntry.Debit(transaction.Id, destinationParticipantId)
        );
        return transaction;
    }

    public static Transaction CreateFee(
        Guid participantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = new Transaction(TransactionType.Fee, description, amount, currency);
        transaction.JournalEntries.Add(JournalEntry.Credit(transaction.Id, participantId));
        return transaction;
    }

    public static Transaction CreateInterest(
        Guid participantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = new Transaction(TransactionType.Interest, description, amount, currency);
        transaction.JournalEntries.Add(JournalEntry.Debit(transaction.Id, participantId));
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
            throw new InvalidAggregateOperationException(
                $"Cannot complete a transaction with status {Status}"
            );
        Status = TransactionStatus.Completed;
    }

    public void Fail()
    {
        if (Status != TransactionStatus.Pending)
            throw new InvalidAggregateOperationException(
                $"Cannot fail a transaction with status {Status}"
            );
        Status = TransactionStatus.Failed;
    }

    public void Reverse()
    {
        if (Status != TransactionStatus.Completed)
            throw new InvalidAggregateOperationException(
                "Only completed transactions can be reversed"
            );
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
