using Banking.Shared.ValueObjects;

namespace Banking.Users.Repositories.Resources;

internal class UserEmail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public required Email Email { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
