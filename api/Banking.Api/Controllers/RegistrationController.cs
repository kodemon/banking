using Banking.Principal;
using Banking.Principal.AccessControl;
using Banking.Principal.DTO.Requests;
using Banking.Users;
using Banking.Users.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
internal class RegistrationController(
    UserService userService,
    PrincipalService principalService,
    IPrincipalContext principalContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!principalContext.IsResolved)
        {
            return Forbid();
        }

        // Step 1 — create user
        var user = await userService.CreateUserAsync(new CreateUserRequest
        {
            Name = request.Name,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email
        });

        try
        {
            // Step 2 — bind to principal
            await principalService.SetAttributeAsync(
                principalContext.Principal.Id,
                new SetAttributeRequest
                {
                    Domain = "user",
                    Key = "user_id",
                    Value = user.Id.ToString()
                });
        }
        catch
        {
            await userService.DeleteUserAsync(user.Id);
            throw;
        }

        return Ok(user);
    }
}

internal record RegisterRequest
{
    public required Banking.Shared.ValueObjects.NameInput Name { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Email { get; init; }
}
