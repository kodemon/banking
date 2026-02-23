using Banking.Domain.ValueObjects;

namespace Banking.Application.DTOs.Users;

public record CreateUserRequest
{
    public required Name Name { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Email { get; init; }
}