using Banking.Atomic.Libraries;
using Banking.Principals.AccessControl;
using Banking.Principals.Atomics;
using Banking.Principals.Commands;
using Banking.Shared.ValueObjects;
using Banking.Users.Atomics;
using Banking.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
[Authorize]
internal class UsersController(
    Atomic atomic,
    IMediator mediator,
    IPrincipalContext principalContext
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] PostUserPayload payload)
    {
        if (principalContext.IsResolved == false)
        {
            return Forbid();
        }

        var user = await atomic.Run(
            AtomicTask.Create(
                name: UserAtomicTaskNames.CreateUser,
                commit: () =>
                    mediator.Send(
                        new CreateUserCommand(
                            payload.Email,
                            new Name(payload.Name.Family, payload.Name.Given),
                            payload.DateOfBirth
                        )
                    ),
                rollback: (user) => new CreateUserRollback { Id = user.Id }
            )
        );

        await atomic.Run(
            AtomicTask.Create(
                name: PrincipalAtomicTaskNames.AddPrincipalAttribute,
                commit: () =>
                    mediator.Send(
                        new SetPrincipalAttributeCommand(
                            principalContext.Principal.Id,
                            "user",
                            "user_id",
                            user.Id.ToString()
                        )
                    ),
                rollback: (principal) =>
                    new SetAttributeRollback
                    {
                        Id = principal.Id,
                        Domain = "user",
                        Key = "user_id",
                    }
            )
        );

        await atomic.Complete();

        return Ok(user);
    }
}

internal record PostUserPayload
{
    public required string Email { get; init; }
    public required UserName Name { get; init; }
    public required DateTime DateOfBirth { get; init; }
}

internal record UserName
{
    public required string Family { get; init; }
    public required string Given { get; init; }
}
