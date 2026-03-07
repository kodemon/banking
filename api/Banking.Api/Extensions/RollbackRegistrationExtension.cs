using Banking.AtomicFlow;
using Banking.AtomicFlow.Interfaces;

namespace Banking.Api;

internal static class RollbackRegistrationExtensions
{
    public static IApplicationBuilder UseRollbackRegistrations(this IApplicationBuilder app)
    {
        var pending = app.ApplicationServices.GetServices<PendingRollbackRegistrations>();
        var registry = app.ApplicationServices.GetRequiredService<AtomicFlowRollbackRegistry>();

        foreach (var batch in pending)
        {
            foreach (var type in batch.Types)
            {
                var register = type.GetMethod(
                    nameof(IRollbackRegistration.Register),
                    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public
                );
                register!.Invoke(null, [registry, app.ApplicationServices]);
            }
        }

        return app;
    }
}
