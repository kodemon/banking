using Banking.Atomic.Interfaces;
using Banking.Shared.Extensions;

namespace Banking.Principals.Atomics;

internal class AddPrinipalRoleRollback : IRollbackRegistration
{
    public required Guid PrincipalId { get; init; }
    public required string Role { get; init; }

    public static string TaskName => PrincipalAtomicTaskNames.AddPrincipalRole;

    public static void Register(IAtomicRollbackRegistry registry, IServiceProvider sp)
    {
        registry.Register<AddPrinipalRoleRollback>(TaskName, rollback => Rollback(rollback, sp));
    }

    private static async Task Rollback(AddPrinipalRoleRollback rollback, IServiceProvider sp)
    {
        await sp.RunInScope<IPrincipalRepository>(async repository =>
        {
            var principal = await repository.GetByIdAsync(rollback.PrincipalId);
            if (principal is null)
            {
                return;
            }
            var role = principal.Roles.Find(a => a.Role == rollback.Role);
            if (role is not null)
            {
                principal.Roles.Remove(role);
                await repository.SaveChangesAsync();
            }
        });
    }
}
