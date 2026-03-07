using Banking.Shared.ValueObjects;
using Banking.Users.Events;

namespace Banking.Api.Commands;

/*
 |--------------------------------------------------------------------------------
 | User Commands
 |--------------------------------------------------------------------------------
 |
 | Commands are the ingress contract for Banking.Api controllers. They live
 | here — not in Banking.Shared — because only Banking.Api ever issues or
 | handles them. Each command maps to one or more domain events that a
 | Wolverine handler in Banking.Api emits and awaits.
 |
 | Single-domain commands are handled by a simple command handler that emits
 | one event. Multi-domain commands start a saga (see SagaCommands.cs).
 |
 */

/// <summary>Handled by UserCommandHandler — emits <see cref="UserCreated"/>.</summary>
public record CreateUserCommand(
    Guid CorrelationId,
    NameInput Name,
    DateTime DateOfBirth,
    string Email
);

/// <summary>Handled by UserCommandHandler — emits <see cref="UserNameUpdated"/>.</summary>
public record UpdateUserNameCommand(Guid CorrelationId, Guid UserId, NameInput Name);

/// <summary>Handled by UserCommandHandler — emits <see cref="UserDeleted"/>.</summary>
public record DeleteUserCommand(Guid CorrelationId, Guid UserId);

/// <summary>Handled by UserCommandHandler — emits <see cref="UserEmailAdded"/>.</summary>
public record AddUserEmailCommand(Guid CorrelationId, Guid UserId, string Address, EmailType Type);

/// <summary>Handled by UserCommandHandler — emits <see cref="UserEmailRemoved"/>.</summary>
public record RemoveUserEmailCommand(Guid CorrelationId, Guid UserId, Guid EmailId);

/// <summary>Handled by UserCommandHandler — emits <see cref="UserAddressAdded"/>.</summary>
public record AddUserAddressCommand(
    Guid CorrelationId,
    Guid UserId,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? Region
);

/// <summary>Handled by UserCommandHandler — emits <see cref="UserAddressRemoved"/>.</summary>
public record RemoveUserAddressCommand(Guid CorrelationId, Guid UserId, Guid AddressId);
