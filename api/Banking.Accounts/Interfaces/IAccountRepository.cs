using Banking.Accounts.Repositories.Resources;

namespace Banking.Accounts.Interfaces;

internal interface IAccountRepository
{
    Task AddAsync(Account account);

    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllByHolderIdAsync(Guid holderId);

    Task DeleteAsync(Account account);

    Task SaveChangesAsync();
}
