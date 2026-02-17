using Banking.Domain.Accounts;
using Banking.Domain.Exceptions;
using Banking.Domain.ValueObjects;

namespace Banking.Domain.Identity;

/*
 |--------------------------------------------------------------------------------
 | Business [Aggregate Root]
 |--------------------------------------------------------------------------------
 */

public class Business
{
    public Guid Id { get; init; }

    public string Name { get; private set; } = string.Empty;
    public string OrganizationNumber { get; init; }

    private readonly List<BusinessAccountHolder> _holdings = new();
    public IReadOnlyCollection<BusinessAccountHolder> AccountHoldings => _holdings.AsReadOnly();

    private readonly List<BusinessAddress> _addresses = new();
    public IReadOnlyCollection<BusinessAddress> Addresses => _addresses.AsReadOnly();

    private readonly List<BusinessEmail> _emails = new();
    public IReadOnlyCollection<BusinessEmail> Emails => _emails.AsReadOnly();

    private readonly List<BusinessPhone> _phones = new();
    public IReadOnlyCollection<BusinessPhone> Phones => _phones.AsReadOnly();

    public DateTime CreatedAt { get; init; }

    public Business(string name, string organizationNumber)
    {
        Id = Guid.NewGuid();
        SetName(name);
        OrganizationNumber = organizationNumber;
        CreatedAt = DateTime.UtcNow;
    }

    /*
     |--------------------------------------------------------------------------------
     | Name
     |--------------------------------------------------------------------------------
     */

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Business name cannot be empty");
        }
        Name = name;
    }
}

/*
 |--------------------------------------------------------------------------------
 | Aggregates
 |--------------------------------------------------------------------------------
 */

public class BusinessAddress
{
    public Guid Id { get; init; }
    public Guid BusinessId { get; init; }

    public required Address Address { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public class BusinessEmail
{
    public Guid Id { get; init; }
    public Guid BusinessId { get; init; }

    public required Email Email { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public class BusinessPhone
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; init; }

    public required string CountryCode { get; set; }
    public required string Number { get; set; }
}
