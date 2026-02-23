using System.Text.RegularExpressions;
using Banking.Shared.Exceptions;

namespace Banking.Shared.ValueObjects;

public record Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(250)
    );

    public string Address { get; }
    public EmailType Type { get; }

    public Email(string address, EmailType type)
    {
        Validate(address);
        Address = address.ToLowerInvariant();
        Type = type;
    }

    private static void Validate(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new DomainValidationException("Email address cannot be empty");
        }

        if (address.Length > 254)
        {
            throw new DomainValidationException("Email address cannot exceed 254 characters");
        }

        var parts = address.Split('@');
        if (parts.Length != 2)
        {
            throw new DomainValidationException("Email address must contain exactly one @ symbol");
        }

        if (parts[0].Length > 64)
        {
            throw new DomainValidationException("Email local part cannot exceed 64 characters");
        }

        if (!EmailRegex.IsMatch(address))
        {
            throw new DomainValidationException($"'{address}' is not a valid email address");
        }
    }
}

public enum EmailType
{
    Primary,
    Work,
    Personal
}