using Banking.Principals.Repositories.Resources;
using Banking.Shared.Exceptions;
using MediatR;

namespace Banking.Principals.Commands;

internal record CreatePrincipalCommand(string Provider, string ExternalId) : IRequest<Principal>;

internal sealed class CreatePrincipalHandler(IPrincipalRepository repository)
    : IRequestHandler<CreatePrincipalCommand, Principal>
{
    public async Task<Principal> Handle(CreatePrincipalCommand message, CancellationToken ct)
    {
        var principal = await repository.GetByIdentityAsync(message.Provider, message.ExternalId);
        if (principal is not null)
        {
            throw new AggregateConflictException(
                $"Identity '{message.Provider}:{message.ExternalId}' is already bound to a principal."
            );
        }

        principal = new Principal();

        principal.AddIdentity(message.Provider, message.ExternalId);
        principal.AddRole("user");

        await repository.AddAsync(principal);
        await repository.SaveChangesAsync();

        return principal;
    }
}
