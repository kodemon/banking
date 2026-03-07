using Banking.Shared.Exceptions;
using Banking.Users.Interfaces;
using Banking.Users.Repositories.Resources;
using MediatR;

namespace Banking.Users.Queries;

internal record GetUserByIdQuery(Guid UserId) : IRequest<User>;

internal sealed class GetUserByIdHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, User>
{
    public async Task<User> Handle(GetUserByIdQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.UserId);
        if (user is null)
        {
            throw new ResourceNotFoundException($"User '{query.UserId}' not found.");
        }
        return user;
    }
}

internal record GetAllUsersQuery : IRequest<IEnumerable<User>>;

internal sealed class GetAllUsersHandler(IUserRepository userRepository)
    : IRequestHandler<GetAllUsersQuery, IEnumerable<User>>
{
    public Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken ct) =>
        userRepository.GetAllAsync();
}
