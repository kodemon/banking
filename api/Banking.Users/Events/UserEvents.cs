using Banking.Shared.Messaging;
using Banking.Shared.ValueObjects;

namespace Banking.Users.Events;

/*
 |--------------------------------------------------------------------------------
 | User Domain Events
 |--------------------------------------------------------------------------------
 |
 | Owned by Banking.Users. These records are public so Banking.Api can
 | reference and emit them via commands. Banking.Users handlers react to
 | them internally — no other domain ever imports these.
 |
 | All events implement ITracedEvent so they carry a CorrelationId
 | linking them back to the originating API request or saga.
 |
 */

public record UserCreated(
    Guid EventId,
    Guid CorrelationId,
    Guid UserId,
    NameInput Name,
    DateTime DateOfBirth,
    string Email
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserNameUpdated(Guid EventId, Guid CorrelationId, Guid UserId, NameInput Name)
    : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserDeleted(Guid EventId, Guid CorrelationId, Guid UserId) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserEmailAdded(
    Guid EventId,
    Guid CorrelationId,
    Guid UserId,
    Guid EmailId,
    string Address,
    EmailType Type
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserEmailRemoved(Guid EventId, Guid CorrelationId, Guid UserId, Guid EmailId)
    : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserAddressAdded(
    Guid EventId,
    Guid CorrelationId,
    Guid UserId,
    Guid AddressId,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? Region
) : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record UserAddressRemoved(Guid EventId, Guid CorrelationId, Guid UserId, Guid AddressId)
    : ITracedEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
