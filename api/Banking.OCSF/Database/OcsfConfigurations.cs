using Banking.OCSF.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.OCSF.Database;

internal class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.HasKey(e => e.Id);

        // Database generates the sequential ID — never set by application code.
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.EventId).IsRequired();
        builder.Property(e => e.ClassUid).IsRequired();
        builder.Property(e => e.CategoryUid).IsRequired();
        builder.Property(e => e.OccurredAt).IsRequired();
        builder.Property(e => e.PersistedAt).IsRequired();

        // Full OCSF payload stored as JSONB for queryability.
        builder.Property(e => e.Payload).HasColumnType("jsonb").IsRequired();

        builder
            .Property(e => e.Hash)
            .HasMaxLength(64) // SHA-256 hex is always 64 chars
            .IsRequired();

        builder.Property(e => e.PreviousHash).HasMaxLength(64);

        // Prevent duplicate persistence on consumer replay.
        builder.HasIndex(e => e.EventId).IsUnique();

        // Efficient time-range queries for compliance reporting.
        builder.HasIndex(e => e.OccurredAt);

        // Efficient filtering by event class (e.g. all authentication events).
        builder.HasIndex(e => e.ClassUid);
    }
}
