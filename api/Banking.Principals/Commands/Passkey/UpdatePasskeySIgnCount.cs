using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Commands;

internal record UpdatePasskeySignCountCommand(string CredentialId, uint SignCount) : IRequest;

internal sealed class UpdatePasskeySignCountHandler(
    IPrincipalPasskeyCredentialRepository repository
) : IRequestHandler<UpdatePasskeySignCountCommand>
{
    public async Task Handle(UpdatePasskeySignCountCommand command, CancellationToken ct)
    {
        var credential = await repository.GetByCredentialIdAsync(command.CredentialId, ct);
        if (credential is null)
        {
            return;
        }

        credential.SignCount = command.SignCount;
        credential.LastUsedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(ct);
    }
}
