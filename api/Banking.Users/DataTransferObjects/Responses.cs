using System.ComponentModel.DataAnnotations;
using Banking.Shared.ValueObjects;

namespace Banking.Users.DTO.Responses;

public record UserResponse
{
    public required Guid Id { get; init; }
    public required Name Name { get; init; }
    public required DateTime DateOfBirth { get; init; }

    [Required]
    public ICollection<AddressResponse> Addresses { get; init; } = new List<AddressResponse>();

    [Required]
    public ICollection<EmailResponse> Emails { get; init; } = new List<EmailResponse>();
    public required DateTime CreatedAt { get; init; }
}

public record AddressResponse
{
    public required Guid Id { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; }
    public string? Region { get; init; }
}

public record EmailResponse
{
    public required Guid Id { get; init; }
    public required string Address { get; init; }
    public required EmailType Type { get; init; }
}