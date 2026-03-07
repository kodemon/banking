using MediatR;

namespace Banking.Principals.Commands;

internal record DeletePrincipalCommand(Guid PrincipalId) : IRequest;

internal sealed class DeletePrincipalHandler(IPrincipalRepository principalRepository)
    : IRequestHandler<DeletePrincipalCommand>
{
    public async Task Handle(DeletePrincipalCommand cmd, CancellationToken ct)
    {
        var principal = await principalRepository.GetByIdAsync(cmd.PrincipalId);
        if (principal is null)
        {
            return;
        }

        await principalRepository.DeleteAsync(principal);
        await principalRepository.SaveChangesAsync();
    }
}
