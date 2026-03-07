using Banking.Shared.ValueObjects;

namespace Banking.Users.Repositories.Resources;

internal class UserAddress
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public required Address Address { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
