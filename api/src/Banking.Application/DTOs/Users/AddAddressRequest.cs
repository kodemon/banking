namespace Banking.Application.DTOs.Users;

public record AddAddressRequest
{
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; }
    public string? Region { get; init; }
}