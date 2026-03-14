using Banking.Principals.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Database.Repositories;

internal class PrincipalIdentityRepository(PrincipalDbContext context)
    : IPrincipalIdentityRepository
{
    public DbSet<PrincipalIdentity> identities = context.PrincipalIdentities;

    public async Task<bool> HasIdentityAsync(string provider, string externalId)
    {
        return await identities.AnyAsync(i => i.Provider == provider && i.ExternalId == externalId);
    }
}
