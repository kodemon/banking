using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Accounts;

/*
 |--------------------------------------------------------------------------------
 | Account [Aggregate Root]
 |--------------------------------------------------------------------------------
 */

internal class Account
{
    public Guid Id { get; init; }

    public AccountType Type { get; init; }
    public AccountStatus Status { get; set; }
    public Currency Currency { get; init; } = null!;

    private readonly List<PersonalAccountHolder> _personalHolders = new();
    public IReadOnlyCollection<PersonalAccountHolder> PersonalHolders => _personalHolders.AsReadOnly();

    private readonly List<BusinessAccountHolder> _businessHolders = new();
    public IReadOnlyCollection<BusinessAccountHolder> BusinessHolders => _businessHolders.AsReadOnly();

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

    public PersonalAccountHolder AddPersonalHolder(Guid userId, PersonalHolderType holderType)
    {
        if (_personalHolders.Any(h => h.UserId == userId))
            throw new AggregateConflictException($"User {userId} is already a holder on account {Id}");

        var holder = new PersonalAccountHolder
        {
            Id = Guid.NewGuid(),
            AccountId = Id,
            UserId = userId,
            HolderType = holderType
        };
        _personalHolders.Add(holder);
        return holder;
    }

    public void RemovePersonalHolder(Guid holderId)
    {
        var holder = _personalHolders.FirstOrDefault(h => h.Id == holderId)
            ?? throw new AggregateNotFoundException($"Personal holder {holderId} not found on account {Id}");
        _personalHolders.Remove(holder);
    }

    public BusinessAccountHolder AddBusinessHolder(Guid businessId, BusinessHolderType holderType)
    {
        if (_businessHolders.Any(h => h.BusinessId == businessId))
            throw new AggregateConflictException($"Business {businessId} is already a holder on account {Id}");

        var holder = new BusinessAccountHolder
        {
            Id = Guid.NewGuid(),
            AccountId = Id,
            BusinessId = businessId,
            HolderType = holderType
        };
        _businessHolders.Add(holder);
        return holder;
    }

    public void RemoveBusinessHolder(Guid holderId)
    {
        var holder = _businessHolders.FirstOrDefault(h => h.Id == holderId)
            ?? throw new AggregateNotFoundException($"Business holder {holderId} not found on account {Id}");
        _businessHolders.Remove(holder);
    }
}

/*
 |--------------------------------------------------------------------------------
 | Account Holders
 |--------------------------------------------------------------------------------
 */

internal class PersonalAccountHolder
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid UserId { get; init; }
    public PersonalHolderType HolderType { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

internal class BusinessAccountHolder
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public Guid BusinessId { get; init; }
    public BusinessHolderType HolderType { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

/*
 |--------------------------------------------------------------------------------
 | Enums
 |--------------------------------------------------------------------------------
 */

internal enum AccountType { Checking, Savings, Loan, Investment }
internal enum AccountStatus { Active, Frozen, Closed }
internal enum PersonalHolderType { Primary, Beneficiary, Guardian, PowerOfAttorney, Custodian }
internal enum BusinessHolderType { Operating, Trust, Escrow, Investment, Payroll }