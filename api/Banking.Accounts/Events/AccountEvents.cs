using Banking.Shared.Messaging;

namespace Banking.Accounts.Events;

/*
 |--------------------------------------------------------------------------------
 | Account Domain Events
 |--------------------------------------------------------------------------------
 |
 | Owned by Banking.Accounts. Public so Banking.Api can emit them.
 | Internal handlers in Banking.Accounts react to them — no other domain
 | ever imports these.
 |
 | AccountType and HolderType are carried as strings — the enums are
 | internal to Banking.Accounts and must not cross the module boundary.
 |
 */

public record AccountCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid AccountId,
    string AccountType,
    string CurrencyCode,
    Guid PrimaryHolderId,
    string PrimaryHolderType
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record AccountHolderAdded(
    Guid EventId,
    Guid CorrelationId,
    Guid AccountId,
    Guid HolderId,
    string HolderType
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record AccountHolderRemoved(Guid EventId, Guid CorrelationId, Guid AccountId, Guid HolderId)
    : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record AccountFrozen(Guid EventId, Guid CorrelationId, Guid AccountId) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record AccountUnfrozen(Guid EventId, Guid CorrelationId, Guid AccountId) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record AccountClosed(Guid EventId, Guid CorrelationId, Guid AccountId) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
