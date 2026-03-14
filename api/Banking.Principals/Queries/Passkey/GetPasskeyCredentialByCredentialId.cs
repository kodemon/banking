using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Queries;

internal record GetPasskeyCredentialByCredentialIdQuery(string CredentialId)
    : IRequest<PrincipalPasskeyCredential?>;

internal sealed class GetPasskeyCredentialByCredentialIdHandler(
    IPrincipalPasskeyCredentialRepository repository
) : IRequestHandler<GetPasskeyCredentialByCredentialIdQuery, PrincipalPasskeyCredential?>
{
    public Task<PrincipalPasskeyCredential?> Handle(
        GetPasskeyCredentialByCredentialIdQuery query,
        CancellationToken ct
    ) => repository.GetByCredentialIdAsync(query.CredentialId, ct);
}
