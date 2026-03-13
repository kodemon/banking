public record AccountNumber
{
    public string Value { get; }

    public AccountNumber(string value)
    {
        if (IsValid(value) == false)
        {
            throw new ArgumentException($"Invalid account number: {value}");
        }
        Value = value;
    }

    public static AccountNumber Generate() =>
        new(Random.Shared.NextInt64(10_000_000_000, 99_999_999_999).ToString());

    private static bool IsValid(string value) =>
        !string.IsNullOrWhiteSpace(value) && value.Length == 11 && value.All(char.IsDigit);

    public override string ToString() => $"{Value[..4]} {Value[4..6]} {Value[6..]}";
}
