using Banking.Accounts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Accounts;

internal class AccountRepository(AccountsDbContext context) : IAccountRepository
{
    public async Task AddAsync(Account account)
    {
        await context.Accounts.AddAsync(account);
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await context.Accounts
            .Include(a => a.PersonalHolders)
            .Include(a => a.BusinessHolders)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Account>> GetAllByUserAsync(Guid userId)
    {
        return await context.Accounts
            .Include(a => a.PersonalHolders)
            .Include(a => a.BusinessHolders)
            .Where(a => a.PersonalHolders.Any(h => h.UserId == userId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Account>> GetAllByBusinessAsync(Guid businessId)
    {
        return await context.Accounts
            .Include(a => a.PersonalHolders)
            .Include(a => a.BusinessHolders)
            .Where(a => a.BusinessHolders.Any(h => h.BusinessId == businessId))
            .ToListAsync();
    }

    public Task DeleteAsync(Account account)
    {
        context.Accounts.Remove(account);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}