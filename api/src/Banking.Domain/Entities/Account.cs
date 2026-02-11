using Banking.Domain.Enums;
using Banking.Domain.ValueObjects;

namespace Banking.Domain.Entities;

/*
    Account

    Account represents an entity that holds money value calculated through JournalEntry movements for
    Transactions.
    
    # Holders

    An Account can belong to one or many account holders representing shared control
    for representative purposes. For a more robust production solution we would created something akin
    to AccountOwnership connection between an Account and its AccountHolders to control level of
    access each holder has to the Account, such as who can transact on the account, what type of
    ownership, when the holder was added etc.

    # Currency

    An account can only transact in its designated currency and is defined on account creation.
    Currency cannot be changed after creation as the JournalEntries has to related the correct
    value for each Account. When moving money in different currencies another layer of currency
    conversion has to be considered, which is beyond the scope of this project but is something
    to consider when creating a complete banking product.
 */

public class Account
{
    public Guid Id { get; set; }

    public ICollection<AccountHolder> Holders { get; set; } = new List<AccountHolder>();

    public required AccountType Type { get; init; }
    public required AccountStatus Status { get; set; }
    public required Currency Currency { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
