using Banking.Shared.ValueObjects;

namespace Banking.Users.DTO.Requests;

internal record CreateUserRequest
{
    public required Name Name { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Email { get; init; }
}

internal record UpdateUserRequest
{
    public Name? Name { get; init; }
}

internal record AddEmailRequest
{
    public required string Address { get; init; }
    public required EmailType Type { get; init; }
}

internal record AddAddressRequest
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; }
    public string? Region { get; init; }
}
