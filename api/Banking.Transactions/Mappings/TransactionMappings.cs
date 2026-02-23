using Banking.Transactions.DTO.Responses;

namespace Banking.Transactions;

internal static class TransactionMappings
{
    public static TransactionResponse ToResponse(this Transaction transaction) => new()
    {
        Id = transaction.Id,
        Type = transaction.Type.ToString(),
        Status = transaction.Status.ToString(),
        ReferenceNumber = transaction.ReferenceNumber,
        Description = transaction.Description,
        Amount = transaction.Amount,
        Currency = transaction.Currency.Code,
        JournalEntries = transaction.JournalEntries.Select(je => je.ToResponse()).ToList(),
        CreatedAt = transaction.CreatedAt
    };

    public static JournalEntryResponse ToResponse(this JournalEntry entry) => new()
    {
        Id = entry.Id,
        ParticipantId = entry.ParticipantId,
        Type = entry.Type.ToString(),
        CreatedAt = entry.CreatedAt
    };
}