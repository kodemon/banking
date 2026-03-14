using Banking.Principals.Database.Models;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record AddPrincipalRoleCommand(Guid PrincipalId, string Role) : IRequest<PrincipalRole>;

internal sealed class AddPrincipalRoleHandler(IPrincipalRepository repository)
    : IRequestHandler<AddPrincipalRoleCommand, PrincipalRole>
{
    public async Task<PrincipalRole> Handle(
        AddPrincipalRoleCommand message,
        CancellationToken token
    )
    {
        var principal = await repository.GetByIdAsync(message.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{message.PrincipalId}' not found.");
        }

        var role = principal.GetRole(message.Role);
        if (role is not null)
        {
            return role;
        }

        role = principal.AddRole(message.Role);

        await repository.SaveChangesAsync();

        return role;
    }
}
