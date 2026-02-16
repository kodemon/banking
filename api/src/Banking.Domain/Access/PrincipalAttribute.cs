namespace Banking.Domain.Access;

public class PrincipalAttribute
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    public required string Key { get; set; }
    public required string Value { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
