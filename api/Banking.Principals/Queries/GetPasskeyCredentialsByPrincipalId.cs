using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Queries;

internal record GetPasskeyCredentialsByPrincipalIdQuery(Guid PrincipalId)
    : IRequest<List<PrincipalPasskeyCredential>>;

internal sealed class GetPasskeyCredentialsByPrincipalIdHandler(
    IPasskeyCredentialRepository repository
) : IRequestHandler<GetPasskeyCredentialsByPrincipalIdQuery, List<PrincipalPasskeyCredential>>
{
    public Task<List<PrincipalPasskeyCredential>> Handle(
        GetPasskeyCredentialsByPrincipalIdQuery query,
        CancellationToken ct
    ) => repository.GetByPrincipalIdAsync(query.PrincipalId, ct);
}
