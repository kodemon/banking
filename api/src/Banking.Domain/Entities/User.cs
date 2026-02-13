namespace Banking.Domain.Entities;

public class User
{
    public Guid Id { get; init; }

    public required string Email { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

