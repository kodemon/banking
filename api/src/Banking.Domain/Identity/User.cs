using Banking.Domain.Accounts;
using Banking.Domain.ValueObjects;

namespace Banking.Domain.Identity;

public class User
{
    public Guid Id { get; init; }

    public required Name Name { get; set; }
    public required DateTime DateOfBirth { get; init; }

    internal ICollection<UserAddress> Addresses { get; set; } = new List<UserAddress>();
    internal ICollection<UserEmail> Emails { get; set; } = new List<UserEmail>();

    public ICollection<PersonalAccountHolder> AccountHoldings { get; set; } = new List<PersonalAccountHolder>();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
