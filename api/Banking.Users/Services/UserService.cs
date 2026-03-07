using Banking.Shared.Exceptions;
using Banking.Users.DTO.Responses;

namespace Banking.Users;

/*
 |--------------------------------------------------------------------------------
 | UserService  [Query Only]
 |--------------------------------------------------------------------------------
 |
 | Read-only query surface for Banking.Api controllers. All mutations are
 | driven by events handled in Banking.Users.Handlers.UserEventHandlers.
 |
 */

internal class UserService(IUserRepository userRepository)
{
    public async Task<UserResponse> GetUserByIdAsync(Guid userId)
    {
        var user =
            await userRepository.GetByIdAsync(userId)
            ?? throw new AggregateNotFoundException($"User {userId} not found");
        return user.ToResponse();
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAllAsync();
        return users.Select(u => u.ToResponse());
    }
}
