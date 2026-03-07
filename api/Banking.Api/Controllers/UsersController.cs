using Banking.Api.Commands;
using Banking.Shared.ValueObjects;
using Banking.Users;
using Banking.Users.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Users")]
[Authorize]
internal class UsersController(IMessageBus bus, UserService userService) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | User
     |--------------------------------------------------------------------------------
     */

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        var correlationId = Guid.NewGuid();
        await bus.InvokeAsync(
            new CreateUserCommand(
                CorrelationId: correlationId,
                Name: request.Name,
                DateOfBirth: request.DateOfBirth,
                Email: request.Email
            )
        );

        // Query back the created user — the handler wrote it synchronously
        var users = await userService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u =>
            u.Emails.Any(e => e.Address == request.Email.ToLowerInvariant())
        );
        return user is null
            ? StatusCode(500)
            : CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("{id:guid}/name")]
    public async Task<IActionResult> UpdateUserName(
        Guid id,
        [FromBody] UpdateUserNameRequest request
    )
    {
        await bus.InvokeAsync(
            new UpdateUserNameCommand(CorrelationId: Guid.NewGuid(), UserId: id, Name: request.Name)
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await bus.InvokeAsync(new DeleteUserCommand(CorrelationId: Guid.NewGuid(), UserId: id));
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Emails
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/emails")]
    public async Task<IActionResult> AddEmail(Guid id, [FromBody] AddEmailRequest request)
    {
        await bus.InvokeAsync(
            new AddUserEmailCommand(
                CorrelationId: Guid.NewGuid(),
                UserId: id,
                Address: request.Address,
                Type: request.Type
            )
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}/emails/{emailId:guid}")]
    public async Task<IActionResult> RemoveEmail(Guid id, Guid emailId)
    {
        await bus.InvokeAsync(
            new RemoveUserEmailCommand(CorrelationId: Guid.NewGuid(), UserId: id, EmailId: emailId)
        );
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Addresses
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/addresses")]
    public async Task<IActionResult> AddAddress(Guid id, [FromBody] AddAddressRequest request)
    {
        await bus.InvokeAsync(
            new AddUserAddressCommand(
                CorrelationId: Guid.NewGuid(),
                UserId: id,
                Street: request.Street,
                City: request.City,
                PostalCode: request.PostalCode,
                Country: request.Country,
                Region: request.Region
            )
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}/addresses/{addressId:guid}")]
    public async Task<IActionResult> RemoveAddress(Guid id, Guid addressId)
    {
        await bus.InvokeAsync(
            new RemoveUserAddressCommand(
                CorrelationId: Guid.NewGuid(),
                UserId: id,
                AddressId: addressId
            )
        );
        return NoContent();
    }
}

/*
 |--------------------------------------------------------------------------------
 | Request bodies
 | (Domain request DTOs are internal — we define public API bodies here)
 |--------------------------------------------------------------------------------
 */

public record CreateUserRequest(NameInput Name, DateTime DateOfBirth, string Email);

public record UpdateUserNameRequest(NameInput Name);

public record AddEmailRequest(string Address, EmailType Type);

public record AddAddressRequest(
    string Street,
    string City,
    string PostalCode,
    string Country,
    string? Region
);
