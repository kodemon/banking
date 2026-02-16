using Banking.Application.DTOs.Identity;
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

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound($"User {id} not found");

        return Ok(user);
    }

    /// <summary>
    /// Update user details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.UpdateUserAsync(id, request);

        if (user == null)
            return NotFound($"User {id} not found");

        return Ok(user);
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var deleted = await _userService.DeleteUserAsync(id);

        if (!deleted)
            return NotFound($"User {id} not found");

        return NoContent();
    }

    /// <summary>
    /// Add address to user
    /// </summary>
    [HttpPost("{id}/addresses")]
    public async Task<ActionResult<AddressResponse>> AddAddress(Guid id, [FromBody] AddAddressRequest request)
    {
        try
        {
            var address = await _userService.AddAddressAsync(id, request);
            return Ok(address);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove address from user
    /// </summary>
    [HttpDelete("{id}/addresses/{addressId}")]
    public async Task<IActionResult> RemoveAddress(Guid id, Guid addressId)
    {
        var removed = await _userService.RemoveAddressAsync(id, addressId);

        if (!removed)
            return NotFound($"Address {addressId} not found for user {id}");

        return NoContent();
    }

    /// <summary>
    /// Add email to user
    /// </summary>
    [HttpPost("{id}/emails")]
    public async Task<ActionResult<EmailResponse>> AddEmail(Guid id, [FromBody] AddEmailRequest request)
    {
        try
        {
            var email = await _userService.AddEmailAsync(id, request);
            return Ok(email);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove email from user
    /// </summary>
    [HttpDelete("{id}/emails/{emailId}")]
    public async Task<IActionResult> RemoveEmail(Guid id, Guid emailId)
    {
        var removed = await _userService.RemoveEmailAsync(id, emailId);

        if (!removed)
            return NotFound($"Email {emailId} not found for user {id}");

        return NoContent();
    }
}