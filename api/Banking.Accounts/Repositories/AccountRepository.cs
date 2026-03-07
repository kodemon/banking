using Banking.Accounts.Database;
using Banking.Accounts.Interfaces;
using Banking.Accounts.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Accounts.Repositories;

internal class AccountRepository(AccountsDbContext context) : IAccountRepository
{
    public DbSet<Account> accounts = context.Accounts;

    public async Task AddAsync(Account account)
    {
        await accounts.AddAsync(account);
    }

    public Task<Account?> GetByIdAsync(Guid id)
    {
        return accounts.Include(a => a.AccountHolders).FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Account>> GetAllByHolderIdAsync(Guid holderId)
    {
        return await accounts
            .Include(a => a.AccountHolders)
            .Where(a => a.AccountHolders.Any(h => h.HolderId == holderId))
            .ToListAsync();
    }

    public Task DeleteAsync(Account account)
    {
        accounts.Remove(account);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
