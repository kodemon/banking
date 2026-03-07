using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Accounts;

/*
 |--------------------------------------------------------------------------------
 | Account [Aggregate Root]
 |--------------------------------------------------------------------------------
 */

public class Account
{
    public Guid Id { get; init; }

    public AccountType Type { get; init; }
    public AccountStatus Status { get; set; }
    public Currency Currency { get; init; } = null!;

    private readonly List<AccountHolder> _accountHolders = new();
    public IReadOnlyCollection<AccountHolder> AccountHolders => _accountHolders.AsReadOnly();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    private Account() { }

    /// <summary>Reconstitutes an Account from an AccountCreated event with a known Id.</summary>
    internal static Account Reconstitute(
        Guid id,
        AccountType type,
        Currency currency,
        DateTime createdAt
    ) =>
        new()
        {
            Id = id,
            Type = type,
            Status = AccountStatus.Active,
            Currency = currency,
            CreatedAt = createdAt,
        };

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

    public void Freeze()
    {
        if (Status == AccountStatus.Closed)
            throw new InvalidAggregateOperationException("Cannot freeze a closed account");
        Status = AccountStatus.Frozen;
    }

    public void Unfreeze()
    {
        if (Status == AccountStatus.Closed)
            throw new InvalidAggregateOperationException("Cannot unfreeze a closed account");
        Status = AccountStatus.Active;
    }

    public void Close()
    {
        if (Status == AccountStatus.Closed)
            throw new InvalidAggregateOperationException("Account is already closed");
        Status = AccountStatus.Closed;
    }

    /*
     |--------------------------------------------------------------------------------
     | Holders
     |--------------------------------------------------------------------------------
     */

    public AccountHolder AddHolder(Guid holderId, AccountHolderType holderType)
    {
        if (_accountHolders.Any(h => h.HolderId == holderId))
            throw new AggregateConflictException($"Holder {holderId} is already on account {Id}");

        var holder = new AccountHolder
        {
            Id = Guid.NewGuid(),
            AccountId = Id,
            HolderId = holderId,
            HolderType = holderType,
        };
        _accountHolders.Add(holder);
        return holder;
    }

    public void RemoveHolder(Guid holderId)
    {
        var holder =
            _accountHolders.FirstOrDefault(h => h.Id == holderId)
            ?? throw new AggregateNotFoundException($"Holder {holderId} not found on account {Id}");
        _accountHolders.Remove(holder);
    }
}

/*
 |--------------------------------------------------------------------------------
 | Account Holder
 |--------------------------------------------------------------------------------
 */

public class AccountHolder
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid HolderId { get; init; } // external reference — could be a User, Business, anything
    public AccountHolderType HolderType { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

/*
 |--------------------------------------------------------------------------------
 | Enums
 |--------------------------------------------------------------------------------
 */

public enum AccountType
{
    Checking,
    Savings,
    Loan,
    Investment,
}

public enum AccountStatus
{
    Active,
    Frozen,
    Closed,
}

public enum AccountHolderType
{
    Primary,
    Beneficiary,
    Guardian,
    PowerOfAttorney,
    Custodian,
    Operating,
    Trust,
    Escrow,
    Investment,
    Payroll,
}
