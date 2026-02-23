namespace Banking.Accounts.DTO.Responses;

public record AccountResponse
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
    public required string Currency { get; init; }
    public ICollection<AccountHolderResponse> Holders { get; init; } = new List<AccountHolderResponse>();
    public required long Balance { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public record AccountHolderResponse
{
    public required Guid Id { get; init; }
    public required Guid HolderId { get; init; }
    public required string HolderType { get; init; }
    public required DateTime CreatedAt { get; init; }
}
