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

    [HttpGet("holder/{holderId}")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAccountsByHolder(Guid holderId)
    {
        var accounts = await accountService.GetAccountsByHolderAsync(holderId);
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
     | Account Holders
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id}/holders/personal")]
    public async Task<ActionResult<AccountHolderResponse>> AddPersonalHolder(Guid id, [FromBody] AddAccountHolderRequest request)
    {
        var holder = await accountService.AddAccountHolderAsync(id, request);
        return Ok(holder);
    }

    [HttpDelete("{id}/holders/personal/{holderId}")]
    public async Task<IActionResult> RemovePersonalHolder(Guid id, Guid holderId)
    {
        await accountService.RemovePersonalHolderAsync(id, holderId);
        return NoContent();
    }
}