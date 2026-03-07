namespace Banking.Events.Interface;

/*
 |--------------------------------------------------------------------------------
 | Domain Event Interfaces
 |--------------------------------------------------------------------------------
 |
 | These are the only event-related contracts Banking.Shared owns. Domains
 | implement these on their own event records. Banking.Api references domain
 | projects directly and works with the concrete event types — these interfaces
 | exist for Wolverine handler discovery, event store serialization, and
 | cross-cutting concerns (logging, tracing) that need to treat any event
 | uniformly without knowing its concrete type.
 |
 | NOTE: The property name "CorrelationId" is intentional. Wolverine intercepts any
 | message with a property named "CorrelationId" and attempts saga state
 | resolution against it. Using CorrelationId avoids that convention entirely.
 |
 */

/// <summary>
/// Marker interface for all domain events.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

/// <summary>
/// Extended marker for events that carry a trace identifier linking them
/// back to the originating command. All events emitted by Banking.Api
/// command handlers implement this, allowing the full chain of events
/// from a single API request to be grouped end-to-end in logs and the
/// event store.
/// </summary>
public interface ICorrelatedEvent : IDomainEvent
{
    Guid CorrelationId { get; }
}
