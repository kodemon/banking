using Banking.Shared.Exceptions;
using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Commands;

internal record AddUserAddressCommand(Guid UserId, Address Address) : IRequest<User>;

internal sealed class AddUserAddressHandler(IUserRepository userRepository)
    : IRequestHandler<AddUserAddressCommand, User>
{
    public async Task<User> Handle(AddUserAddressCommand cmd, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(cmd.UserId);
        if (user is null)
        {
            throw new ResourceNotFoundException($"User '{cmd.UserId}' not found.");
        }

        user.AddAddress(cmd.Address);

        await userRepository.SaveChangesAsync();

        return user;
    }
}
