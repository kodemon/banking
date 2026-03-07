using Banking.Events.Repositories.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Events.Persistence;

public class StoredEventConfiguration : IEntityTypeConfiguration<StoredEvent>
{
    public void Configure(EntityTypeBuilder<StoredEvent> builder)
    {
        builder.ToTable("event");

        // Surrogate PK — auto-increment gives us global ordering for free.
        builder.HasKey(e => e.SequenceId);
        builder.Property(e => e.SequenceId).ValueGeneratedOnAdd();

        builder.Property(e => e.EventId).IsRequired();

        builder.Property(e => e.CorrelationId); // nullable — Guid?

        builder.Property(e => e.StreamId).IsRequired();

        builder.Property(e => e.Version).IsRequired();

        builder.Property(e => e.EventType).HasMaxLength(500).IsRequired();

        builder.Property(e => e.Payload).IsRequired();

        builder.Property(e => e.OccurredAt).IsRequired();

        // Unique: no two events can occupy the same position in the same stream.
        // This is the optimistic concurrency guard — if two writers try to append
        // Version=3 to StreamId=X, one will get a unique constraint violation.

        builder.HasIndex(e => new { e.StreamId, e.Version }).IsUnique();

        // Fast lookup of all events for a single aggregate replay.

        builder.HasIndex(e => e.StreamId);

        // Fast lookup of all events in a saga/request trace.

        builder.HasIndex(e => e.CorrelationId);

        // Global time-ordered scan — used by the event store reader and projections.

        builder.HasIndex(e => e.OccurredAt);

        // Globally unique event identity — guards against double-emit.

        builder.HasIndex(e => e.EventId).IsUnique();
    }
}
