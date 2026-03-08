using Banking.Atomic.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Banking.Atomic.Services;

/*
 |--------------------------------------------------------------------------------
 | AtomicRecoveryService
 |--------------------------------------------------------------------------------
 |
 | Runs once on startup to replay any rollbacks that were persisted but never
 | executed — for example when a flow partially committed before the process
 | crashed or was killed.
 |
 | Executes pending records in ascending CreatedAt order so that multi-step
 | flows are rolled back in the correct sequence.
 |
 | Records that fail to roll back are left in the database for manual
 | inspection and retry — the same behaviour as a mid-flight rollback failure
 | in Atomic.Run().
 |
 */

internal class AtomicRecoveryService(
    IServiceScopeFactory scopeFactory,
    IAtomicRollbackRegistry registry,
    ILogger<AtomicRecoveryService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAtomicRepository>();

        var pending = await repository.GetAllPendingAsync();

        if (pending.Count == 0)
        {
            return;
        }

        logger.LogWarning(
            "Atomic recovery: found {Count} pending rollback(s) from a previous run",
            pending.Count
        );

        foreach (var record in pending)
        {
            try
            {
                logger.LogInformation(
                    "Replaying rollback for task '{TaskName}' (FlowId: {FlowId})",
                    record.TaskName,
                    record.FlowId
                );

                await registry.ExecuteAsync(record.TaskName, record.RollbackJson);
                await repository.DeleteAsync(record.FlowId, record.TaskName);

                logger.LogInformation(
                    "Rollback completed for task '{TaskName}' (FlowId: {FlowId})",
                    record.TaskName,
                    record.FlowId
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Rollback failed for task '{TaskName}' (FlowId: {FlowId}) — record left for manual retry",
                    record.TaskName,
                    record.FlowId
                );
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
