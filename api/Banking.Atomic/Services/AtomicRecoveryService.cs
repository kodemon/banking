using Banking.Atomic.Interfaces;
using Banking.Atomic.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Banking.Atomic.Services;

internal class AtomicRecoveryService(
    IServiceScopeFactory scopeFactory,
    IAtomicRollbackRegistry registry,
    ILogger<AtomicRecoveryService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        // ### Database Guard
        // On clean repository state the database file and tables does not exist yet
        // so attempting to run this services at build time will crash the process.
        // This guard prevents the build from crashing as it runs on startup via
        // the Banking.Api <OpenApiGenerateDocuments>true</OpenApiGenerateDocuments>
        // trigger.

        var db = scope.ServiceProvider.GetRequiredService<AtomicDbContext>();
        try
        {
            var conn = db.Database.GetDbConnection();
            await conn.OpenAsync(cancellationToken);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name='AtomicRecords'";
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            if (Convert.ToInt32(result) == 0)
            {
                return;
            }
        }
        catch
        {
            return;
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }

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

                // ### Rollback
                // Execute the rollback task.

                await registry.ExecuteAsync(record.TaskName, record.RollbackJson);

                // ### Clean
                // Remove the rollback task from the atomic repository.

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
