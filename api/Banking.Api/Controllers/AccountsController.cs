using Banking.Accounts;
using Banking.Accounts.DTO.Responses;
using Banking.Api.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Accounts")]
[Authorize]
internal class AccountsController(IMessageBus bus, AccountService accountService) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Account
     |--------------------------------------------------------------------------------
     */

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        await bus.InvokeAsync(
            new CreateAccountCommand(
                CorrelationId: Guid.NewGuid(),
                AccountType: request.AccountType,
                CurrencyCode: request.CurrencyCode,
                PrimaryHolderId: request.PrimaryHolderId,
                PrimaryHolderType: request.PrimaryHolderType
            )
        );
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AccountResponse>> GetAccount(Guid id)
    {
        var account = await accountService.GetAccountByIdAsync(id);
        return Ok(account);
    }

    [HttpGet("holder/{holderId:guid}")]
    public async Task<ActionResult<IEnumerable<AccountResponse>>> GetAccountsByHolder(Guid holderId)
    {
        var accounts = await accountService.GetAccountsByHolderAsync(holderId);
        return Ok(accounts);
    }

    [HttpPatch("{id:guid}/freeze")]
    public async Task<IActionResult> FreezeAccount(Guid id)
    {
        await bus.InvokeAsync(
            new FreezeAccountCommand(CorrelationId: Guid.NewGuid(), AccountId: id)
        );
        return NoContent();
    }

    [HttpPatch("{id:guid}/unfreeze")]
    public async Task<IActionResult> UnfreezeAccount(Guid id)
    {
        await bus.InvokeAsync(
            new UnfreezeAccountCommand(CorrelationId: Guid.NewGuid(), AccountId: id)
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> CloseAccount(Guid id)
    {
        await bus.InvokeAsync(
            new CloseAccountCommand(CorrelationId: Guid.NewGuid(), AccountId: id)
        );
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Holders
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/holders")]
    public async Task<IActionResult> AddHolder(Guid id, [FromBody] AddHolderRequest request)
    {
        await bus.InvokeAsync(
            new AddAccountHolderCommand(
                CorrelationId: Guid.NewGuid(),
                AccountId: id,
                HolderId: request.HolderId,
                HolderType: request.HolderType
            )
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}/holders/{holderId:guid}")]
    public async Task<IActionResult> RemoveHolder(Guid id, Guid holderId)
    {
        await bus.InvokeAsync(
            new RemoveAccountHolderCommand(
                CorrelationId: Guid.NewGuid(),
                AccountId: id,
                HolderId: holderId
            )
        );
        return NoContent();
    }
}

/*
 |--------------------------------------------------------------------------------
 | Request bodies
 |--------------------------------------------------------------------------------
 */

public record CreateAccountRequest(
    string AccountType,
    string CurrencyCode,
    Guid PrimaryHolderId,
    string PrimaryHolderType
);

public record AddHolderRequest(Guid HolderId, string HolderType);
