using Banking.Application.DTOs.Users;
using Banking.Application.Interfaces;
using Banking.Domain.Exceptions;
using Banking.Domain.Identity;
using Banking.Domain.ValueObjects;
using Banking.Application.Mappings;

namespace Banking.Application.Services;

public class UserService(IUserRepository userRepository)
{
    /*
     |--------------------------------------------------------------------------------
     | User
     |--------------------------------------------------------------------------------
     */

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new AggregateConflictException($"Email '{request.Email}' is already registered");
        }

        var user = new User(
            request.Name,
            request.DateOfBirth
        );

        user.AddEmail(new Email(request.Email, EmailType.Primary));

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return user.ToResponse();
    }

    public async Task<UserResponse> GetUserByIdAsync(Guid userId)
    {
        var user = await GetUser(userId);
        return user.ToResponse();
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(u => u.ToResponse());
    }

    public async Task<UserResponse> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await GetUser(userId);
        if (request.Name is not null)
        {
            user.SetName(request.Name.Family, request.Name.Given);
        }
        await userRepository.SaveChangesAsync();
        return user.ToResponse();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await GetUser(userId);
        await userRepository.DeleteAsync(user);
        await userRepository.SaveChangesAsync();
    }

    /*
     |--------------------------------------------------------------------------------
     | Emails
     |--------------------------------------------------------------------------------
     */

    public async Task<EmailResponse> AddEmailAsync(Guid userId, AddEmailRequest request)
    {
        var user = await GetUser(userId);
        var email = user.AddEmail(new Email(request.Address, request.Type));
        await userRepository.SaveChangesAsync();
        return email.ToResponse();
    }

    public async Task RemoveEmailAsync(Guid userId, Guid emailId)
    {
        var user = await GetUser(userId);
        user.RemoveEmail(emailId);
        await userRepository.SaveChangesAsync();
    }

    /*
     |--------------------------------------------------------------------------------
     | Addresses
     |--------------------------------------------------------------------------------
     */

    public async Task<AddressResponse> AddAddressAsync(Guid userId, AddAddressRequest request)
    {
        var user = await GetUser(userId);
        var address = user.AddAddress(new Address(
            request.Street,
            request.City,
            request.PostalCode,
            request.Country,
            request.Region
        ));
        await userRepository.SaveChangesAsync();
        return address.ToResponse();
    }

    public async Task RemoveAddressAsync(Guid userId, Guid addressId)
    {
        var user = await GetUser(userId);
        user.RemoveAddress(addressId);
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
            throw new AggregateNotFoundException($"User {userId} not found");
        }
        return user;
    }
}