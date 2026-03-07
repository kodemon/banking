using Banking.Accounts.DTO.Responses;
using Banking.Shared.Exceptions;
using Banking.Transactions;

namespace Banking.Accounts;

/*
 |--------------------------------------------------------------------------------
 | AccountService  [Query Only]
 |--------------------------------------------------------------------------------
 |
 | Read-only query surface for Banking.Api controllers. All mutations are
 | driven by events handled in Banking.Accounts.Handlers.AccountEventHandlers.
 |
 */

internal class AccountService(IAccountRepository accountRepository, IBalanceService balanceService)
{
    public async Task<AccountResponse> GetAccountByIdAsync(Guid accountId)
    {
        var account = await GetAccount(accountId);
        var balance = await balanceService.GetBalanceAsync(accountId);
        return account.ToResponse(balance);
    }

    public async Task<IEnumerable<AccountResponse>> GetAccountsByHolderAsync(Guid holderId)
    {
        var accounts = await accountRepository.GetAllByHolderAsync(holderId);
        var responses = new List<AccountResponse>();
        foreach (var account in accounts)
        {
            var balance = await balanceService.GetBalanceAsync(account.Id);
            responses.Add(account.ToResponse(balance));
        }
        return responses;
    }

    private async Task<Account> GetAccount(Guid accountId)
    {
        return await accountRepository.GetByIdAsync(accountId)
            ?? throw new AggregateNotFoundException($"Account {accountId} not found");
    }
}
