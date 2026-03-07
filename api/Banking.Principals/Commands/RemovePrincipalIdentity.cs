using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record RemovePrincipalIdentityCommand(Guid PrincipalId, string Provider, string ExternalId)
    : IRequest<Principal>;

internal sealed class RemovePrincipalIdentityHandler(IPrincipalRepository principalRepository)
    : IRequestHandler<RemovePrincipalIdentityCommand, Principal>
{
    public async Task<Principal> Handle(RemovePrincipalIdentityCommand cmd, CancellationToken ct)
    {
        var principal = await principalRepository.GetByIdAsync(cmd.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{cmd.PrincipalId}' not found.");
        }

        var identity = principal.Identities.Find(i =>
            i.Provider == cmd.Provider && i.ExternalId == cmd.ExternalId
        );

        if (identity is not null)
        {
            principal.Identities.Remove(identity);
            await principalRepository.SaveChangesAsync();
        }

        return principal;
    }
}
