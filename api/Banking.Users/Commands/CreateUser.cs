using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Commands;

internal record CreateUserCommand(string Email, Name Name, DateTime DateOfBirth) : IRequest<User>;

internal sealed class CreateUserHandler(IUserRepository userRepository)
    : IRequestHandler<CreateUserCommand, User>
{
    public async Task<User> Handle(CreateUserCommand cmd, CancellationToken ct)
    {
        if (await userRepository.ExistsByEmailAsync(cmd.Email))
        {
            throw new AggregateConflictException($"Email '{cmd.Email}' is already registered.");
        }

        var user = new User(cmd.Name, cmd.DateOfBirth);

        user.AddEmail(new Email(cmd.Email, EmailType.Primary));

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return user;
    }
}
