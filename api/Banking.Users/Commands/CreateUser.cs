using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Commands;

internal record CreateUserCommand(
    Guid OwnerId,
    string EmailAddress,
    Name Name,
    DateTime DateOfBirth
) : IRequest<User>;

internal sealed class CreateUserHandler(IUserRepository repository)
    : IRequestHandler<CreateUserCommand, User>
{
    public async Task<User> Handle(CreateUserCommand message, CancellationToken token)
    {
        var user = await repository.GetByOwnerIdAsync(message.OwnerId);
        if (user is not null)
        {
            throw new ResourceConflictException($"Owner already has a user.");
        }

        user = await repository.GetByEmailAsync(message.EmailAddress);
        if (user is not null)
        {
            throw new AggregateConflictException(
                $"Email '{message.EmailAddress}' is already registered."
            );
        }

        user = new User(message.OwnerId, message.Name, message.DateOfBirth);

        user.AddEmail(new Email(message.EmailAddress, EmailType.Primary));

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();

        return user;
    }
}
