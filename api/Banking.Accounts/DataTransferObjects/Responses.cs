namespace Banking.Accounts.DTO.Responses;

public record AccountResponse
{
    public required Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
    public required string Currency { get; init; }
    public ICollection<PersonalHolderResponse> PersonalHolders { get; init; } = new List<PersonalHolderResponse>();
    public ICollection<BusinessHolderResponse> BusinessHolders { get; init; } = new List<BusinessHolderResponse>();
    public required long Balance { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public record PersonalHolderResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string HolderType { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public record BusinessHolderResponse
{
    public required Guid Id { get; init; }
    public required Guid BusinessId { get; init; }
    public required string HolderType { get; init; }
    public required DateTime CreatedAt { get; init; }
}