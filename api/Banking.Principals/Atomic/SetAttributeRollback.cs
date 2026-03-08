using Banking.Atomic;
using Banking.Atomic.Interfaces;
using Banking.Shared.Extensions;

namespace Banking.Principals.Atomics;

internal class SetAttributeRollback : IRollbackRegistration
{
    public required Guid PrincipalId { get; init; }
    public required Dictionary<string, object> Attributes { get; init; }

    public static string TaskName => PrincipalAtomicTaskNames.AddPrincipalAttribute;

    public static void Register(IAtomicRollbackRegistry registry, IServiceProvider sp)
    {
        registry.Register<SetAttributeRollback>(TaskName, rollback => Rollback(rollback, sp));
    }

    private static async Task Rollback(SetAttributeRollback rollback, IServiceProvider sp)
    {
        await sp.RunInScope<IPrincipalRepository>(async repository =>
        {
            var principal = await repository.GetByIdAsync(rollback.PrincipalId);
            if (principal is null)
            {
                return;
            }

            principal.Attributes = rollback.Attributes;

            await repository.SaveChangesAsync();
        });
    }
}
