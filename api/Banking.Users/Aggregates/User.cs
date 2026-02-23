using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;

namespace Banking.Users;

/*
 |--------------------------------------------------------------------------------
 | User [Aggregate Root]
 |--------------------------------------------------------------------------------
 */

internal class User
{
    public Guid Id { get; init; }

    public Name Name { get; private set; }
    public DateTime DateOfBirth { get; init; }

    private readonly List<UserAddress> _addresses = new();
    public IReadOnlyCollection<UserAddress> Addresses => _addresses.AsReadOnly();

    private readonly List<UserEmail> _emails = new();
    public IReadOnlyCollection<UserEmail> Emails => _emails.AsReadOnly();

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
     | Name
     |--------------------------------------------------------------------------------
     */

    public void SetName(string given, string family)
    {
        Name = new Name(family, given);
    }

    public void SetGivenName(string given)
    {
        Name = Name.WithGiven(given);
    }

    public void SetFamilyName(string family)
    {
        Name = Name.WithFamily(family);
    }

    /*
     |--------------------------------------------------------------------------------
     | Emails
     |--------------------------------------------------------------------------------
     */

    public UserEmail AddEmail(Email email)
    {
        if (_emails.Any(e => e.Email.Address == email.Address))
        {
            throw new AggregateConflictException($"Email '{email.Address}' is already registered");
        }
        var userEmail = new UserEmail
        {
            Id = Guid.NewGuid(),
            UserId = Id,
            Email = email
        };
        _emails.Add(userEmail);
        return userEmail;
    }

    public void RemoveEmail(Guid emailId)
    {
        var email = _emails.FirstOrDefault(e => e.Id == emailId)
            ?? throw new AggregateDeletedException($"Email {emailId} not found for user {Id}");
        _emails.Remove(email);
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
            Address = address
        };
        _addresses.Add(userAddress);
        return userAddress;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new AggregateDeletedException($"Address {addressId} not found for user {Id}");
        _addresses.Remove(address);
    }
}

/*
 |--------------------------------------------------------------------------------
 | Aggregates
 |--------------------------------------------------------------------------------
 */

internal class UserAddress
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public required Address Address { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

internal class UserEmail
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }

    public required Email Email { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}