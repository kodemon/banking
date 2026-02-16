namespace Banking.Domain.Identity;

public class UserAddress
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }

    public string? Region { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
