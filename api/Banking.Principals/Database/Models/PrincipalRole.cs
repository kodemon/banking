namespace Banking.Principals.Database.Models;

internal class PrincipalRole
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public string Role { get; init; }

    public DateTime CreatedAt { get; init; }

    public PrincipalRole(Guid principalId, string role)
    {
        Id = Guid.NewGuid();
        PrincipalId = principalId;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}
