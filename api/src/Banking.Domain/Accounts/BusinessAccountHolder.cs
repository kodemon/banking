using Banking.Domain.Identity;

namespace Banking.Domain.Accounts;

public class BusinessAccountHolder
{
    public Guid Id { get; init; }

    public Guid BusinessId { get; init; }
    public Business Business { get; set; } = null!;

    public Guid AccountId { get; init; }
    public Account Account { get; set; } = null!;

    public BusinessHolderType HolderType { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public enum BusinessHolderType
{
    Operating,   // Main business account
    Trust,       // Trust account
    Escrow,      // Escrow/holding
    Investment,  // Investment account
    Payroll      // Payroll account
}
