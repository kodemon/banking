using Banking.Users.DTO.Requests;
using Banking.Users.DTO.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Users;

[ApiController]
[Route("api/[controller]")]
internal class UsersController(UserService userService) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | User
     |--------------------------------------------------------------------------------
     */

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await userService.UpdateUserAsync(id, request);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await userService.DeleteUserAsync(id);
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | User Address
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id}/addresses")]
    public async Task<ActionResult<AddressResponse>> AddAddress(Guid id, [FromBody] AddAddressRequest request)
    {
        var address = await userService.AddAddressAsync(id, request);
        return Ok(address);
    }

    [HttpDelete("{id}/addresses/{addressId}")]
    public async Task<IActionResult> RemoveAddress(Guid id, Guid addressId)
    {
        await userService.RemoveAddressAsync(id, addressId);
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | User Email
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id}/emails")]
    public async Task<ActionResult<EmailResponse>> AddEmail(Guid id, [FromBody] AddEmailRequest request)
    {
        var email = await userService.AddEmailAsync(id, request);
        return Ok(email);
    }

    [HttpDelete("{id}/emails/{emailId}")]
    public async Task<IActionResult> RemoveEmail(Guid id, Guid emailId)
    {
        await userService.RemoveEmailAsync(id, emailId);
        return NoContent();
    }
}