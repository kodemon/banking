using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Transactions.DTO.Requests;
using Banking.Transactions.DTO.Responses;

namespace Banking.Transactions;

internal class TransactionService(ITransactionRepository transactionRepository)
{
    /*
     |--------------------------------------------------------------------------------
     | Create
     |--------------------------------------------------------------------------------
     */

    public async Task<TransactionResponse> CreateDepositAsync(CreateDepositRequest request)
    {
        var transaction = Transaction.CreateDeposit(
            request.DestinationParticipantId,
            request.Amount,
            Currency.FromCode(request.CurrencyCode),
            request.Description
        );

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction.ToResponse();
    }

    public async Task<TransactionResponse> CreateWithdrawalAsync(CreateWithdrawalRequest request)
    {
        var transaction = Transaction.CreateWithdrawal(
            request.SourceParticipantId,
            request.Amount,
            Currency.FromCode(request.CurrencyCode),
            request.Description
        );

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction.ToResponse();
    }

    public async Task<TransactionResponse> CreateTransferAsync(CreateTransferRequest request)
    {
        var transaction = Transaction.CreateTransfer(
            request.SourceParticipantId,
            request.SourceParticipantId,
            request.Amount,
            Currency.FromCode(request.CurrencyCode),
            request.Description
        );

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction.ToResponse();
    }

    /*
     |--------------------------------------------------------------------------------
     | Read
     |--------------------------------------------------------------------------------
     */

    public async Task<TransactionResponse> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        return transaction.ToResponse();
    }

    public async Task<IEnumerable<TransactionResponse>> GetTransactionsByAccountAsync(Guid accountId)
    {
        var transactions = await transactionRepository.GetAllByParticipantAsync(accountId);
        return transactions.Select(t => t.ToResponse());
    }

    /*
     |--------------------------------------------------------------------------------
     | Status Transitions
     |--------------------------------------------------------------------------------
     */

    public async Task<TransactionResponse> CompleteTransactionAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        transaction.Complete();
        await transactionRepository.SaveChangesAsync();
        return transaction.ToResponse();
    }

    public async Task<TransactionResponse> FailTransactionAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        transaction.Fail();
        await transactionRepository.SaveChangesAsync();
        return transaction.ToResponse();
    }

    public async Task<TransactionResponse> ReverseTransactionAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        transaction.Reverse();
        await transactionRepository.SaveChangesAsync();
        return transaction.ToResponse();
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers
     |--------------------------------------------------------------------------------
     */

    private async Task<Transaction> GetTransaction(Guid transactionId)
    {
        var transaction = await transactionRepository.GetByIdAsync(transactionId);
        if (transaction is null)
            throw new AggregateNotFoundException($"Transaction {transactionId} not found");
        return transaction;
    }
}