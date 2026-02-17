using Banking.Application.DTOs.Users;
using Banking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /*
     |--------------------------------------------------------------------------------
     | User
     |--------------------------------------------------------------------------------
     */

    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.UpdateUserAsync(id, request);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUserAsync(id);
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
        var address = await _userService.AddAddressAsync(id, request);
        return Ok(address);
    }

    [HttpDelete("{id}/addresses/{addressId}")]
    public async Task<IActionResult> RemoveAddress(Guid id, Guid addressId)
    {
        await _userService.RemoveAddressAsync(id, addressId);
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
        var email = await _userService.AddEmailAsync(id, request);
        return Ok(email);
    }

    [HttpDelete("{id}/emails/{emailId}")]
    public async Task<IActionResult> RemoveEmail(Guid id, Guid emailId)
    {
        await _userService.RemoveEmailAsync(id, emailId);
        return NoContent();
    }
}