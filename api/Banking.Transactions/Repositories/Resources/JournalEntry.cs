using Banking.Transactions.Enums;

namespace Banking.Transactions.Repositories.Resources;

internal class JournalEntry
{
    public Guid Id { get; init; }
    public Guid TransactionId { get; init; }
    public Guid ParticipantId { get; init; } // external reference

    public JournalEntryType Type { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private JournalEntry() { }

    internal static JournalEntry Credit(Guid transactionId, Guid participantId) =>
        new()
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            ParticipantId = participantId,
            Type = JournalEntryType.Credit,
        };

    internal static JournalEntry Debit(Guid transactionId, Guid participantId) =>
        new()
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            ParticipantId = participantId,
            Type = JournalEntryType.Debit,
        };
}
