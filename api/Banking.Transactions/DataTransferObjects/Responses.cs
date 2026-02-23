namespace Banking.Transactions.DTO.Responses;

public record TransactionResponse
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
    public required string ReferenceNumber { get; init; }
    public required string Description { get; init; }
    public required long Amount { get; init; }
    public required string Currency { get; init; }
    public ICollection<JournalEntryResponse> JournalEntries { get; init; } = new List<JournalEntryResponse>();
    public required DateTime CreatedAt { get; init; }
}

public record JournalEntryResponse
{
    public required Guid Id { get; init; }
    public required Guid ParticipantId { get; init; }
    public required string Type { get; init; }
    public required DateTime CreatedAt { get; init; }
}