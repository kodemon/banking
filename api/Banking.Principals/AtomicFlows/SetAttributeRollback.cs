using Banking.AtomicFlow;
using Banking.AtomicFlow.Interfaces;
using Banking.Shared.Extensions;

namespace Banking.Principals.AtomicFlows;

internal class SetAttributeRollback : IRollbackRegistration
{
    public required Guid Id { get; init; }
    public required string Domain { get; init; }
    public required string Key { get; init; }

    public static string TaskName => PrincipalAtomicTaskNames.AddPrincipalAttribute;

    public static void Register(IAtomicFlowRollbackRegistry registry, IServiceProvider sp)
    {
        registry.Register<SetAttributeRollback>(TaskName, rollback => Rollback(rollback, sp));
    }

    private static async Task Rollback(SetAttributeRollback rollback, IServiceProvider sp)
    {
        await sp.RunInScope<IPrincipalRepository>(async repository =>
        {
            var principal = await repository.GetByIdAsync(rollback.Id);
            if (principal is null)
            {
                return;
            }

            var attribute = principal.Attributes.Find(a =>
                a.Domain == rollback.Domain && a.Key == rollback.Key
            );

            if (attribute is not null)
            {
                principal.Attributes.Remove(attribute);
                await repository.SaveChangesAsync();
            }
        });
    }
}
