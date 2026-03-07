using Banking.Users.Repositories.Resources;

namespace Banking.Users.Interfaces;

internal interface IUserRepository
{
    Task AddAsync(User user);

    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();

    Task<bool> ExistsByEmailAsync(string emailAddress);

    Task DeleteAsync(User user);

    Task SaveChangesAsync();
}
