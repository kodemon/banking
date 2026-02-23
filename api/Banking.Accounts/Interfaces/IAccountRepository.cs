namespace Banking.Accounts;

internal interface IAccountRepository
{
    Task AddAsync(Account account);

    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId);
    Task<IEnumerable<Account>> GetAllByBusinessAsync(Guid businessId);

    Task DeleteAsync(Account account);

    Task SaveChangesAsync();
}