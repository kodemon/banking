using Banking.AtomicFlow.Repositories.Resources;

namespace Banking.AtomicFlow.Interfaces;

public interface IAtomicFlowRepository
{
    Task SaveAsync(AtomicFlowRecord record);
    Task DeleteAsync(Guid flowId, string taskName);
    Task DeleteAllAsync(Guid flowId);
    Task<List<AtomicFlowRecord>> GetPendingAsync(Guid flowId);
    Task<List<AtomicFlowRecord>> GetAllPendingAsync();
}
