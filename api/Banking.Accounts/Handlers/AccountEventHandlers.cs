using Banking.Accounts.Events;
using Banking.Accounts.Persistence;
using Banking.Shared.ValueObjects;
using Wolverine.Attributes;

namespace Banking.Accounts.Handlers;

/*
 |--------------------------------------------------------------------------------
 | AccountEventHandlers
 |--------------------------------------------------------------------------------
 |
 | Reacts to account domain events published by Banking.Api command handlers.
 | AccountType and HolderType arrive as strings and are parsed back into
 | the domain's internal enums here — the boundary crossing is explicit.
 | No knowledge of any other domain.
 |
 */

[WolverineHandler]
public class AccountEventHandlers(IAccountRepository repository, AccountsDbContext db)
{
    public async Task Handle(AccountCreated evt)
    {
        var accountType = Enum.Parse<AccountType>(evt.AccountType);
        var holderType = Enum.Parse<AccountHolderType>(evt.PrimaryHolderType);
        var currency = Currency.FromCode(evt.CurrencyCode);

        var account = Account.Reconstitute(evt.AccountId, accountType, currency, evt.OccurredAt);
        account.AddHolder(evt.PrimaryHolderId, holderType);

        await repository.AddAsync(account);
    }

    public async Task Handle(AccountHolderAdded evt)
    {
        var account =
            await repository.GetByIdAsync(evt.AccountId)
            ?? throw new InvalidOperationException(
                $"Account {evt.AccountId} not found for AccountHolderAdded"
            );

        var holderType = Enum.Parse<AccountHolderType>(evt.HolderType);
        account.AddHolder(evt.HolderId, holderType);
    }

    public async Task Handle(AccountHolderRemoved evt)
    {
        var account =
            await repository.GetByIdAsync(evt.AccountId)
            ?? throw new InvalidOperationException(
                $"Account {evt.AccountId} not found for AccountHolderRemoved"
            );

        account.RemoveHolder(evt.HolderId);
    }

    public async Task Handle(AccountFrozen evt)
    {
        var account =
            await repository.GetByIdAsync(evt.AccountId)
            ?? throw new InvalidOperationException(
                $"Account {evt.AccountId} not found for AccountFrozen"
            );

        account.Freeze();
    }

    public async Task Handle(AccountUnfrozen evt)
    {
        var account =
            await repository.GetByIdAsync(evt.AccountId)
            ?? throw new InvalidOperationException(
                $"Account {evt.AccountId} not found for AccountUnfrozen"
            );

        account.Unfreeze();
    }

    public async Task Handle(AccountClosed evt)
    {
        var account =
            await repository.GetByIdAsync(evt.AccountId)
            ?? throw new InvalidOperationException(
                $"Account {evt.AccountId} not found for AccountClosed"
            );

        account.Close();
    }
}
