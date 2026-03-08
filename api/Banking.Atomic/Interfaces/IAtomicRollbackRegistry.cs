namespace Banking.Atomic.Interfaces;

public interface IAtomicRollbackRegistry
{
    void Register<TRollback>(string taskName, Func<TRollback, Task> handler);
    Task ExecuteAsync(string taskName, string rollbackJson);
    bool IsRegistered(string taskName);
}
