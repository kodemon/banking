using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record AddPrincipalIdentityCommand(Guid PrincipalId, string Provider, string ExternalId)
    : IRequest<Principal>;

internal sealed class AddPrincipalIdentityHandler(
    IPrincipalRepository principalRepository,
    IPrincipalIdentityRepository identityRepository
) : IRequestHandler<AddPrincipalIdentityCommand, Principal>
{
    public async Task<Principal> Handle(AddPrincipalIdentityCommand cmd, CancellationToken ct)
    {
        if (await identityRepository.HasIdentityAsync(cmd.Provider, cmd.ExternalId))
        {
            throw new ResourceConflictException(
                $"Identity '{cmd.Provider}:{cmd.ExternalId}' is already bound to a principal."
            );
        }

        var principal = await principalRepository.GetByIdAsync(cmd.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{cmd.PrincipalId}' not found.");
        }

        principal.Identities.Add(
            new PrincipalIdentity
            {
                Id = Guid.NewGuid(),
                PrincipalId = cmd.PrincipalId,
                Provider = cmd.Provider,
                ExternalId = cmd.ExternalId,
            }
        );

        await principalRepository.SaveChangesAsync();

        return principal;
    }
}
