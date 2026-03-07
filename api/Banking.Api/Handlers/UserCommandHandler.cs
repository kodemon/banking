using Banking.Shared.ValueObjects;
using Banking.Users.Events;

namespace Banking.Users.Handlers;

/*
 |--------------------------------------------------------------------------------
 | UserEventHandlers
 |--------------------------------------------------------------------------------
 |
 | Reacts to user domain events published by Banking.Api command handlers.
 | Wolverine discovers these by convention — the class is internal, the
 | Handle methods are public, and the first argument is the message type.
 |
 | These handlers only write read models. No knowledge of any other domain.
 |
 | AutoApplyTransactions() in Program.cs commits UsersDbContext after each
 | handler returns — no SaveChangesAsync() needed here.
 |
 */

internal class UserEventHandlers(IUserRepository repository)
{
    public async Task Handle(UserCreated evt)
    {
        var user = User.Reconstitute(
            id: evt.UserId,
            name: new Name(evt.Name.Family, evt.Name.Given),
            dob: evt.DateOfBirth,
            createdAt: evt.OccurredAt
        );

        user.AddEmail(new Email(evt.Email, EmailType.Primary));
        await repository.AddAsync(user);
    }

    public async Task Handle(UserNameUpdated evt)
    {
        var user =
            await repository.GetByIdAsync(evt.UserId)
            ?? throw new InvalidOperationException(
                $"User {evt.UserId} not found for UserNameUpdated"
            );

        user.SetName(evt.Name.Given, evt.Name.Family);
    }

    public async Task Handle(UserDeleted evt)
    {
        var user =
            await repository.GetByIdAsync(evt.UserId)
            ?? throw new InvalidOperationException($"User {evt.UserId} not found for UserDeleted");

        await repository.DeleteAsync(user);
    }

    public async Task Handle(UserEmailAdded evt)
    {
        var user =
            await repository.GetByIdAsync(evt.UserId)
            ?? throw new InvalidOperationException(
                $"User {evt.UserId} not found for UserEmailAdded"
            );

        user.AddEmail(new Email(evt.Address, evt.Type));
    }

    public async Task Handle(UserEmailRemoved evt)
    {
        var user =
            await repository.GetByIdAsync(evt.UserId)
            ?? throw new InvalidOperationException(
                $"User {evt.UserId} not found for UserEmailRemoved"
            );

        user.RemoveEmail(evt.EmailId);
    }

    public async Task Handle(UserAddressAdded evt)
    {
        var user =
            await repository.GetByIdAsync(evt.UserId)
            ?? throw new InvalidOperationException(
                $"User {evt.UserId} not found for UserAddressAdded"
            );

        user.AddAddress(new Address(evt.Street, evt.City, evt.PostalCode, evt.Country, evt.Region));
    }

    public async Task Handle(UserAddressRemoved evt)
    {
        var user =
            await repository.GetByIdAsync(evt.UserId)
            ?? throw new InvalidOperationException(
                $"User {evt.UserId} not found for UserAddressRemoved"
            );

        user.RemoveAddress(evt.AddressId);
    }
}
