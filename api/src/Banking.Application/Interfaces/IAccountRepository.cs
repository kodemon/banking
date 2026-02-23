using Banking.Domain.Accounts;

namespace Banking.Application.Interfaces;

public interface IAccountRepository
{
    Task AddAsync(Account account);

    Task<Account?> GetByIdAsync(Guid id);
    Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId);
    Task<IEnumerable<Account>> GetAllByBusinessAsync(Guid businessId);

    Task DeleteAsync(Account account);

    Task SaveChangesAsync();
}