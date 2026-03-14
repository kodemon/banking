using MediatR;

namespace Banking.Principals.Commands;

internal record RemovePrincipalIdentityCommand(Guid PrincipalId, string Provider, string ExternalId)
    : IRequest;

internal sealed class RemovePrincipalIdentityHandler(IPrincipalRepository repository)
    : IRequestHandler<RemovePrincipalIdentityCommand>
{
    public async Task Handle(RemovePrincipalIdentityCommand message, CancellationToken token)
    {
        var principal = await repository.GetByIdAsync(message.PrincipalId);
        if (principal is null)
        {
            return;
        }

        principal.RemoveIdentity(message.Provider, message.ExternalId);

        await repository.SaveChangesAsync();
    }
}
