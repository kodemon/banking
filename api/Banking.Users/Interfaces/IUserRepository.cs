using Banking.Users.Repositories.Resources;

namespace Banking.Users.Interfaces;

internal interface IUserRepository
{
    Task AddAsync(User user);

    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByOwnerIdAsync(Guid ownerId);
    Task<User?> GetByEmailAsync(string emailAddress);
    Task<IEnumerable<User>> GetAllAsync();

    Task DeleteAsync(User user);

    Task SaveChangesAsync();
}
