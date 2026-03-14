using Banking.Principals.Database.Models;

namespace Banking.Principals;

internal interface IPrincipalRepository
{
    Task AddAsync(Principal principal);

    Task<Principal?> GetByIdAsync(Guid id);
    Task<Principal?> GetByIdentityAsync(string provider, string externalId);
    Task<IEnumerable<Principal>> GetAllAsync();

    Task DeleteAsync(Principal principal);

    Task SaveChangesAsync();
}
