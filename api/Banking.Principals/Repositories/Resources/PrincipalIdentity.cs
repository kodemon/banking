namespace Banking.Principals.Repositories.Resources;

public class PrincipalIdentity
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public required string Provider { get; init; }
    public required string ExternalId { get; init; }

    public DateTime CreatedAt { get; init; }
}
