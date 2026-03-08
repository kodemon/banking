using Banking.Users.Interfaces;
using MediatR;

namespace Banking.Users.Commands;

internal record DeleteUserCommand(Guid UserId) : IRequest;

internal sealed class DeleteUserHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand cmd, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(cmd.UserId);
        if (user is null)
        {
            return;
        }

        await userRepository.DeleteAsync(user);
        await userRepository.SaveChangesAsync();
    }
}
