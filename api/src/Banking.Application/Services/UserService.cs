using Banking.Application.DTOs.Identity;
using Banking.Application.Interfaces;
using Banking.Domain.Identity;
using Banking.Domain.ValueObjects;

namespace Banking.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        // Validate email doesn't already exist
        var emailExists = await _userRepository.EmailExistsAsync(request.Email);
        if (emailExists)
        {
            throw new InvalidOperationException($"Email '{request.Email}' is already registered");
        }

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Add email
        await _userRepository.AddEmailAsync(new UserEmail
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Address = request.Email
        });

        await _userRepository.SaveChangesAsync();

        return await GetUserByIdAsync(user.Id)
            ?? throw new InvalidOperationException("Failed to retrieve created user");
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        var emails = await _userRepository.GetUserEmailsAsync(userId);
        var addresses = await _userRepository.GetUserAddressesAsync(userId);

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
            Addresses = addresses.Select(a => new AddressResponse
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                PostalCode = a.PostalCode,
                Country = a.Country,
                Region = a.Region
            }).ToList(),
            Emails = emails.Select(e => new EmailResponse
            {
                Id = e.Id,
                Address = e.Address
            }).ToList()
        };
    }

    public async Task<UserResponse?> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        // Update name if provided
        if (!string.IsNullOrEmpty(request.GivenName) || !string.IsNullOrEmpty(request.FamilyName))
        {
            var givenName = request.GivenName ?? user.Name.Given;
            var familyName = request.FamilyName ?? user.Name.Family;
            user.Name = new Name(familyName, givenName);
        }

        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        return await GetUserByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        await _userRepository.DeleteAsync(user);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<AddressResponse> AddAddressAsync(Guid userId, AddAddressRequest request)
    {
        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
            throw new InvalidOperationException($"User {userId} not found");

        var address = new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Street = request.Street,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Region = request.Region
        };

        await _userRepository.AddAddressAsync(address);
        await _userRepository.SaveChangesAsync();

        return new AddressResponse
        {
            Id = address.Id,
            Street = address.Street,
            City = address.City,
            PostalCode = address.PostalCode,
            Country = address.Country,
            Region = address.Region
        };
    }

    public async Task<EmailResponse> AddEmailAsync(Guid userId, AddEmailRequest request)
    {
        var userExists = await _userRepository.ExistsAsync(userId);
        if (!userExists)
            throw new InvalidOperationException($"User {userId} not found");

        // Check if email already exists
        var emailExists = await _userRepository.EmailExistsAsync(request.Address);
        if (emailExists)
            throw new InvalidOperationException($"Email '{request.Address}' is already registered");

        var email = new UserEmail
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Address = request.Address
        };

        await _userRepository.AddEmailAsync(email);
        await _userRepository.SaveChangesAsync();

        return new EmailResponse
        {
            Id = email.Id,
            Address = email.Address
        };
    }

    public async Task<bool> RemoveAddressAsync(Guid userId, Guid addressId)
    {
        var address = await _userRepository.GetAddressAsync(addressId, userId);
        if (address == null)
            return false;

        await _userRepository.DeleteAddressAsync(address);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveEmailAsync(Guid userId, Guid emailId)
    {
        var email = await _userRepository.GetEmailAsync(emailId, userId);
        if (email == null)
            return false;

        await _userRepository.DeleteEmailAsync(email);
        await _userRepository.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        var responses = new List<UserResponse>();
        foreach (var user in users)
        {
            var response = await GetUserByIdAsync(user.Id);
            if (response != null)
                responses.Add(response);
        }

        return responses;
    }
}