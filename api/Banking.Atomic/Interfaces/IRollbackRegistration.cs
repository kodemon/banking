namespace Banking.Atomic.Interfaces;

public interface IRollbackRegistration
{
    static abstract string TaskName { get; }
    static abstract void Register(IAtomicRollbackRegistry registry, IServiceProvider sp);
}
