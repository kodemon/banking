using Banking.Api.Commands;
using Banking.Events;
using Banking.Users.Events;
using Wolverine;

namespace Banking.Api.Handlers;

/*
 |--------------------------------------------------------------------------------
 | RegisterUserCommandHandler
 |--------------------------------------------------------------------------------
 |
 | Orchestrates user registration for an identity that already has a principal.
 |
 | Steps (sequential, inline):
 |   1. CreateUserCommand         → UserCreated        → Banking.Users writes user record
 |   2. SetPrincipalAttributeCommand → PrincipalAttributeSet → links user_id to principal
 |   3. AddPrincipalRoleCommand   → PrincipalRoleAdded → assigns "user" role
 |
 | PrincipalId comes from the caller (IPrincipalContext) — the principal was
 | created by PrincipalProvisioner on first login, not here.
 |
 */

public class RegisterUserCommandHandler(IEventStore eventStore, IMessageBus bus)
{
    public async Task Handle(RegisterUserCommand cmd)
    {
        // Step 1 — create user record; mint UserId here so we can carry it forward
        var userId = Guid.NewGuid();

        var userCreated = new UserCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            UserId: userId,
            Name: cmd.Name,
            DateOfBirth: cmd.DateOfBirth,
            Email: cmd.Email
        );

        await eventStore.AppendAsync(userCreated, streamId: userId);
        await bus.InvokeAsync(userCreated);

        // Step 2 — link user_id onto the existing principal
        await bus.InvokeAsync(
            new SetPrincipalAttributeCommand(
                CorrelationId: cmd.CorrelationId,
                PrincipalId: cmd.PrincipalId,
                Domain: "user",
                Key: "user_id",
                Value: userId.ToString()
            )
        );

        // Step 3 — assign "user" role to the principal
        await bus.InvokeAsync(
            new AddPrincipalRoleCommand(
                CorrelationId: cmd.CorrelationId,
                PrincipalId: cmd.PrincipalId,
                Role: "user"
            )
        );
    }
}
