using Banking.Domain.Identity;

namespace Banking.Application.Interfaces;

/*
 |--------------------------------------------------------------------------------
 | Repository
 |--------------------------------------------------------------------------------
 */

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<bool> ExistsByEmailAsync(string emailAddress);
    Task AddAsync(User user);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
}