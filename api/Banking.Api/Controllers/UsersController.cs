using Banking.Api.Identity;
using Banking.Principals.Queries;
using Banking.Shared.ValueObjects;
using Banking.Users.Commands;
using Banking.Users.Repositories.Resources;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
[RequireSession]
internal class UsersController(IAuth auth, IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<User>(200)]
    [ProducesResponseType(409)]
    public async Task<ActionResult> Register([FromBody] PostUserPayload payload)
    {
        var user = await mediator.Send(
            new CreateUserCommand(
                auth.Principal.Id,
                payload.Email,
                new Name(payload.Name.Family, payload.Name.Given),
                payload.DateOfBirth
            )
        );
        return Ok(user);
    }

    [HttpGet("me")]
    [ProducesResponseType<User>(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Me()
    {
        var user = await mediator.Send(new GetUserByOwnerIdQuery(auth.Principal.Id));
        if (user is null)
        {
            return NotFound();
        }
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
