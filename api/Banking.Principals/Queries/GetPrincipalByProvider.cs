using Banking.Principals.Repositories.Resources;
using MediatR;

namespace Banking.Principals.Queries;

internal record GetPrincipalByProviderQuery(string Provider, string ExternalId)
    : IRequest<Principal?>;

internal sealed class GetPrincipalByProviderHandler(IPrincipalRepository repository)
    : IRequestHandler<GetPrincipalByProviderQuery, Principal?>
{
    public Task<Principal?> Handle(GetPrincipalByProviderQuery query, CancellationToken ct) =>
        repository.GetByIdentityAsync(query.Provider, query.ExternalId);
}
