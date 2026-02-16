using Banking.Domain.Accounts;

namespace Banking.Domain.Identity;

public class Business
{
    public Guid Id { get; init; }

    public required string Name { get; set; }
    public required string OrganizationNumber { get; init; }

    internal ICollection<BusinessAddress> Addresses { get; set; } = new List<BusinessAddress>();
    internal ICollection<BusinessEmail> Emails { get; set; } = new List<BusinessEmail>();
    internal ICollection<BusinessPhone> Phones { get; set; } = new List<BusinessPhone>();

    public ICollection<BusinessAccountHolder> AccountHoldings { get; set; } = new List<BusinessAccountHolder>();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
