using Banking.Accounts.Enums;
using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories.Resources;
using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Accounts;

internal class AccountService(IAccountRepository accountRepository)
{
    /*
     |--------------------------------------------------------------------------------
     | Create
     |--------------------------------------------------------------------------------
     */

    public async Task<Account> CreateAccountAsync(
        AccountType type,
        Guid holderId,
        AccountHolderType holderType,
        string currencyCode
    )
    {
        var account = new Account(type, Currency.FromCode(currencyCode));

        account.AddHolder(holderId, holderType);

        await accountRepository.AddAsync(account);
        await accountRepository.SaveChangesAsync();

        return account;
    }

    /*
     |--------------------------------------------------------------------------------
     | Read
     |--------------------------------------------------------------------------------
     */

    public Task<Account> GetAccountByIdAsync(Guid accountId)
    {
        return GetAccount(accountId);
    }

    public Task<IEnumerable<Account>> GetAccountsByHolderAsync(Guid holderId)
    {
        return accountRepository.GetAllByHolderIdAsync(holderId);
    }

    /*
     |--------------------------------------------------------------------------------
     | Update
     |--------------------------------------------------------------------------------
     */

    public async Task<Account> AddAccountHolderAsync(
        Guid accountId,
        Guid holderId,
        AccountHolderType holderType
    )
    {
        var account = await GetAccount(accountId);

        account.AddHolder(holderId, holderType);

        await accountRepository.SaveChangesAsync();

        return account;
    }

    public async Task<Account> UnfreezeAccountIdAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);

        account.Unfreeze();

        await accountRepository.SaveChangesAsync();

        return account;
    }

    /*
     |--------------------------------------------------------------------------------
     | Delete
     |--------------------------------------------------------------------------------
     */

    public async Task<Account> RemovePersonalHolderAsync(Guid accountId, Guid holderId)
    {
        var account = await GetAccount(accountId);

        account.RemoveHolder(holderId);

        await accountRepository.SaveChangesAsync();

        return account;
    }

    public async Task<Account> FreezeAccountIdAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);

        account.Freeze();

        await accountRepository.SaveChangesAsync();

        return account;
    }

    public async Task<Account> CloseAccountIdAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);

        account.Close();

        await accountRepository.SaveChangesAsync();

        return account;
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers
     |--------------------------------------------------------------------------------
     */

    private async Task<Account> GetAccount(Guid accountId)
    {
        var account = await accountRepository.GetByIdAsync(accountId);
        if (account is null)
        {
            throw new ResourceNotFoundException($"Account {accountId} not found");
        }
        return account;
    }
}
