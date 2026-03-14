using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Commands;

internal record RegisterPasskeyCredentialCommand(
    Guid PrincipalId,
    string CredentialId,
    byte[] PublicKey,
    uint SignCount,
    string Name,
    Guid AaGuid
) : IRequest<PrincipalPasskeyCredential>;

internal sealed class RegisterPasskeyCredentialHandler(IPasskeyCredentialRepository repository)
    : IRequestHandler<RegisterPasskeyCredentialCommand, PrincipalPasskeyCredential>
{
    public async Task<PrincipalPasskeyCredential> Handle(
        RegisterPasskeyCredentialCommand command,
        CancellationToken ct
    )
    {
        var credential = new PrincipalPasskeyCredential(
            command.PrincipalId,
            command.CredentialId,
            command.PublicKey,
            command.SignCount,
            command.Name,
            command.AaGuid
        );

        await repository.AddAsync(credential, ct);
        await repository.SaveChangesAsync(ct);

        return credential;
    }
}
