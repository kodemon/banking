using Banking.Principals.Repositories.Resources;
using MediatR;

namespace Banking.Principals.Queries;

internal record GetPrincipalByIdQuery(Guid PrincipalId) : IRequest<Principal?>;

internal sealed class GetPrincipalByIdHandler(IPrincipalRepository repository)
    : IRequestHandler<GetPrincipalByIdQuery, Principal?>
{
    public Task<Principal?> Handle(GetPrincipalByIdQuery query, CancellationToken ct) =>
        repository.GetByIdAsync(query.PrincipalId);
}
