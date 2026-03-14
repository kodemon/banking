using MediatR;

namespace Banking.Principals.Commands;

internal record DeletePrincipalCommand(Guid PrincipalId) : IRequest;

internal sealed class DeletePrincipalHandler(IPrincipalRepository repository)
    : IRequestHandler<DeletePrincipalCommand>
{
    public async Task Handle(DeletePrincipalCommand cmd, CancellationToken ct)
    {
        var principal = await repository.GetByIdAsync(cmd.PrincipalId);
        if (principal is null)
        {
            return;
        }

        await repository.DeleteAsync(principal);
        await repository.SaveChangesAsync();
    }
}
