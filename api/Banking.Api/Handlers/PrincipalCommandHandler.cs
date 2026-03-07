using Banking.Api.Commands;
using Banking.Events;
using Banking.Principals.Events;
using Wolverine;

namespace Banking.Api.Handlers;

public class PrincipalCommandHandler(IEventStore eventStore, IMessageBus bus)
{
    public async Task Handle(CreatePrincipalCommand cmd)
    {
        var evt = new PrincipalCreated(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Provider: cmd.Provider,
            ExternalId: cmd.ExternalId
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(DeletePrincipalCommand cmd)
    {
        var evt = new PrincipalDeleted(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(AddPrincipalIdentityCommand cmd)
    {
        var evt = new PrincipalIdentityAdded(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Provider: cmd.Provider,
            ExternalId: cmd.ExternalId
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(RemovePrincipalIdentityCommand cmd)
    {
        var evt = new PrincipalIdentityRemoved(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Provider: cmd.Provider,
            ExternalId: cmd.ExternalId
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(AddPrincipalRoleCommand cmd)
    {
        var evt = new PrincipalRoleAdded(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Role: cmd.Role
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(RemovePrincipalRoleCommand cmd)
    {
        var evt = new PrincipalRoleRemoved(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Role: cmd.Role
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(SetPrincipalAttributeCommand cmd)
    {
        var evt = new PrincipalAttributeSet(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Domain: cmd.Domain,
            Key: cmd.Key,
            Value: cmd.Value
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }

    public async Task Handle(RemovePrincipalAttributeCommand cmd)
    {
        var evt = new PrincipalAttributeRemoved(
            EventId: Guid.NewGuid(),
            CorrelationId: cmd.CorrelationId,
            PrincipalId: cmd.PrincipalId,
            Domain: cmd.Domain,
            Key: cmd.Key
        );

        await eventStore.AppendAsync(evt, streamId: evt.PrincipalId);
        await bus.InvokeAsync(evt);
    }
}
