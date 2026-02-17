using Banking.Domain.ValueObjects;

namespace Banking.Application.DTOs.Users;

public record UpdateUserRequest
{
    public Name? Name { get; init; }
}