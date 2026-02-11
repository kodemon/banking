namespace Banking.Domain.ValueObjects;

public readonly record struct Money(long Amount, string Currency)
{
    public Money Add(Money other)
    {
        ValidateCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        ValidateCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    private void ValidateCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot add/subtract money with different currencies");
        }
    }
}