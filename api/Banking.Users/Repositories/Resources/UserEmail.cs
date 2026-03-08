using Banking.Shared.ValueObjects;

namespace Banking.Users.Repositories.Resources;

internal class UserEmail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public Email Email { get; init; }

    public DateTime CreatedAt { get; init; }

    private UserEmail()
    {
        Email = null!;
    }

    public UserEmail(Guid userId, Email email)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
}
