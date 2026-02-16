namespace Banking.Application.DTOs.Identity;

public record AddEmailRequest
{
    public required string Address { get; init; }
}