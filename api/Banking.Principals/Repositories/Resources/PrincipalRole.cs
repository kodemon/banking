namespace Banking.Principals.Repositories.Resources;

public class PrincipalRole
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public required string Role { get; init; }

    public DateTime CreatedAt { get; init; }
}
