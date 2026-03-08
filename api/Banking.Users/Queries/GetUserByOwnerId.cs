using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Principals.Queries;

internal record GetUserByOwnerIdQuery(Guid OwnerId) : IRequest<User?>;

internal sealed class GetUserByIdHandler(IUserRepository repository)
    : IRequestHandler<GetUserByOwnerIdQuery, User?>
{
    public Task<User?> Handle(GetUserByOwnerIdQuery message, CancellationToken token) =>
        repository.GetByOwnerIdAsync(message.OwnerId);
}
