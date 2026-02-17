using Banking.Domain.Identity;

namespace Banking.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);

    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();

    Task<bool> ExistsByEmailAsync(string emailAddress);

    Task DeleteAsync(User user);

    Task SaveChangesAsync();
}