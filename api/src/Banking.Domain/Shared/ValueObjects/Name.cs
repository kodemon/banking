namespace Banking.Domain.ValueObjects;

public record Name(string Family, string Given)
{
    private Name() : this(string.Empty, string.Empty) { }

    public string Full()
    {
        return $"{Given} {Family}";
    }
}