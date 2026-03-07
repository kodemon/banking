using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Commands;

internal record AddUserEmailCommand(Guid UserId, Email Email) : IRequest<User>;

internal sealed class AddUserEmailHandler(IUserRepository userRepository)
    : IRequestHandler<AddUserEmailCommand, User>
{
    public async Task<User> Handle(AddUserEmailCommand cmd, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(cmd.UserId);
        if (user is null)
        {
            throw new ResourceNotFoundException($"User '{cmd.UserId}' not found.");
        }

        user.AddEmail(cmd.Email);

        await userRepository.SaveChangesAsync();

        return user;
    }
}
