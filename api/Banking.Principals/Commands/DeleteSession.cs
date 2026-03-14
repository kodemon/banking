using Banking.Principals.Interfaces;
using MediatR;

namespace Banking.Principals.Commands;

internal record DeleteSessionCommand(Guid SessionId) : IRequest;

internal sealed class DeleteSessionHandler(ISessionRepository repository)
    : IRequestHandler<DeleteSessionCommand>
{
    public async Task Handle(DeleteSessionCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.SessionId, ct);
        await repository.SaveChangesAsync(ct);
    }
}
