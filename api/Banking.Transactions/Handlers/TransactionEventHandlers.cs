using Banking.Shared.ValueObjects;
using Banking.Transactions.Events;
using Banking.Transactions.Persistence;
using Wolverine.Attributes;

namespace Banking.Transactions.Handlers;

/*
 |--------------------------------------------------------------------------------
 | TransactionEventHandlers
 |--------------------------------------------------------------------------------
 |
 | Reacts to transaction domain events published by Banking.Api command handlers.
 | Each creation event reconstitutes the Transaction aggregate with the known Id
 | from the event, then adds the appropriate journal entries.
 | No knowledge of any other domain.
 |
 */

[WolverineHandler]
public class TransactionEventHandlers(ITransactionRepository repository, TransactionsDbContext db)
{
    public async Task Handle(DepositCreated evt)
    {
        var currency = Currency.FromCode(evt.CurrencyCode);
        var transaction = Transaction.Reconstitute(
            evt.TransactionId,
            TransactionType.Deposit,
            evt.Description,
            evt.Amount,
            currency,
            evt.OccurredAt
        );

        transaction.AddDebitEntry(evt.DestinationParticipantId);
        await repository.AddAsync(transaction);
    }

    public async Task Handle(WithdrawalCreated evt)
    {
        var currency = Currency.FromCode(evt.CurrencyCode);
        var transaction = Transaction.Reconstitute(
            evt.TransactionId,
            TransactionType.Withdrawal,
            evt.Description,
            evt.Amount,
            currency,
            evt.OccurredAt
        );

        transaction.AddCreditEntry(evt.SourceParticipantId);
        await repository.AddAsync(transaction);
    }

    public async Task Handle(TransferCreated evt)
    {
        var currency = Currency.FromCode(evt.CurrencyCode);
        var transaction = Transaction.Reconstitute(
            evt.TransactionId,
            TransactionType.Transfer,
            evt.Description,
            evt.Amount,
            currency,
            evt.OccurredAt
        );

        transaction.AddCreditEntry(evt.SourceParticipantId);
        transaction.AddDebitEntry(evt.DestinationParticipantId);
        await repository.AddAsync(transaction);
    }

    public async Task Handle(FeeCreated evt)
    {
        var currency = Currency.FromCode(evt.CurrencyCode);
        var transaction = Transaction.Reconstitute(
            evt.TransactionId,
            TransactionType.Fee,
            evt.Description,
            evt.Amount,
            currency,
            evt.OccurredAt
        );

        transaction.AddCreditEntry(evt.ParticipantId);
        await repository.AddAsync(transaction);
    }

    public async Task Handle(InterestCreated evt)
    {
        var currency = Currency.FromCode(evt.CurrencyCode);
        var transaction = Transaction.Reconstitute(
            evt.TransactionId,
            TransactionType.Interest,
            evt.Description,
            evt.Amount,
            currency,
            evt.OccurredAt
        );

        transaction.AddDebitEntry(evt.ParticipantId);
        await repository.AddAsync(transaction);
    }

    public async Task Handle(TransactionCompleted evt)
    {
        var transaction =
            await repository.GetByIdAsync(evt.TransactionId)
            ?? throw new InvalidOperationException(
                $"Transaction {evt.TransactionId} not found for TransactionCompleted"
            );

        transaction.Complete();
    }

    public async Task Handle(TransactionFailed evt)
    {
        var transaction =
            await repository.GetByIdAsync(evt.TransactionId)
            ?? throw new InvalidOperationException(
                $"Transaction {evt.TransactionId} not found for TransactionFailed"
            );

        transaction.Fail();
    }

    public async Task Handle(TransactionReversed evt)
    {
        var transaction =
            await repository.GetByIdAsync(evt.TransactionId)
            ?? throw new InvalidOperationException(
                $"Transaction {evt.TransactionId} not found for TransactionReversed"
            );

        transaction.Reverse();
    }
}
