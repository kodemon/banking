using Banking.Atomic.Interfaces;
using Banking.Atomic.Persistence;
using Banking.Atomic.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Atomic.Repositories;

internal class AtomicRepository(AtomicDbContext context) : IAtomicRepository
{
    public async Task SaveAsync(AtomicRecord record)
    {
        context.AtomicRecords.Add(record);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid flowId, string taskName)
    {
        var record = await context.AtomicRecords.FirstOrDefaultAsync(r =>
            r.FlowId == flowId && r.TaskName == taskName
        );

        if (record != null)
        {
            context.AtomicRecords.Remove(record);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync(Guid flowId)
    {
        var records = await context.AtomicRecords.Where(r => r.FlowId == flowId).ToListAsync();

        if (records.Count > 0)
        {
            context.AtomicRecords.RemoveRange(records);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<AtomicRecord>> GetPendingAsync(Guid flowId)
    {
        return await context
            .AtomicRecords.Where(r => r.FlowId == flowId)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<AtomicRecord>> GetAllPendingAsync()
    {
        return await context.AtomicRecords.OrderBy(r => r.CreatedAt).ToListAsync();
    }
}
