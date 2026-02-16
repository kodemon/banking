namespace Banking.Domain.Identity;

public class UserEmail
{
    public Guid Id { get; set; }
    public Guid UserId { get; init; }

    public required string Address { get; set; }
}
