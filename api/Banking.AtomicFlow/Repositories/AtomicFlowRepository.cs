using Banking.AtomicFlow.Interfaces;
using Banking.AtomicFlow.Persistence;
using Banking.AtomicFlow.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.AtomicFlow.Repositories;

internal class AtomicFlowRepository(AtomicFlowDbContext context) : IAtomicFlowRepository
{
    public async Task SaveAsync(AtomicFlowRecord record)
    {
        context.AtomicFlowRecords.Add(record);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid flowId, string taskName)
    {
        var record = await context.AtomicFlowRecords.FirstOrDefaultAsync(r =>
            r.FlowId == flowId && r.TaskName == taskName
        );

        if (record != null)
        {
            context.AtomicFlowRecords.Remove(record);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync(Guid flowId)
    {
        var records = await context.AtomicFlowRecords.Where(r => r.FlowId == flowId).ToListAsync();

        if (records.Count > 0)
        {
            context.AtomicFlowRecords.RemoveRange(records);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<AtomicFlowRecord>> GetPendingAsync(Guid flowId)
    {
        return await context
            .AtomicFlowRecords.Where(r => r.FlowId == flowId)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AtomicFlowRecord>> GetAllPendingAsync()
    {
        return await context.AtomicFlowRecords.OrderBy(r => r.CreatedAt).ToListAsync();
    }
}
