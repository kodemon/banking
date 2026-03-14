namespace Banking.Principals.Database.Models;

internal class PrincipalSession
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    private PrincipalSession() { }

    public PrincipalSession(Guid principalId, TimeSpan lifetime)
    {
        Id = Guid.NewGuid();
        PrincipalId = principalId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.Add(lifetime);
    }
}
