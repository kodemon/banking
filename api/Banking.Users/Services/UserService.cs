using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;

namespace Banking.Users;

internal class UserService(IUserRepository userRepository)
{
    /*
     |--------------------------------------------------------------------------------
     | Create
     |--------------------------------------------------------------------------------
     */

    public async Task<User> CreateUserAsync(string email, Name name, DateTime dateOfBirth)
    {
        if (await userRepository.ExistsByEmailAsync(email))
        {
            throw new AggregateConflictException($"Email '{email}' is already registered");
        }

        var user = new User(name, dateOfBirth);

        user.AddEmail(new Email(email, EmailType.Primary));

        await userRepository.AddAsync(user);

        await userRepository.SaveChangesAsync();

        return user;
    }

    /*
     |--------------------------------------------------------------------------------
     | Read
     |--------------------------------------------------------------------------------
     */

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await GetUser(userId);
    }

    /*
     |--------------------------------------------------------------------------------
     | Update
     |--------------------------------------------------------------------------------
     */

    public async Task<User> AddEmailAsync(Guid userId, Email email)
    {
        var user = await GetUser(userId);

        user.AddEmail(email);

        await userRepository.SaveChangesAsync();

        return user;
    }

    public async Task<User> AddAddressAsync(Guid userId, Address address)
    {
        var user = await GetUser(userId);

        user.AddAddress(address);

        await userRepository.SaveChangesAsync();

        return user;
    }

    /*
     |--------------------------------------------------------------------------------
     | Delete
     |--------------------------------------------------------------------------------
     */

    public async Task RemoveEmailAsync(Guid userId, Guid emailId)
    {
        var user = await GetUser(userId);
        user.RemoveEmail(emailId);
        await userRepository.SaveChangesAsync();
    }

    public async Task RemoveAddressAsync(Guid userId, Guid addressId)
    {
        var user = await GetUser(userId);
        user.RemoveAddress(addressId);
        await userRepository.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await GetUser(userId);
        await userRepository.DeleteAsync(user);
        await userRepository.SaveChangesAsync();
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers
     |--------------------------------------------------------------------------------
     */

    private async Task<User> GetUser(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new ResourceNotFoundException($"User {userId} not found");
        }
        return user;
    }
}
