using Banking.Principals.Commands;
using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | PrincipalProvisioner
 |--------------------------------------------------------------------------------
 |
 | Internal helper used exclusively by ZitadelClaimsTransformation to resolve
 | or provision a principal during the authentication pipeline.
 |
 | IMediator is not used here because claims transformation runs inside the
 | authentication middleware, before a proper request context is established.
 | Dispatching through the mediator pipeline would add overhead and pipeline
 | behaviors (e.g. validation, logging) that aren't appropriate at this stage.
 |
 | Handlers are called directly via the repositories instead.
 |
 */

internal class PrincipalProvisioner(
    IPrincipalRepository principalRepository,
    IPrincipalIdentityRepository identityRepository,
    PrincipalResolver principalResolver
)
{
    public async Task<ResolvedPrincipal> ResolveAsync(string provider, string externalId)
    {
        var principal = await principalRepository.GetByIdentityAsync(provider, externalId);
        if (principal is null)
        {
            throw new AggregateNotFoundException(
                $"No principal found for identity '{provider}:{externalId}'."
            );
        }

        return principalResolver.Resolve(principal);
    }

    public async Task<Principal> CreateAsync(string provider, string externalId)
    {
        if (await identityRepository.HasIdentityAsync(provider, externalId))
        {
            throw new AggregateConflictException(
                $"Identity '{provider}:{externalId}' is already bound to a principal."
            );
        }

        var principalId = Guid.NewGuid();

        var principal = new Principal
        {
            Id = principalId,
            Identities = new List<PrincipalIdentity>([
                new PrincipalIdentity
                {
                    Id = Guid.NewGuid(),
                    PrincipalId = principalId,
                    Provider = provider,
                    ExternalId = externalId,
                },
            ]),
        };

        await principalRepository.AddAsync(principal);
        await principalRepository.SaveChangesAsync();

        return principal;
    }
}
