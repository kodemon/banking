namespace Banking.Principal;

internal interface IPrincipalRepository
{
    Task AddAsync(Principal principal);

    Task<Principal?> GetByIdAsync(Guid id);
    Task<Principal?> GetByIdentityAsync(string provider, string externalId);
    Task<IEnumerable<Principal>> GetAllAsync();

    Task<bool> IdentityExistsAsync(string provider, string externalId);

    Task DeleteAsync(Principal principal);

    Task SaveChangesAsync();
}