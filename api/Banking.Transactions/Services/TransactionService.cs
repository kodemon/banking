using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Transactions.Interfaces;
using Banking.Transactions.Repositories.Resources;

namespace Banking.Transactions;

internal class TransactionService(ITransactionRepository transactionRepository)
{
    /*
     |--------------------------------------------------------------------------------
     | Create
     |--------------------------------------------------------------------------------
     */

    public async Task<Transaction> CreateDepositAsync(
        Guid destinationParticipantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = Transaction.CreateDeposit(
            destinationParticipantId,
            amount,
            currency,
            description
        );

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction> CreateWithdrawalAsync(
        Guid participantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = Transaction.CreateWithdrawal(
            participantId,
            amount,
            currency,
            description
        );

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction;
    }

    public async Task<Transaction> CreateTransferAsync(
        Guid fromParticipantId,
        Guid toParticipantId,
        long amount,
        Currency currency,
        string description
    )
    {
        var transaction = Transaction.CreateTransfer(
            fromParticipantId,
            toParticipantId,
            amount,
            currency,
            description
        );

        await transactionRepository.AddAsync(transaction);
        await transactionRepository.SaveChangesAsync();

        return transaction;
    }

    /*
     |--------------------------------------------------------------------------------
     | Read
     |--------------------------------------------------------------------------------
     */

    public async Task<Transaction> GetTransactionByIdAsync(Guid transactionId)
    {
        return await GetTransaction(transactionId);
    }

    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountAsync(Guid accountId)
    {
        return await transactionRepository.GetAllByParticipantAsync(accountId);
    }

    /*
     |--------------------------------------------------------------------------------
     | Status Transitions
     |--------------------------------------------------------------------------------
     */

    public async Task<Transaction> CompleteTransactionAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        transaction.Complete();
        await transactionRepository.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> FailTransactionAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        transaction.Fail();
        await transactionRepository.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> ReverseTransactionAsync(Guid transactionId)
    {
        var transaction = await GetTransaction(transactionId);
        transaction.Reverse();
        await transactionRepository.SaveChangesAsync();
        return transaction;
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
        {
            throw new ResourceNotFoundException($"Transaction {transactionId} not found");
        }
        return transaction;
    }
}
