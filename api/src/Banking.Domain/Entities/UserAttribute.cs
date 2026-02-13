using Banking.Domain.Enums;

namespace Banking.Domain.Entities;

public class UserAttribute
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public required UserAttributeKey Key { get; set; }
    public required string Value { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
