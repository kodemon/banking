namespace Banking.Domain.ValueObjects;

public readonly record struct Name(string Family, string Given)
{
    public string Full()
    {
        return $"{Given} {Family}";
    }
}