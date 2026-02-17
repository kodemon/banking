using Banking.Domain.ValueObjects;

namespace Banking.Application.DTOs.Users;

public record AddEmailRequest
{
    public required string Address { get; init; }
    public required EmailType Type { get; init; }
}