namespace Banking.Events.Repositories.Resources;

/*
 |--------------------------------------------------------------------------------
 | StoredEvent
 |--------------------------------------------------------------------------------
 |
 | The append-only record of every domain event emitted by Banking.Api.
 | This is the write side of the system — the authoritative, immutable log
 | of everything that has happened.
 |
 | Design decisions:
 |
 |   EventType  — fully-qualified CLR type name (e.g. "Banking.Users.Events.UserCreated").
 |                Allows the event store to deserialize any event without a
 |                separate type registry.
 |
 |   Payload    — JSON-serialized event record. The event's own properties
 |                (EventId, CorrelationId, OccurredAt) are embedded in the
 |                payload — they are duplicated onto StoredEvent columns only
 |                for queryability without deserializing every row.
 |
 |   StreamId   — groups events by aggregate identity (e.g. UserId, AccountId).
 |                Enables efficient replay of all events for a single aggregate.
 |                For events that don't belong to a single aggregate stream
 |                (e.g. a saga correlation event) this is the CorrelationId.
 |
 |   Version    — monotonically increasing position within a stream. Used for
 |                optimistic concurrency and ordered replay.
 |
 | No navigation properties, no foreign keys, no update paths.
 | Once written, a StoredEvent is never modified or deleted.
 |
 */

public class StoredEvent
{
    /// <summary>
    /// Surrogate primary key — sequential for efficient pagination and ordering.
    /// </summary>
    public long SequenceId { get; init; }

    /// <summary>
    /// The EventId from IDomainEvent — globally unique per event occurrence.
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// CorrelationId from ITracedEvent. Null for uncorrelated events
    /// (rare — most events in this system are correlated).
    /// Groups all events from one API request or saga.
    /// </summary>
    public Guid? CorrelationId { get; init; }

    /// <summary>
    /// Groups events by aggregate or saga. For aggregate events this is the
    /// aggregate's Id (UserId, AccountId, etc.). For saga-only events this
    /// is the CorrelationId.
    /// </summary>
    public Guid StreamId { get; init; }

    /// <summary>
    /// Monotonically increasing position within the stream.
    /// Assigned by EventsDbContext.AppendAsync — not by the caller.
    /// </summary>
    public int Version { get; init; }

    /// <summary>
    /// Fully-qualified CLR type name. e.g. "Banking.Users.Events.UserCreated".
    /// </summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// JSON-serialized event record including all properties.
    /// </summary>
    public string Payload { get; init; } = string.Empty;

    /// <summary>
    /// Copied from IDomainEvent.OccurredAt for queryability.
    /// </summary>
    public DateTime OccurredAt { get; init; }

    // EF Core constructor
    private StoredEvent() { }

    internal StoredEvent(
        Guid eventId,
        Guid? correlationId,
        Guid streamId,
        int version,
        string eventType,
        string payload,
        DateTime occurredAt
    )
    {
        EventId = eventId;
        CorrelationId = correlationId;
        StreamId = streamId;
        Version = version;
        EventType = eventType;
        Payload = payload;
        OccurredAt = occurredAt;
    }
}
