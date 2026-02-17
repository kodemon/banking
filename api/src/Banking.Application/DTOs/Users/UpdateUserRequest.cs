namespace Banking.Application.DTOs.Users;

public record UpdateUserRequest
{
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
}