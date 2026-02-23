namespace Banking.Accounts.DTO.Requests;

internal record CreateAccountRequest
{
    public required Guid HolderId { get; init; }
    public required AccountHolderType HolderType { get; init; }
    public required AccountType Type { get; init; }
    public required string CurrencyCode { get; init; }
}

internal record AddAccountHolderRequest
{
    public required Guid HolderId { get; init; }
    public required AccountHolderType HolderType { get; init; }
}
