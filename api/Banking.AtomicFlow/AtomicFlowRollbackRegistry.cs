using System.Text.Json;

namespace Banking.AtomicFlow;

public interface IAtomicFlowRollbackRegistry
{
    void Register<TRollback>(string taskName, Func<TRollback, Task> handler);
    Task ExecuteAsync(string taskName, string rollbackJson);
    bool IsRegistered(string taskName);
}

public class AtomicFlowRollbackRegistry : IAtomicFlowRollbackRegistry
{
    private readonly Dictionary<string, (Type RollbackType, Func<object, Task> Handler)> _handlers =
        new();

    public void Register<TRollback>(string taskName, Func<TRollback, Task> handler)
    {
        if (_handlers.ContainsKey(taskName) == true)
        {
            throw new InvalidOperationException(
                $"A rollback handler for task '{taskName}' is already registered."
            );
        }
        _handlers[taskName] = (typeof(TRollback), async (obj) => await handler((TRollback)obj));
    }

    public async Task ExecuteAsync(string taskName, string rollbackJson)
    {
        if (_handlers.TryGetValue(taskName, out var entry) == false)
        {
            throw new InvalidOperationException(
                $"No rollback handler registered for task '{taskName}'."
            );
        }

        var rollbackData = JsonSerializer.Deserialize(rollbackJson, entry.RollbackType);
        if (rollbackData is null)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize rollback data for task '{taskName}'."
            );
        }

        await entry.Handler(rollbackData);
    }

    public bool IsRegistered(string taskName) => _handlers.ContainsKey(taskName);
}
