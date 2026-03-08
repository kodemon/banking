using Banking.Accounts.Enums;
using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories.Resources;
using Banking.Shared.ValueObjects;
using MediatR;

namespace Banking.Accounts.Commands;

internal record CreateAccountCommand(
    string accountName,
    AccountType accountType,
    Currency currency,
    Guid holderId,
    AccountHolderType holderType
) : IRequest<Account>;

internal class CreateAccountHandler(IAccountRepository repository)
    : IRequestHandler<CreateAccountCommand, Account>
{
    public async Task<Account> Handle(CreateAccountCommand message, CancellationToken token)
    {
        var account = new Account(message.accountName, message.accountType, message.currency);

        account.AddHolder(message.holderId, message.holderType);

        await repository.AddAsync(account);
        await repository.SaveChangesAsync();

        return account;
    }
}
