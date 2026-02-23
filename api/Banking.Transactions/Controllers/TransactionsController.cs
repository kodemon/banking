using Banking.Transactions.DTO.Requests;
using Banking.Transactions.DTO.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Transactions;

[ApiController]
[Route("api/[controller]")]
internal class TransactionsController(TransactionService transactionService) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Create
     |--------------------------------------------------------------------------------
     */

    [HttpPost("deposit")]
    public async Task<ActionResult<TransactionResponse>> CreateDeposit([FromBody] CreateDepositRequest request)
    {
        var transaction = await transactionService.CreateDepositAsync(request);
        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    [HttpPost("withdrawal")]
    public async Task<ActionResult<TransactionResponse>> CreateWithdrawal([FromBody] CreateWithdrawalRequest request)
    {
        var transaction = await transactionService.CreateWithdrawalAsync(request);
        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    [HttpPost("transfer")]
    public async Task<ActionResult<TransactionResponse>> CreateTransfer([FromBody] CreateTransferRequest request)
    {
        var transaction = await transactionService.CreateTransferAsync(request);
        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    /*
     |--------------------------------------------------------------------------------
     | Read
     |--------------------------------------------------------------------------------
     */

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionResponse>> GetTransaction(Guid id)
    {
        var transaction = await transactionService.GetTransactionByIdAsync(id);
        return Ok(transaction);
    }

    [HttpGet("account/{participantId}")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByAccount(Guid participantId)
    {
        var transactions = await transactionService.GetTransactionsByAccountAsync(participantId);
        return Ok(transactions);
    }

    /*
     |--------------------------------------------------------------------------------
     | Status Transitions
     |--------------------------------------------------------------------------------
     */

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TransactionResponse>> CompleteTransaction(Guid id)
    {
        var transaction = await transactionService.CompleteTransactionAsync(id);
        return Ok(transaction);
    }

    [HttpPatch("{id}/fail")]
    public async Task<ActionResult<TransactionResponse>> FailTransaction(Guid id)
    {
        var transaction = await transactionService.FailTransactionAsync(id);
        return Ok(transaction);
    }

    [HttpPatch("{id}/reverse")]
    public async Task<ActionResult<TransactionResponse>> ReverseTransaction(Guid id)
    {
        var transaction = await transactionService.ReverseTransactionAsync(id);
        return Ok(transaction);
    }
}