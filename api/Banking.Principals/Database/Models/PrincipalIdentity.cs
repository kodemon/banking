namespace Banking.Principals.Database.Models;

internal class PrincipalIdentity
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public string Provider { get; init; }
    public string ExternalId { get; init; }

    public DateTime CreatedAt { get; init; }

    public PrincipalIdentity(Guid principalId, string provider, string externalId)
    {
        Id = Guid.NewGuid();
        PrincipalId = principalId;
        Provider = provider;
        ExternalId = externalId;
        CreatedAt = DateTime.UtcNow;
    }
}
