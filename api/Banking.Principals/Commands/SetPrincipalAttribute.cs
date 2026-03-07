using Banking.Principals.AccessControl;
using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record SetPrincipalAttributeCommand(
    Guid PrincipalId,
    string Domain,
    string Key,
    string Value
) : IRequest<Principal>;

internal sealed class SetPrincipalAttributeHandler(
    IPrincipalRepository principalRepository,
    PrincipalResolver principalResolver
) : IRequestHandler<SetPrincipalAttributeCommand, Principal>
{
    public async Task<Principal> Handle(SetPrincipalAttributeCommand cmd, CancellationToken ct)
    {
        var resolver = principalResolver.GetResolver(cmd.Domain);
        if (resolver is null)
        {
            throw new AggregateNotFoundException(
                $"No attribute resolver registered for domain '{cmd.Domain}'."
            );
        }

        var validation = resolver.Validate(cmd.Key, cmd.Value);
        if (!validation.IsValid)
        {
            throw new AttributeValidationException(validation.Error!);
        }

        var principal = await principalRepository.GetByIdAsync(cmd.PrincipalId);
        if (principal is null)
        {
            throw new ResourceNotFoundException($"Principal '{cmd.PrincipalId}' not found.");
        }

        principal.Attributes.Add(
            new PrincipalAttribute
            {
                Id = Guid.NewGuid(),
                Domain = cmd.Domain,
                Key = cmd.Key,
                Value = cmd.Value,
            }
        );

        await principalRepository.SaveChangesAsync();

        return principal;
    }
}
