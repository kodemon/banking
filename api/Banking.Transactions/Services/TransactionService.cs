using Banking.Shared.Exceptions;
using Banking.Transactions.DTO.Responses;

namespace Banking.Transactions;

/*
 |--------------------------------------------------------------------------------
 | TransactionService  [Query Only]
 |--------------------------------------------------------------------------------
 |
 | Read-only query surface for Banking.Api controllers. All mutations are
 | driven by events handled in Banking.Transactions.Handlers.TransactionEventHandlers.
 |
 */

internal class TransactionService(ITransactionRepository transactionRepository)
{
    public async Task<TransactionResponse> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction =
            await transactionRepository.GetByIdAsync(transactionId)
            ?? throw new AggregateNotFoundException($"Transaction {transactionId} not found");
        return transaction.ToResponse();
    }

    public async Task<IEnumerable<TransactionResponse>> GetTransactionsByAccountAsync(
        Guid accountId
    )
    {
        var transactions = await transactionRepository.GetAllByParticipantAsync(accountId);
        return transactions.Select(t => t.ToResponse());
    }
}
