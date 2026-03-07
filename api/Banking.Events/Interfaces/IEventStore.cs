namespace Banking.Events.Interface;

/*
 |--------------------------------------------------------------------------------
 | IEventStore
 |--------------------------------------------------------------------------------
 |
 | Write and read path for a domain's own event log. Each domain registers its
 | own IEventStore implementation backed by its own DbContext — there is no
 | central store. Banking.Principals owns principal events in principals.db,
 | Banking.Users owns user events in users.db, and so on.
 |
 | Banking.Api command handlers resolve the correct store by injecting the
 | domain's DbContext directly, which is passed to EventStore<TContext>.
 |
 */

public interface IEventStore
{
    /// <summary>
    /// Appends a single event to the stream identified by <paramref name="streamId"/>.
    /// Version is (current max for stream) + 1.
    /// </summary>
    Task AppendAsync<TEvent>(TEvent domainEvent, Guid streamId, CancellationToken ct = default)
        where TEvent : ICorrelatedEvent;

    /// <summary>
    /// Appends multiple events to the same stream, assigning sequential versions.
    /// </summary>
    Task AppendManyAsync<TEvent>(
        IEnumerable<TEvent> domainEvents,
        Guid streamId,
        CancellationToken ct = default
    )
        where TEvent : ICorrelatedEvent;

    /// <summary>
    /// Returns all events for a stream in version order.
    /// </summary>
    Task<IReadOnlyList<StoredEvent>> GetStreamAsync(Guid streamId, CancellationToken ct = default);

    /// <summary>
    /// Returns all events for a correlation ID in sequence order.
    /// </summary>
    Task<IReadOnlyList<StoredEvent>> GetCorrelationAsync(
        Guid correlationId,
        CancellationToken ct = default
    );
}

public class EventStreamConflictException(Guid streamId, int version)
    : Exception(
        $"Stream {streamId} already has an event at version {version}. Optimistic concurrency conflict."
    );
