using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record CreatePrincipalCommand(string Provider, string ExternalId) : IRequest<Principal>;

internal sealed class CreatePrincipalHandler(
    IPrincipalRepository principalRepository,
    IPrincipalIdentityRepository identityRepository
) : IRequestHandler<CreatePrincipalCommand, Principal>
{
    public async Task<Principal> Handle(CreatePrincipalCommand cmd, CancellationToken ct)
    {
        if (await identityRepository.HasIdentityAsync(cmd.Provider, cmd.ExternalId))
        {
            throw new AggregateConflictException(
                $"Identity '{cmd.Provider}:{cmd.ExternalId}' is already bound to a principal."
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
                    Provider = cmd.Provider,
                    ExternalId = cmd.ExternalId,
                },
            ]),
        };

        await principalRepository.AddAsync(principal);
        await principalRepository.SaveChangesAsync();

        return principal;
    }
}
