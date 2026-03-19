using Banking.Shared.ValueObjects;
using Banking.Transactions.Database.Models;
using Banking.Transactions.Interfaces;
using MediatR;

namespace Banking.Transactions.Commands;

internal record CreateTransferCommand(
    Guid fromParticipantId,
    Guid toParticipantId,
    long amount,
    Currency currency,
    string description
) : IRequest<Transaction>;

internal sealed class CreateTransferHandler(ITransactionRepository repository)
    : IRequestHandler<CreateTransferCommand, Transaction>
{
    public async Task<Transaction> Handle(CreateTransferCommand message, CancellationToken token)
    {
        var transaction = Transaction.CreateTransfer(
            message.fromParticipantId,
            message.toParticipantId,
            message.amount,
            message.currency,
            message.description
        );

        await repository.AddAsync(transaction);
        await repository.SaveChangesAsync();

        return transaction;
    }
}
