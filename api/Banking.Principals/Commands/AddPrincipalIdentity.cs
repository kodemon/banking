using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record AddPrincipalIdentityCommand(Guid PrincipalId, string Provider, string ExternalId)
    : IRequest<PrincipalIdentity>;

internal sealed class AddPrincipalIdentityHandler(IPrincipalRepository repository)
    : IRequestHandler<AddPrincipalIdentityCommand, PrincipalIdentity>
{
    public async Task<PrincipalIdentity> Handle(
        AddPrincipalIdentityCommand message,
        CancellationToken token
    )
    {
        var principal = await repository.GetByIdAsync(message.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{message.PrincipalId}' not found.");
        }

        var identity = principal.GetIdentity(message.Provider, message.ExternalId);
        if (identity is not null)
        {
            return identity;
        }

        identity = principal.AddIdentity(message.Provider, message.ExternalId);

        await repository.SaveChangesAsync();

        return identity;
    }
}
