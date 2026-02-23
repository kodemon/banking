using Banking.Shared.ValueObjects;

namespace Banking.Accounts.DTO.Requests;

internal record CreateAccountRequest
{
    public required AccountType Type { get; init; }
    public required string CurrencyCode { get; init; }
}

internal record AddPersonalHolderRequest
{
    public required Guid UserId { get; init; }
    public required PersonalHolderType HolderType { get; init; }
}

internal record AddBusinessHolderRequest
{
    public required Guid BusinessId { get; init; }
    public required BusinessHolderType HolderType { get; init; }
}