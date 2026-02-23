using Banking.Accounts.DTO.Requests;
using Banking.Accounts.DTO.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Accounts;

[ApiController]
[Route("api/[controller]")]
internal class AccountsController(AccountService accountService) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Account
     |--------------------------------------------------------------------------------
     */

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var account = await accountService.CreateAccountAsync(request);
        return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponse>> GetAccount(Guid id)
    {
        var account = await accountService.GetAccountByIdAsync(id);
        return Ok(account);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAccountsByUser(Guid userId)
    {
        var accounts = await accountService.GetAccountsByUserAsync(userId);
        return Ok(accounts);
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAccountsByBusiness(Guid businessId)
    {
        var accounts = await accountService.GetAccountsByBusinessAsync(businessId);
        return Ok(accounts);
    }

    [HttpPatch("{id}/freeze")]
    public async Task<ActionResult<AccountResponse>> FreezeAccount(Guid id)
    {
        var account = await accountService.FreezeAccountAsync(id);
        return Ok(account);
    }

    [HttpPatch("{id}/unfreeze")]
    public async Task<ActionResult<AccountResponse>> UnfreezeAccount(Guid id)
    {
        var account = await accountService.UnfreezeAccountAsync(id);
        return Ok(account);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CloseAccount(Guid id)
    {
        await accountService.CloseAccountAsync(id);
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Personal Holders
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id}/holders/personal")]
    public async Task<ActionResult<PersonalHolderResponse>> AddPersonalHolder(Guid id, [FromBody] AddPersonalHolderRequest request)
    {
        var holder = await accountService.AddPersonalHolderAsync(id, request);
        return Ok(holder);
    }

    [HttpDelete("{id}/holders/personal/{holderId}")]
    public async Task<IActionResult> RemovePersonalHolder(Guid id, Guid holderId)
    {
        await accountService.RemovePersonalHolderAsync(id, holderId);
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Business Holders
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id}/holders/business")]
    public async Task<ActionResult<BusinessHolderResponse>> AddBusinessHolder(Guid id, [FromBody] AddBusinessHolderRequest request)
    {
        var holder = await accountService.AddBusinessHolderAsync(id, request);
        return Ok(holder);
    }

    [HttpDelete("{id}/holders/business/{holderId}")]
    public async Task<IActionResult> RemoveBusinessHolder(Guid id, Guid holderId)
    {
        await accountService.RemoveBusinessHolderAsync(id, holderId);
        return NoContent();
    }
}