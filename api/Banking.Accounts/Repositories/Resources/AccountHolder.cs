using Banking.Accounts.Enums;

namespace Banking.Accounts.Repositories.Resources;

internal class AccountHolder
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid HolderId { get; init; } // external reference

    public AccountHolderType HolderType { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
