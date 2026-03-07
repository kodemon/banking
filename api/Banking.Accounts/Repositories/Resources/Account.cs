using Banking.Accounts.Enums;
using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Accounts.Repositories.Resources;

internal class Account
{
    public Guid Id { get; init; }

    public AccountType Type { get; init; }
    public AccountStatus Status { get; set; }
    public Currency Currency { get; init; } = null!;

    public readonly List<AccountHolder> AccountHolders = new();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private Account() { }

    public Account(AccountType type, Currency currency)
    {
        Id = Guid.NewGuid();
        Type = type;
        Status = AccountStatus.Active;
        Currency = currency;
    }

    /*
     |--------------------------------------------------------------------------------
     | Status
     |--------------------------------------------------------------------------------
     */

    public Account Freeze()
    {
        if (Status == AccountStatus.Closed)
        {
            throw new InvalidAggregateOperationException("Cannot freeze a closed account");
        }
        Status = AccountStatus.Frozen;
        return this;
    }

    public Account Unfreeze()
    {
        if (Status == AccountStatus.Closed)
        {
            throw new InvalidAggregateOperationException("Cannot unfreeze a closed account");
        }
        Status = AccountStatus.Active;
        return this;
    }

    public Account Close()
    {
        if (Status == AccountStatus.Closed)
        {
            throw new InvalidAggregateOperationException("Account is already closed");
        }
        Status = AccountStatus.Closed;
        return this;
    }

    /*
     |--------------------------------------------------------------------------------
     | Holders
     |--------------------------------------------------------------------------------
     */

    public Account AddHolder(Guid holderId, AccountHolderType holderType)
    {
        if (AccountHolders.Any(h => h.HolderId == holderId))
        {
            throw new AggregateConflictException($"Holder {holderId} is already on account {Id}");
        }

        var holder = new AccountHolder
        {
            Id = Guid.NewGuid(),
            AccountId = Id,
            HolderId = holderId,
            HolderType = holderType,
        };

        AccountHolders.Add(holder);

        return this;
    }

    public Account RemoveHolder(Guid holderId)
    {
        var holder = AccountHolders.FirstOrDefault(h => h.Id == holderId);
        if (holder is null)
        {
            throw new AggregateNotFoundException($"Holder {holderId} not found on account {Id}");
        }

        AccountHolders.Remove(holder);

        return this;
    }
}
