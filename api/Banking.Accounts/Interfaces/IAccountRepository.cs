namespace Banking.Accounts;

public interface IAccountRepository
{
    Task AddAsync(Account account);

    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllByHolderAsync(Guid holderId);

    Task DeleteAsync(Account account);

    Task SaveChangesAsync();
}
