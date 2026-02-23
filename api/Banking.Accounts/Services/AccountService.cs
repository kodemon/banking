using Banking.Accounts.DTO.Requests;
using Banking.Accounts.DTO.Responses;
using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Transactions;

namespace Banking.Accounts;

internal class AccountService(IAccountRepository accountRepository, IBalanceService balanceService)
{
    /*
     |--------------------------------------------------------------------------------
     | Account
     |--------------------------------------------------------------------------------
     */

    public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request)
    {
        var account = new Account(
            request.Type,
            Currency.FromCode(request.CurrencyCode)
        );

        account.AddHolder(request.HolderId, request.HolderType);

        await accountRepository.AddAsync(account);
        await accountRepository.SaveChangesAsync();

        return account.ToResponse(0);
    }

    public async Task<AccountResponse> GetAccountByIdAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);
        var balance = await balanceService.GetBalanceAsync(accountId);
        return account.ToResponse(balance);
    }

    public async Task<IEnumerable<AccountResponse>> GetAccountsByHolderAsync(Guid holderId)
    {
        var accounts = await accountRepository.GetAllByHolderAsync(holderId);
        return await ToResponsesAsync(accounts);
    }

    public async Task<AccountResponse> FreezeAccountAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);
        account.Freeze();
        await accountRepository.SaveChangesAsync();
        var balance = await balanceService.GetBalanceAsync(accountId);
        return account.ToResponse(balance);
    }

    public async Task<AccountResponse> UnfreezeAccountAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);
        account.Unfreeze();
        await accountRepository.SaveChangesAsync();
        var balance = await balanceService.GetBalanceAsync(accountId);
        return account.ToResponse(balance);
    }

    public async Task CloseAccountAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);
        account.Close();
        await accountRepository.SaveChangesAsync();
    }

    /*
     |--------------------------------------------------------------------------------
     | Personal Holders
     |--------------------------------------------------------------------------------
     */

    public async Task<AccountHolderResponse> AddAccountHolderAsync(Guid accountId, AddAccountHolderRequest request)
    {
        var account = await GetAccount(accountId);
        var holder = account.AddHolder(request.HolderId, request.HolderType);
        await accountRepository.SaveChangesAsync();
        return holder.ToResponse();
    }

    public async Task RemovePersonalHolderAsync(Guid accountId, Guid holderId)
    {
        var account = await GetAccount(accountId);
        account.RemoveHolder(holderId);
        await accountRepository.SaveChangesAsync();
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
            throw new AggregateNotFoundException($"Account {accountId} not found");
        return account;
    }

    private async Task<IEnumerable<AccountResponse>> ToResponsesAsync(IEnumerable<Account> accounts)
    {
        var responses = new List<AccountResponse>();
        foreach (var account in accounts)
        {
            var balance = await balanceService.GetBalanceAsync(account.Id);
            responses.Add(account.ToResponse(balance));
        }
        return responses;
    }
}