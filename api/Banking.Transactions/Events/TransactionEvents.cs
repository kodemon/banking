using Banking.Shared.Messaging;

namespace Banking.Transactions.Events;

/*
 |--------------------------------------------------------------------------------
 | Transaction Domain Events
 |--------------------------------------------------------------------------------
 |
 | Owned by Banking.Transactions. Public so Banking.Api can emit them.
 | Internal handlers in Banking.Transactions react to them — no other domain
 | ever imports these.
 |
 | Amount is always in the smallest currency unit (øre, cents, pence).
 | ParticipantId references an Account by plain Guid — no cross-domain type.
 |
 */

public record DepositCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid TransactionId,
    Guid DestinationParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record WithdrawalCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid TransactionId,
    Guid SourceParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record TransferCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid TransactionId,
    Guid SourceParticipantId,
    Guid DestinationParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record FeeCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid TransactionId,
    Guid ParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record InterestCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid TransactionId,
    Guid ParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record TransactionCompleted(Guid EventId, Guid CorrelationId, Guid TransactionId)
    : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record TransactionFailed(Guid EventId, Guid CorrelationId, Guid TransactionId, string Reason)
    : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record TransactionReversed(
    Guid EventId,
    Guid CorrelationId,
    Guid TransactionId,
    string Reason
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
