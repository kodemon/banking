using Banking.Api.Commands;
using Banking.Events;
using Banking.Transactions.Events;
using Wolverine;

namespace Banking.Api.Handlers;

public class TransactionCommandHandler(IEventStore eventStore, IMessageBus bus)
{
    public async Task Handle(CreateDepositCommand cmd)
    {
        var evt = new DepositCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: Guid.NewGuid(),
            DestinationParticipantId: cmd.DestinationParticipantId,
            Amount: cmd.Amount,
            CurrencyCode: cmd.CurrencyCode,
            Description: cmd.Description
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(CreateWithdrawalCommand cmd)
    {
        var evt = new WithdrawalCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: Guid.NewGuid(),
            SourceParticipantId: cmd.SourceParticipantId,
            Amount: cmd.Amount,
            CurrencyCode: cmd.CurrencyCode,
            Description: cmd.Description
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(CreateTransferCommand cmd)
    {
        var evt = new TransferCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: Guid.NewGuid(),
            SourceParticipantId: cmd.SourceParticipantId,
            DestinationParticipantId: cmd.DestinationParticipantId,
            Amount: cmd.Amount,
            CurrencyCode: cmd.CurrencyCode,
            Description: cmd.Description
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(CreateFeeCommand cmd)
    {
        var evt = new FeeCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: Guid.NewGuid(),
            ParticipantId: cmd.ParticipantId,
            Amount: cmd.Amount,
            CurrencyCode: cmd.CurrencyCode,
            Description: cmd.Description
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(CreateInterestCommand cmd)
    {
        var evt = new InterestCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: Guid.NewGuid(),
            ParticipantId: cmd.ParticipantId,
            Amount: cmd.Amount,
            CurrencyCode: cmd.CurrencyCode,
            Description: cmd.Description
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(CompleteTransactionCommand cmd)
    {
        var evt = new TransactionCompleted(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: cmd.TransactionId
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(FailTransactionCommand cmd)
    {
        var evt = new TransactionFailed(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: cmd.TransactionId,
            Reason: cmd.Reason
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(ReverseTransactionCommand cmd)
    {
        var evt = new TransactionReversed(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            TransactionId: cmd.TransactionId,
            Reason: cmd.Reason
        );

        await eventStore.AppendAsync(evt, streamId: evt.TransactionId);
        await bus.InvokeAsync(evt);
    }
}
