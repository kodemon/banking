namespace Banking.Atomic.Interfaces;

public interface IAtomicTask
{
    string Name { get; }
    Task ExecuteCommitAsync();
    string GetRollbackJson();
    string GetRollbackType();
}
