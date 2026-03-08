using Banking.Shared.ValueObjects;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Commands;

internal record AddUserAddressCommand(Guid UserId, Address Address) : IRequest<UserAddress?>;

internal sealed class AddUserAddressHandler(IUserRepository repository)
    : IRequestHandler<AddUserAddressCommand, UserAddress?>
{
    public async Task<UserAddress?> Handle(AddUserAddressCommand message, CancellationToken token)
    {
        var user = await repository.GetByIdAsync(message.UserId);
        if (user is null)
        {
            return null;
        }

        var address = user.AddAddress(message.Address);

        await repository.SaveChangesAsync();

        return address;
    }
}
