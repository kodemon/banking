using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record RemovePrincipalAttributeCommand(Guid PrincipalId, string Domain, string Key)
    : IRequest<Principal>;

internal sealed class RemovePrincipalAttributeHandler(IPrincipalRepository principalRepository)
    : IRequestHandler<RemovePrincipalAttributeCommand, Principal>
{
    public async Task<Principal> Handle(RemovePrincipalAttributeCommand cmd, CancellationToken ct)
    {
        var principal = await principalRepository.GetByIdAsync(cmd.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{cmd.PrincipalId}' not found.");
        }

        var attribute = principal.Attributes.Find(a => a.Domain == cmd.Domain && a.Key == cmd.Key);

        if (attribute is not null)
        {
            principal.Attributes.Remove(attribute);
            await principalRepository.SaveChangesAsync();
        }

        return principal;
    }
}
