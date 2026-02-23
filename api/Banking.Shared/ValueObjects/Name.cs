using Banking.Shared.Exceptions;

namespace Banking.Shared.ValueObjects;

public record Name
{
    public string Family { get; }
    public string Given { get; }

    public Name(string family, string given)
    {
        Validate(family, given);
        Family = family;
        Given = given;
    }

    public string Full => $"{Given} {Family}";

    public Name WithGiven(string given) => new(Family, given);
    public Name WithFamily(string family) => new(family, Given);

    private static void Validate(string family, string given)
    {
        if (string.IsNullOrWhiteSpace(given))
        {
            throw new DomainValidationException("Given name cannot be empty");
        }
        if (string.IsNullOrWhiteSpace(family))
        {
            throw new DomainValidationException("Family name cannot be empty");
        }
    }
}