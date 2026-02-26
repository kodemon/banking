using Banking.Principal.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principal;

internal class PrincipalRepository(PrincipalDbContext context) : IPrincipalRepository
{
    public async Task AddAsync(Principal principal)
    {
        await context.Principals.AddAsync(principal);
    }

    public async Task<Principal?> GetByIdAsync(Guid id)
    {
        return await context.Principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Principal?> GetByIdentityAsync(string provider, string externalId)
    {
        return await context.Principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p =>
                p.Identities.Any(i => i.Provider == provider && i.ExternalId == externalId));
    }

    public async Task<IEnumerable<Principal>> GetAllAsync()
    {
        return await context.Principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .ToListAsync();
    }

    public async Task<bool> IdentityExistsAsync(string provider, string externalId)
    {
        return await context.PrincipalIdentities
            .AnyAsync(i => i.Provider == provider && i.ExternalId == externalId);
    }

    public Task DeleteAsync(Principal principal)
    {
        context.Principals.Remove(principal);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}