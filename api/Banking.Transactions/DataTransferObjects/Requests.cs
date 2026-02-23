namespace Banking.Transactions.DTO.Requests;

internal record CreateDepositRequest
{
    public required Guid DestinationParticipantId { get; init; }
    public required long Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required string Description { get; init; }
}

internal record CreateWithdrawalRequest
{
    public required Guid SourceParticipantId { get; init; }
    public required long Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required string Description { get; init; }
}

internal record CreateTransferRequest
{
    public required Guid SourceParticipantId { get; init; }
    public required Guid DestinationParticipantId { get; init; }
    public required long Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required string Description { get; init; }
}