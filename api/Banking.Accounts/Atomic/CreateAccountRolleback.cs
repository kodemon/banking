using Banking.Accounts.Interfaces;
using Banking.Atomic.Interfaces;
using Banking.Shared.Extensions;

namespace Banking.Accounts.Atomics;

internal class CreateAccountRollback : IRollbackRegistration
{
    public required Guid AccountId { get; init; }

    public static string TaskName => AccountAtomicTaskNames.CreateAccount;

    public static void Register(IAtomicRollbackRegistry registry, IServiceProvider sp)
    {
        registry.Register<CreateAccountRollback>(TaskName, rollback => Rollback(rollback, sp));
    }

    private static async Task Rollback(CreateAccountRollback rollback, IServiceProvider sp)
    {
        await sp.RunInScope<IAccountRepository>(async repository =>
        {
            var account = await repository.GetByIdAsync(rollback.AccountId);
            if (account is null)
            {
                return;
            }
            await repository.DeleteAsync(account);
            await repository.SaveChangesAsync();
        });
    }
}
