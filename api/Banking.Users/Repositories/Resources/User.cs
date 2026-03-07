using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Users.Repositories.Resources;

internal class User
{
    public Guid Id { get; init; }

    public Name Name { get; private set; }
    public DateTime DateOfBirth { get; init; }

    public List<UserAddress> Addresses = new();
    public List<UserEmail> Emails = new();

    public DateTime CreatedAt { get; init; }

    private User()
    {
        Name = null!;
    }

    public User(Name name, DateTime dob)
    {
        Id = Guid.NewGuid();
        Name = name;
        DateOfBirth = dob;
        CreatedAt = DateTime.UtcNow;
    }

    /*
     |--------------------------------------------------------------------------------
     | Emails
     |--------------------------------------------------------------------------------
     */

    public UserEmail AddEmail(Email email)
    {
        if (Emails.Any(e => e.Email.Address == email.Address))
        {
            throw new AggregateConflictException($"Email '{email.Address}' is already registered");
        }
        var userEmail = new UserEmail
        {
            Id = Guid.NewGuid(),
            UserId = Id,
            Email = email,
        };
        Emails.Add(userEmail);
        return userEmail;
    }

    public void RemoveEmail(Guid emailId)
    {
        var email = Emails.FirstOrDefault(e => e.Id == emailId);
        if (email is null)
        {
            throw new AggregateDeletedException($"Email {emailId} not found for user {Id}");
        }
        Emails.Remove(email);
    }

    /*
     |--------------------------------------------------------------------------------
     | Addresses
     |--------------------------------------------------------------------------------
     */

    public UserAddress AddAddress(Address address)
    {
        var userAddress = new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = Id,
            Address = address,
        };
        Addresses.Add(userAddress);
        return userAddress;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = Addresses.FirstOrDefault(a => a.Id == addressId);
        if (address is null)
        {
            throw new AggregateDeletedException($"Address {addressId} not found for user {Id}");
        }
        Addresses.Remove(address);
    }
}
