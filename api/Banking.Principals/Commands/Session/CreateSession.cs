using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Commands;

internal record CreateSessionCommand(Guid PrincipalId) : IRequest<PrincipalSession>;

internal sealed class CreateSessionHandler(IPrincipalSessionRepository repository)
    : IRequestHandler<CreateSessionCommand, PrincipalSession>
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(30);

    public async Task<PrincipalSession> Handle(CreateSessionCommand command, CancellationToken ct)
    {
        var session = new PrincipalSession(command.PrincipalId, SessionLifetime);
        await repository.AddAsync(session, ct);
        await repository.SaveChangesAsync(ct);
        return session;
    }
}
