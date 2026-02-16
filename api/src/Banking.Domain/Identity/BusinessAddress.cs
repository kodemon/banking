namespace Banking.Domain.Identity;

public class BusinessAddress
{
    public Guid Id { get; init; }
    public Guid BusinessId { get; init; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }

    public string? Region { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
