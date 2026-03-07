using Banking.Events.Interface;

namespace Banking.Principals.EventStore.Events;

public record PrincipalCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid PrincipalId,
    string Provider,
    string ExternalId
) : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalDeleted(Guid EventId, Guid CorrelationId, Guid PrincipalId)
    : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalIdentityAdded(
    Guid EventId,
    Guid CorrelationId,
    Guid PrincipalId,
    string Provider,
    string ExternalId
) : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalIdentityRemoved(
    Guid EventId,
    Guid CorrelationId,
    Guid PrincipalId,
    string Provider,
    string ExternalId
) : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalRoleAdded(Guid EventId, Guid CorrelationId, Guid PrincipalId, string Role)
    : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalRoleRemoved(Guid EventId, Guid CorrelationId, Guid PrincipalId, string Role)
    : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalAttributeSet(
    Guid EventId,
    Guid CorrelationId,
    Guid PrincipalId,
    string Domain,
    string Key,
    string Value
) : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record PrincipalAttributeRemoved(
    Guid EventId,
    Guid CorrelationId,
    Guid PrincipalId,
    string Domain,
    string Key
) : ICorrelatedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
