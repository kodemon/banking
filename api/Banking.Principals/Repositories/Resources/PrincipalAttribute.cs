namespace Banking.Principals.Repositories.Resources;

internal class PrincipalAttribute
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public required string Domain { get; init; }
    public required string Key { get; init; }
    public required string Value { get; set; }

    public DateTime CreatedAt { get; init; }
}
