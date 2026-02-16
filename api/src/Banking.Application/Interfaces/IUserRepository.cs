using Banking.Domain.Identity;

namespace Banking.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> ExistsAsync(Guid id);

    // Email operations
    Task<UserEmail> AddEmailAsync(UserEmail email);
    Task<bool> EmailExistsAsync(string emailAddress);
    Task<UserEmail?> GetEmailAsync(Guid emailId, Guid userId);
    Task DeleteEmailAsync(UserEmail email);
    Task<IEnumerable<UserEmail>> GetUserEmailsAsync(Guid userId);

    // Address operations
    Task<UserAddress> AddAddressAsync(UserAddress address);
    Task<UserAddress?> GetAddressAsync(Guid addressId, Guid userId);
    Task DeleteAddressAsync(UserAddress address);
    Task<IEnumerable<UserAddress>> GetUserAddressesAsync(Guid userId);

    Task SaveChangesAsync();
}