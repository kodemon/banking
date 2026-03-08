using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Users.Repositories.Resources;

internal class User
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }

    public Name Name { get; private set; }
    public DateTime DateOfBirth { get; init; }

    public List<UserAddress> Addresses = new();
    public List<UserEmail> Emails = new();

    public DateTime CreatedAt { get; init; }

    private User()
    {
        Name = null!;
    }

    public User(Guid ownerId, Name name, DateTime dob)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Name = name;
        DateOfBirth = dob;
        CreatedAt = DateTime.UtcNow;
    }

    /*
     |--------------------------------------------------------------------------------
     | Emails
     |--------------------------------------------------------------------------------
     */

    public bool HasEmail(Email email) => Emails.Any(e => e.Email.Address == email.Address);

    public bool HasEmail(string address) => Emails.Any(e => e.Email.Address == address);

    public UserEmail AddEmail(Email email)
    {
        var userEmail = GetEmail(email);
        if (userEmail is not null)
        {
            return userEmail;
        }
        userEmail = new UserEmail(Id, email);
        Emails.Add(userEmail);
        return userEmail;
    }

    public UserEmail? GetEmail(Email email) => Emails.Find(e => e.Email.Address == email.Address);

    public UserEmail? GetEmail(string address) => Emails.Find(e => e.Email.Address == address);

    public void RemoveEmail(UserEmail email)
    {
        Emails.Remove(email);
    }

    /*
     |--------------------------------------------------------------------------------
     | Addresses
     |--------------------------------------------------------------------------------
     */

    public UserAddress AddAddress(Address address)
    {
        var userAddress = new UserAddress(Id, address);
        Addresses.Add(userAddress);
        return userAddress;
    }

    public void RemoveAddress(UserAddress address)
    {
        Addresses.Remove(address);
    }
}
