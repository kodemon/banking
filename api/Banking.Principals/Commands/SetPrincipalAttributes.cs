using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record SetPrincipalAttributeCommand(
    Guid PrincipalId,
    Dictionary<string, object> Attributes
) : IRequest<Principal>;

internal sealed class SetPrincipalAttributeHandler(IPrincipalRepository repository)
    : IRequestHandler<SetPrincipalAttributeCommand, Principal>
{
    public async Task<Principal> Handle(
        SetPrincipalAttributeCommand message,
        CancellationToken token
    )
    {
        var principal = await repository.GetByIdAsync(message.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{message.PrincipalId}' not found.");
        }

        principal.Attributes = message.Attributes;

        await repository.SaveChangesAsync();

        return principal;
    }
}
