using Banking.Domain.ValueObjects;

namespace Banking.Domain.Entities;

/*
    Account Holder

    Repersents an ownership entity of an account. In our project every AccountHolder assigned to an
    account has full ownership access to it. In a more productionr ready banking system we would want
    a connecting middle layer describing the AccountHolder level of ownership of an account. For this
    project we want to keep things simple as we aim to serve educational level solutions rather than
    a full fledged banking system.

    Instead of creaeting one large AccountHolder we define individual details in separate classes
    and tables. This allows for multiple meta data entries of the same type, and also cleanly
    separates the ability to form different outputs based on separated concepts.

    # Account Type

    Account type is determined by what identities is attached to the holder. If a holder has a single
    IndividualIdentity the account type would be personal, if only BusinessIdentity is present we
    would define it as a business account, if it has multiple IndividualIdentity entries we would
    consider it a joint account, and if lastly if it has a mix of Individual and Bussines identities
    we could consider it a shared account. By not explicitly creating account types we produce more
    flexibility for potential future identity types which we can re-define through new definitions
    based on mixture rather than explicit definitions.
 */

public class AccountHolder
{
    public Guid Id { get; init; }

    public ICollection<Account> Accounts { get; set; } = new List<Account>();

    internal ICollection<AccountHolderIndividualIdentity> IndividualIdentities { get; set; } = new List<AccountHolderIndividualIdentity>();
    internal ICollection<AccountHolderBusinessIdentity> BusinessIdentities { get; set; } = new List<AccountHolderBusinessIdentity>();
    internal ICollection<AccountHolderAddress> Addresses { get; set; } = new List<AccountHolderAddress>();
    internal ICollection<AccountHolderEmail> Emails { get; set; } = new List<AccountHolderEmail>();
    internal ICollection<AccountHolderPhone> Phones { get; set; } = new List<AccountHolderPhone>();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string GetAccountType()
    {
        return AccountHolderType.FromIdentities(IndividualIdentities, BusinessIdentities).ToString();
    }
}

internal class AccountHolderType
{
    public string Value { get; private init; }

    private AccountHolderType(string value) => Value = value;

    public static readonly AccountHolderType Personal = new("Personal");
    public static readonly AccountHolderType Business = new("Business");
    public static readonly AccountHolderType Joint = new("Joint");
    public static readonly AccountHolderType Shared = new("Shared");
    public static readonly AccountHolderType Unknown = new("Unknown");

    public static AccountHolderType FromIdentities(
        ICollection<AccountHolderIndividualIdentity> individuals,
        ICollection<AccountHolderBusinessIdentity> businesses
    )
    {
        var hasIndividual = individuals.Any();
        var hasBusiness = businesses.Any();

        if (hasIndividual && hasBusiness) return Shared;
        if (individuals.Count > 1) return Joint;
        if (hasBusiness) return Business;
        if (hasIndividual) return Personal;

        return Unknown;
    }

    public override string ToString() => Value;
}

internal class AccountHolderIndividualIdentity
{
    public Guid Id { get; init; }
    public Guid AccountHolderId { get; init; }

    public required Name Name { get; set; }
    public required DateTime DateOfBirth { get; init; }
}

internal class AccountHolderBusinessIdentity
{
    public Guid Id { get; init; }
    public Guid AccountHolderId { get; init; }

    public required string Name { get; set; }
    public required string OrganizationNumber { get; init; }

}

internal class AccountHolderAddress
{
    public Guid Id { get; init; }
    public Guid AccountHolderId { get; init; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }
    public required string Country { get; set; }

    public string? Region { get; set; }  // State/Province, optional

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

internal class AccountHolderEmail
{
    public Guid Id { get; set; }
    public Guid AccountHolderId { get; init; }

    public required string Address { get; set; }
}

internal class AccountHolderPhone
{
    public Guid Id { get; set; }
    public Guid AccountHolderId { get; init; }

    public required string CountryCode { get; set; }
    public required string Number { get; set; }
}
