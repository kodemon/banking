using Banking.Principals.Database;
using Banking.Principals.Repositories.Resources;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Repositories;

internal class PrincipalIdentityRepository(PrincipalDbContext context)
    : IPrincipalIdentityRepository
{
    public DbSet<PrincipalIdentity> identities = context.PrincipalIdentities;

    public async Task<bool> HasIdentityAsync(string provider, string externalId)
    {
        return await identities.AnyAsync(i => i.Provider == provider && i.ExternalId == externalId);
    }
}
