using Banking.AtomicFlow.Interfaces;
using Banking.AtomicFlow.Repositories.Resources;

namespace Banking.AtomicFlow;

public class AtomicFlow(IAtomicFlowRepository repository, IAtomicFlowRollbackRegistry registry)
{
    private readonly IAtomicFlowRepository _repository = repository;
    private readonly IAtomicFlowRollbackRegistry _registry = registry;
    private readonly List<IAtomicFlowTask> _committed = new();
    private AtomicFlowState _state = AtomicFlowState.Pending;

    public Guid FlowId { get; } = Guid.NewGuid();

    public async Task<TResult> Run<TResult, TRollback>(AtomicFlowTask<TResult, TRollback> task)
    {
        if (_state != AtomicFlowState.Pending)
            throw new InvalidOperationException(
                $"AtomicFlow cannot run new tasks in state '{_state}'."
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
            _state = AtomicFlowState.RollingBack;

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

            _state = AtomicFlowState.RolledBack;

            if (rollbackErrors.Any())
                throw new AggregateException(
                    $"Commit failed and {rollbackErrors.Count} rollback(s) also failed. "
                        + $"Pending rollbacks persisted under FlowId '{FlowId}'.",
                    rollbackErrors.Prepend(ex)
                );

            throw;
        }

        await _repository.SaveAsync(
            new AtomicFlowRecord
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
        if (_state != AtomicFlowState.Pending)
        {
            throw new InvalidOperationException($"AtomicFlow cannot complete in state '{_state}'.");
        }
        await _repository.DeleteAllAsync(FlowId);
        _state = AtomicFlowState.Completed;
    }
}

public enum AtomicFlowState
{
    Pending,
    Completed,
    RollingBack,
    RolledBack,
}
