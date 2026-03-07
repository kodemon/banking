namespace Banking.AtomicFlow.Interfaces;

public interface IRollbackRegistration
{
    static abstract string TaskName { get; }
    static abstract void Register(IAtomicFlowRollbackRegistry registry, IServiceProvider sp);
}
