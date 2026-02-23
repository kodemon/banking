using Banking.Users.DTO.Responses;

namespace Banking.Users;

internal static class UserMappings
{
    public static UserResponse ToResponse(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        DateOfBirth = user.DateOfBirth,
        CreatedAt = user.CreatedAt,
        Emails = user.Emails.Select(e => e.ToResponse()).ToList(),
        Addresses = user.Addresses.Select(a => a.ToResponse()).ToList()
    };

    public static EmailResponse ToResponse(this UserEmail email) => new()
    {
        Id = email.Id,
        Address = email.Email.Address,
        Type = email.Email.Type
    };

    public static AddressResponse ToResponse(this UserAddress address) => new()
    {
        Id = address.Id,
        Street = address.Address.Street,
        City = address.Address.City,
        PostalCode = address.Address.PostalCode,
        Country = address.Address.Country,
        Region = address.Address.Region
    };
}