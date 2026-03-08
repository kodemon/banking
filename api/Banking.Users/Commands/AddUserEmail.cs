using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Commands;

internal record AddUserEmailCommand(Guid UserId, Email Email) : IRequest<UserEmail?>;

internal sealed class AddUserEmailHandler(IUserRepository repository)
    : IRequestHandler<AddUserEmailCommand, UserEmail?>
{
    public async Task<UserEmail?> Handle(AddUserEmailCommand message, CancellationToken token)
    {
        var user = await repository.GetByIdAsync(message.UserId);
        if (user is null)
        {
            return null;
        }

        var email = user.AddEmail(message.Email);

        await repository.SaveChangesAsync();

        return email;
    }
}
