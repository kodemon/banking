namespace Banking.OCSF.Database.Models;

/// <summary>
/// Represents a persisted OCSF audit event in the database.
///
/// This table is append-only by design:
///   - The application DB user has INSERT only, no UPDATE or DELETE.
///   - Each row stores a SHA-256 hash of its own content chained with
///     the hash of the previous row, making undetected tampering infeasible.
///
/// The full OCSF event payload is stored as JSONB, giving you both
/// schema compliance and queryability within PostgreSQL.
/// </summary>
internal class AuditLogEntry
{
    /// <summary>
    /// Sequential database ID. Used to establish hash chain ordering.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// The OCSF Event ID from the original event (EventId on the event class).
    /// Unique — prevents duplicate persistence if the consumer replays a message.
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// OCSF Class UID (e.g. 3002 for Authentication).
    /// Stored as a column for efficient filtering without JSON parsing.
    /// </summary>
    public int ClassUid { get; init; }

    /// <summary>
    /// OCSF Category UID (e.g. 3 for Identity & Access Management).
    /// </summary>
    public int CategoryUid { get; init; }

    /// <summary>
    /// When the event occurred, in UTC. From the event's OccurredAt field.
    /// Stored as a column for efficient time-range queries.
    /// </summary>
    public DateTime OccurredAt { get; init; }

    /// <summary>
    /// When this row was written to the database, in UTC.
    /// Distinct from OccurredAt — allows detection of delayed delivery.
    /// </summary>
    public DateTime PersistedAt { get; init; }

    /// <summary>
    /// The full OCSF event serialised as JSON.
    /// Stored as JSONB in PostgreSQL for queryability.
    /// </summary>
    public string Payload { get; init; }

    /// <summary>
    /// SHA-256 hash of: Payload + PreviousHash.
    /// Chaining the previous hash makes it impossible to tamper with
    /// an earlier record without invalidating all subsequent hashes.
    /// </summary>
    public string Hash { get; init; }

    /// <summary>
    /// The Hash value of the immediately preceding row (by Id).
    /// Null only for the very first row ever written.
    /// </summary>
    public string? PreviousHash { get; init; }

    public AuditLogEntry(
        Guid eventId,
        int classUid,
        int categoryUid,
        DateTime occurredAt,
        string payload,
        string hash,
        string? previousHash
    )
    {
        EventId = eventId;
        ClassUid = classUid;
        CategoryUid = categoryUid;
        OccurredAt = occurredAt;
        PersistedAt = DateTime.UtcNow;
        Payload = payload;
        Hash = hash;
        PreviousHash = previousHash;
    }
}
