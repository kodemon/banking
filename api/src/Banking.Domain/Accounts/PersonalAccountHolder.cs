using Banking.Domain.Identity;

namespace Banking.Domain.Accounts;

public class PersonalAccountHolder
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }
    public User User { get; set; } = null!;

    public Guid AccountId { get; init; }
    public Account Account { get; set; } = null!;

    public PersonalHolderType HolderType { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public enum PersonalHolderType
{
    Primary,           // Main owner
    Beneficiary,       // Receives on death
    Guardian,          // Manages for a minor
    PowerOfAttorney,   // Legal authority
    Custodian          // Holds for someone else
}
