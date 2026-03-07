using Banking.Principals.Database;
using Banking.Principals.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Repositories;

internal class PrincipalRepository(PrincipalContext context)
{
    public DbSet<Principal> principals = context.Principals;

    public async Task AddAsync(Principal principal)
    {
        await principals.AddAsync(principal);
    }

    public Task<Principal?> GetByIdAsync(Guid id)
    {
        return principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<Principal?> GetByIdentityAsync(string provider, string externalId)
    {
        return principals
            .Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p =>
                p.Identities.Any(i => i.Provider == provider && i.ExternalId == externalId)
            );
    }

    public Task DeleteByIdAsync(Guid id)
    {
        return principals.Where(p => p.Id == id).ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
