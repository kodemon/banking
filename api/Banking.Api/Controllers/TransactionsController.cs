using Banking.Api.Commands;
using Banking.Transactions;
using Banking.Transactions.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Transactions")]
[Authorize]
internal class TransactionsController(IMessageBus bus, TransactionService transactionService)
    : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Create
     |--------------------------------------------------------------------------------
     */

    [HttpPost("deposit")]
    public async Task<IActionResult> CreateDeposit([FromBody] CreateDepositRequest request)
    {
        await bus.InvokeAsync(
            new CreateDepositCommand(
                CorrelationId: Guid.NewGuid(),
                DestinationParticipantId: request.DestinationParticipantId,
                Amount: request.Amount,
                CurrencyCode: request.CurrencyCode,
                Description: request.Description
            )
        );
        return NoContent();
    }

    [HttpPost("withdrawal")]
    public async Task<IActionResult> CreateWithdrawal([FromBody] CreateWithdrawalRequest request)
    {
        await bus.InvokeAsync(
            new CreateWithdrawalCommand(
                CorrelationId: Guid.NewGuid(),
                SourceParticipantId: request.SourceParticipantId,
                Amount: request.Amount,
                CurrencyCode: request.CurrencyCode,
                Description: request.Description
            )
        );
        return NoContent();
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferRequest request)
    {
        await bus.InvokeAsync(
            new CreateTransferCommand(
                CorrelationId: Guid.NewGuid(),
                SourceParticipantId: request.SourceParticipantId,
                DestinationParticipantId: request.DestinationParticipantId,
                Amount: request.Amount,
                CurrencyCode: request.CurrencyCode,
                Description: request.Description
            )
        );
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Read
     |--------------------------------------------------------------------------------
     */

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> GetTransaction(Guid id)
    {
        var transaction = await transactionService.GetTransactionByIdAsync(id);
        return Ok(transaction);
    }

    [HttpGet("account/{participantId:guid}")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByAccount(
        Guid participantId
    )
    {
        var transactions = await transactionService.GetTransactionsByAccountAsync(participantId);
        return Ok(transactions);
    }

    /*
     |--------------------------------------------------------------------------------
     | Status transitions
     |--------------------------------------------------------------------------------
     */

    [HttpPatch("{id:guid}/complete")]
    public async Task<IActionResult> CompleteTransaction(Guid id)
    {
        await bus.InvokeAsync(
            new CompleteTransactionCommand(CorrelationId: Guid.NewGuid(), TransactionId: id)
        );
        return NoContent();
    }

    [HttpPatch("{id:guid}/fail")]
    public async Task<IActionResult> FailTransaction(
        Guid id,
        [FromBody] FailTransactionRequest request
    )
    {
        await bus.InvokeAsync(
            new FailTransactionCommand(
                CorrelationId: Guid.NewGuid(),
                TransactionId: id,
                Reason: request.Reason
            )
        );
        return NoContent();
    }

    [HttpPatch("{id:guid}/reverse")]
    public async Task<IActionResult> ReverseTransaction(
        Guid id,
        [FromBody] ReverseTransactionRequest request
    )
    {
        await bus.InvokeAsync(
            new ReverseTransactionCommand(
                CorrelationId: Guid.NewGuid(),
                TransactionId: id,
                Reason: request.Reason
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

public record CreateDepositRequest(
    Guid DestinationParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

public record CreateWithdrawalRequest(
    Guid SourceParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

public record CreateTransferRequest(
    Guid SourceParticipantId,
    Guid DestinationParticipantId,
    long Amount,
    string CurrencyCode,
    string Description
);

public record FailTransactionRequest(string Reason);

public record ReverseTransactionRequest(string Reason);
