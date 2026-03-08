using Banking.Principals.Database;
using Banking.Principals.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Repositories;

internal class PrincipalRepository(PrincipalDbContext context) : IPrincipalRepository
{
    public DbSet<Principal> principals = context.Principals;

    public async Task AddAsync(Principal principal)
    {
        await principals.AddAsync(principal);
    }

    public async Task<Principal?> GetByIdAsync(Guid id)
    {
        return await principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Principal?> GetByIdentityAsync(string provider, string externalId)
    {
        return await principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p =>
                p.Identities.Any(i => i.Provider == provider && i.ExternalId == externalId)
            );
    }

    public async Task<IEnumerable<Principal>> GetAllAsync()
    {
        return await principals.Include(p => p.Identities).Include(p => p.Roles).ToListAsync();
    }

    public Task DeleteAsync(Principal principal)
    {
        principals.Remove(principal);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
