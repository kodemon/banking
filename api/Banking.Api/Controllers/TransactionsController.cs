using Banking.Accounts.Queries;
using Banking.Api.Identity;
using Banking.Shared.ValueObjects;
using Banking.Transactions.Commands;
using Banking.Transactions.Database.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/transactions")]
[RequireSession]
[Tags("Transaction")]
internal class TransactionsController(IAuth auth, IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<Transaction>(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Transfer(CreateTransactionDTO payload)
    {
        var account = await mediator.Send(new GetAccountByIdQuery(payload.FromAccountId));
        if (account is null)
        {
            return NotFound();
        }

        var allowed = await auth.IsAllowed(
            "account",
            "*",
            "create.transaction",
            new Attributes().GuidList("accountHolders", account.GetAccountHolderIds())
        );
        if (!allowed)
        {
            return NotFound();
        }

        var transaction = await mediator.Send(
            new CreateTransferCommand(
                payload.FromAccountId,
                payload.ToAccountId,
                payload.Amount,
                Currency.FromCode(payload.Currency),
                payload.Description
            )
        );

        return Ok(transaction);
    }
}

internal record CreateTransactionDTO
{
    public required Guid FromAccountId { get; init; }
    public required Guid ToAccountId { get; init; }
    public required string Currency { get; init; }
    public required long Amount { get; init; }
    public required string Description { get; init; }
}
