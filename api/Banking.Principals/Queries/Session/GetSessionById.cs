using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Queries;

internal record GetSessionByIdQuery(Guid SessionId) : IRequest<PrincipalSession?>;

internal sealed class GetSessionByIdHandler(IPrincipalSessionRepository repository)
    : IRequestHandler<GetSessionByIdQuery, PrincipalSession?>
{
    public Task<PrincipalSession?> Handle(GetSessionByIdQuery query, CancellationToken ct) =>
        repository.GetByIdAsync(query.SessionId, ct);
}
