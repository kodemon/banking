using Banking.Atomic;
using Banking.Atomic.Interfaces;
using Banking.Shared.Extensions;
using Banking.Users.Interfaces;

namespace Banking.Users.Atomics;

internal class CreateUserRollback : IRollbackRegistration
{
    public required Guid Id { get; init; }

    public static string TaskName => UserAtomicTaskNames.CreateUser;

    public static void Register(IAtomicRollbackRegistry registry, IServiceProvider sp)
    {
        registry.Register<CreateUserRollback>(TaskName, rollback => Rollback(rollback, sp));
    }

    private static async Task Rollback(CreateUserRollback rollback, IServiceProvider sp)
    {
        await sp.RunInScope<IUserRepository>(async repository =>
        {
            var user = await repository.GetByIdAsync(rollback.Id);
            if (user is null)
            {
                return;
            }
            await repository.DeleteAsync(user);
            await repository.SaveChangesAsync();
        });
    }
}
