using Banking.Shared.ValueObjects;

namespace Banking.Users.Repositories.Resources;

internal class UserAddress
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public Address Address { get; init; }

    public DateTime CreatedAt { get; init; }

    private UserAddress()
    {
        Address = null!;
    }

    public UserAddress(Guid userId, Address address)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Address = address;
        CreatedAt = DateTime.UtcNow;
    }
}
