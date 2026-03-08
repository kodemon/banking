using Banking.Atomic.Interfaces;
using Banking.Atomic.Repositories.Resources;

namespace Banking.Atomic.Libraries;

public class Atomic(IAtomicRepository repository, IAtomicRollbackRegistry registry)
{
    private readonly IAtomicRepository _repository = repository;
    private readonly IAtomicRollbackRegistry _registry = registry;
    private readonly List<IAtomicTask> _committed = new();
    private AtomicState _state = AtomicState.Pending;

    public Guid FlowId { get; } = Guid.NewGuid();

    public async Task<TResult> Run<TResult, TRollback>(AtomicTask<TResult, TRollback> task)
    {
        if (_state != AtomicState.Pending)
            throw new InvalidOperationException(
                $"Atomic cannot run new tasks in state '{_state}'."
            );

        if (_registry.IsRegistered(task.Name) == false)
            throw new InvalidOperationException(
                $"No rollback handler registered for task '{task.Name}'. "
                    + $"Register a handler before running the flow."
            );

        try
        {
            await task.ExecuteCommitAsync();
        }
        catch (Exception ex)
        {
            _state = AtomicState.RollingBack;

            var rollbackErrors = new List<Exception>();

            foreach (var committed in Enumerable.Reverse(_committed))
            {
                try
                {
                    var records = await _repository.GetPendingAsync(FlowId);
                    var record = records.FirstOrDefault(r => r.TaskName == committed.Name);

                    if (record != null)
                    {
                        await _registry.ExecuteAsync(record.TaskName, record.RollbackJson);
                        await _repository.DeleteAsync(FlowId, committed.Name);
                    }
                }
                catch (Exception rollbackEx)
                {
                    // Record intentionally left in repository for deferred retry
                    rollbackErrors.Add(rollbackEx);
                }
            }

            _state = AtomicState.RolledBack;

            if (rollbackErrors.Any())
                throw new AggregateException(
                    $"Commit failed and {rollbackErrors.Count} rollback(s) also failed. "
                        + $"Pending rollbacks persisted under FlowId '{FlowId}'.",
                    rollbackErrors.Prepend(ex)
                );

            throw;
        }

        await _repository.SaveAsync(
            new AtomicRecord
            {
                Id = Guid.NewGuid(),
                FlowId = FlowId,
                TaskName = task.Name,
                RollbackJson = task.GetRollbackJson(),
                RollbackType = task.GetRollbackType(),
                CreatedAt = DateTime.UtcNow,
            }
        );

        _committed.Add(task);

        return task.Result;
    }

    public async Task Complete()
    {
        if (_state != AtomicState.Pending)
        {
            throw new InvalidOperationException($"Atomic cannot complete in state '{_state}'.");
        }
        await _repository.DeleteAllAsync(FlowId);
        _state = AtomicState.Completed;
    }
}

public enum AtomicState
{
    Pending,
    Completed,
    RollingBack,
    RolledBack,
}
