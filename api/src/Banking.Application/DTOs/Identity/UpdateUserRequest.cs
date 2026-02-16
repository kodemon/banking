namespace Banking.Application.DTOs.Identity;

public record UpdateUserRequest
{
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
}