namespace Banking.Domain.Identity;

public class BusinessPhone
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; init; }

    public required string CountryCode { get; set; }
    public required string Number { get; set; }
}