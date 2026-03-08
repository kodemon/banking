using Banking.Atomic.Repositories.Resources;

namespace Banking.Atomic.Interfaces;

public interface IAtomicRepository
{
    Task SaveAsync(AtomicRecord record);
    Task DeleteAsync(Guid flowId, string taskName);
    Task DeleteAllAsync(Guid flowId);
    Task<List<AtomicRecord>> GetPendingAsync(Guid flowId);
    Task<List<AtomicRecord>> GetAllPendingAsync();
}
