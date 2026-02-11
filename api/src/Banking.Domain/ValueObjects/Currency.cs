namespace Banking.Domain.ValueObjects;

public class Currency : IEquatable<Currency>
{
    public string Code { get; private init; }

    private Currency(string code)
    {
        Code = code;
    }

    public static Currency NOK => new("NOK");
    public static Currency USD => new("USD");
    public static Currency EUR => new("EUR");
    public static Currency GBP => new("GBP");
    public static Currency SEK => new("SEK");
    public static Currency DKK => new("DKK");

    public static Currency FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Currency code cannot be empty", nameof(code));
        }

        var upperCode = code.ToUpperInvariant();

        if (upperCode.Length != 3) // TODO: Check against ISO 4217 codes
        {
            throw new ArgumentException($"Invalid currency code: {code}. Must be 3 characters.", nameof(code));
        }

        return new Currency(upperCode);
    }

    public bool Equals(Currency? other)
    {
        if (other is null)
        {
            return false;
        }
        return Code == other.Code;
    }

    public override bool Equals(object? obj) => Equals(obj as Currency);

    public override int GetHashCode() => Code.GetHashCode();

    public static bool operator ==(Currency? left, Currency? right)
    {
        if (left is null)
        {
            return right is null;
        }
        return left.Equals(right);
    }

    public static bool operator !=(Currency? left, Currency? right) => !(left == right);

    public override string ToString() => Code;
}
