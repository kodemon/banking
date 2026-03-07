using System.Text.Json;
using Banking.Events.Interface;
using Microsoft.EntityFrameworkCore;

namespace Banking.Events;

/*
 |--------------------------------------------------------------------------------
 | EventStore<TContext>
 |--------------------------------------------------------------------------------
 |
 | Generic implementation of IEventStore that works against any DbContext
 | implementing IHasEvents. Each domain instantiates this with its own
 | DbContext — the event log is stored in that domain's SQLite file.
 |
 | There is no central event store. Each domain owns and queries its own
 | history. Banking.Api command handlers pass the domain DbContext in directly.
 |
 */

public class EventStore<TContext>(TContext db) : IEventStore
    where TContext : DbContext, IHasEvents
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task AppendAsync<TEvent>(
        TEvent domainEvent,
        Guid streamId,
        CancellationToken ct = default
    )
        where TEvent : ICorrelatedEvent
    {
        var version = await NextVersionAsync(streamId, ct);
        await db.Events.AddAsync(ToStoredEvent(domainEvent, streamId, version), ct);
    }

    public async Task AppendManyAsync<TEvent>(
        IEnumerable<TEvent> domainEvents,
        Guid streamId,
        CancellationToken ct = default
    )
        where TEvent : ICorrelatedEvent
    {
        var version = await NextVersionAsync(streamId, ct);
        var stored = domainEvents.Select(e => ToStoredEvent(e, streamId, version++));
        await db.Events.AddRangeAsync(stored, ct);
    }

    public async Task<IReadOnlyList<StoredEvent>> GetStreamAsync(
        Guid streamId,
        CancellationToken ct = default
    )
    {
        return await db
            .Events.Where(e => e.StreamId == streamId)
            .OrderBy(e => e.Version)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<StoredEvent>> GetCorrelationAsync(
        Guid correlationId,
        CancellationToken ct = default
    )
    {
        return await db
            .Events.Where(e => e.CorrelationId == correlationId)
            .OrderBy(e => e.SequenceId)
            .ToListAsync(ct);
    }

    private async Task<int> NextVersionAsync(Guid streamId, CancellationToken ct)
    {
        var max = await db
            .Events.Where(e => e.StreamId == streamId)
            .MaxAsync(e => (int?)e.Version, ct);
        return (max ?? 0) + 1;
    }

    private static StoredEvent ToStoredEvent<TEvent>(TEvent domainEvent, Guid streamId, int version)
        where TEvent : ICorrelatedEvent
    {
        return new StoredEvent(
            eventId: domainEvent.EventId,
            correlationId: domainEvent.CorrelationId,
            streamId: streamId,
            version: version,
            eventType: domainEvent.GetType().FullName!,
            payload: JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptions),
            occurredAt: domainEvent.OccurredAt
        );
    }
}
