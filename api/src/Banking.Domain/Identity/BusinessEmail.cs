namespace Banking.Domain.Identity;

public class BusinessEmail
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; init; }

    public required string Address { get; set; }
}
