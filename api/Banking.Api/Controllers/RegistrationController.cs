using Banking.Api.Commands;
using Banking.Principal;
using Banking.Principal.AccessControl;
using Banking.Principal.DTO.Responses;
using Banking.Shared.ValueObjects;
using Banking.Users;
using Banking.Users.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Registration")]
[Authorize]
internal class RegistrationController(
    IMessageBus bus,
    UserService userService,
    PrincipalService principalService,
    IPrincipalContext principalContext
) : ControllerBase
{
    /// <summary>
    /// Registers the authenticated principal as a user.
    ///
    /// Prerequisite: the caller must already have a principal (created
    /// automatically on first login). This endpoint creates the user record
    /// and links it back to the existing principal:
    ///   1. UserCreated           — user record written in Banking.Users
    ///   2. PrincipalAttributeSet — user_id linked to principal
    ///   3. PrincipalRoleAdded    — "user" role assigned
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserRegistrationResult>> Register(
        [FromBody] RegisterRequest request
    )
    {
        if (!principalContext.IsResolved)
            return Forbid();

        var principalId = principalContext.Principal.Id;
        var identity = principalContext.Principal.Identities.First();

        await bus.InvokeAsync(
            new RegisterUserCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: principalId,
                Name: request.Name,
                DateOfBirth: request.DateOfBirth,
                Email: request.Email
            )
        );

        var users = await userService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u =>
            u.Emails.Any(e => e.Address == request.Email.ToLowerInvariant())
        );

        if (user is null)
            return StatusCode(500, "User record was not created");

        var principal = await principalService.GetByIdentityAsync(
            identity.Provider,
            identity.ExternalId
        );

        return Ok(new UserRegistrationResult(user, principal));
    }
}

public record RegisterRequest(NameInput Name, DateTime DateOfBirth, string Email);

public record UserRegistrationResult(UserResponse User, PrincipalResponse Principal);
