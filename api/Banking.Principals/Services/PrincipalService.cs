using Banking.Principal.AccessControl;
using Banking.Principal.DTO.Requests;
using Banking.Principal.DTO.Responses;
using Banking.Principal.Mappings;
using Banking.Shared.Exceptions;

namespace Banking.Principal;

/*
 |--------------------------------------------------------------------------------
 | PrincipalService
 |--------------------------------------------------------------------------------
 |
 | Orchestrates principal management and attribute resolution.
 |
 | Attribute updates are routed through the domain's resolver for validation
 | before persisting. The domain is the sole authority on correctness —
 | this service only enforces that a resolver exists for the given domain.
 |
 */

internal class PrincipalService(
    IPrincipalRepository principalRepository,
    PrincipalResolver principalResolver)
{
    /*
     |--------------------------------------------------------------------------------
     | Principal
     |--------------------------------------------------------------------------------
     */

    public async Task<PrincipalResponse> CreatePrincipalAsync(CreatePrincipalRequest request)
    {
        if (await principalRepository.IdentityExistsAsync(request.Provider, request.ExternalId))
            throw new AggregateConflictException(
                $"Identity '{request.Provider}:{request.ExternalId}' is already bound to a principal.");

        var principal = Principal.Create();
        principal.AddIdentity(request.Provider, request.ExternalId);

        await principalRepository.AddAsync(principal);
        await principalRepository.SaveChangesAsync();

        return principal.ToResponse();
    }

    public async Task<PrincipalResponse> GetByIdAsync(Guid id)
    {
        var principal = await GetPrincipal(id);
        return principal.ToResponse();
    }

    public async Task<PrincipalResponse> GetByIdentityAsync(string provider, string externalId)
    {
        var principal = await principalRepository.GetByIdentityAsync(provider, externalId)
            ?? throw new AggregateNotFoundException(
                $"No principal found for identity '{provider}:{externalId}'.");

        return principal.ToResponse();
    }

    public async Task<IEnumerable<PrincipalResponse>> GetAllAsync()
    {
        var principals = await principalRepository.GetAllAsync();
        return principals.Select(p => p.ToResponse());
    }

    public async Task DeleteAsync(Guid id)
    {
        var principal = await GetPrincipal(id);
        await principalRepository.DeleteAsync(principal);
        await principalRepository.SaveChangesAsync();
    }

    /*
     |--------------------------------------------------------------------------------
     | Identities
     |--------------------------------------------------------------------------------
     */

    public async Task<PrincipalResponse> AddIdentityAsync(Guid id, AddIdentityRequest request)
    {
        if (await principalRepository.IdentityExistsAsync(request.Provider, request.ExternalId))
            throw new AggregateConflictException(
                $"Identity '{request.Provider}:{request.ExternalId}' is already bound to a principal.");

        var principal = await GetPrincipal(id);
        principal.AddIdentity(request.Provider, request.ExternalId);
        await principalRepository.SaveChangesAsync();

        return principal.ToResponse();
    }

    public async Task<PrincipalResponse> RemoveIdentityAsync(Guid id, RemoveIdentityRequest request)
    {
        var principal = await GetPrincipal(id);
        principal.RemoveIdentity(request.Provider, request.ExternalId);
        await principalRepository.SaveChangesAsync();

        return principal.ToResponse();
    }

    /*
     |--------------------------------------------------------------------------------
     | Roles
     |--------------------------------------------------------------------------------
     */

    public async Task<PrincipalResponse> AddRoleAsync(Guid id, AddRoleRequest request)
    {
        var principal = await GetPrincipal(id);
        principal.AddRole(request.Role);
        await principalRepository.SaveChangesAsync();
        return principal.ToResponse();
    }

    public async Task<PrincipalResponse> RemoveRoleAsync(Guid id, string role)
    {
        var principal = await GetPrincipal(id);
        principal.RemoveRole(role);
        await principalRepository.SaveChangesAsync();
        return principal.ToResponse();
    }

    /*
     |--------------------------------------------------------------------------------
     | Attributes
     |--------------------------------------------------------------------------------
     */

    public async Task<PrincipalResponse> SetAttributeAsync(Guid id, SetAttributeRequest request)
    {
        var resolver = principalResolver.GetResolver(request.Domain)
            ?? throw new AggregateNotFoundException(
                $"No attribute resolver registered for domain '{request.Domain}'.");

        var validation = resolver.Validate(request.Key, request.Value);

        if (!validation.IsValid)
            throw new AttributeValidationException(validation.Error!);

        var principal = await GetPrincipal(id);
        principal.SetAttribute(request.Domain, request.Key, request.Value);
        await principalRepository.SaveChangesAsync();

        return principal.ToResponse();
    }

    public async Task<PrincipalResponse> RemoveAttributeAsync(Guid id, RemoveAttributeRequest request)
    {
        var principal = await GetPrincipal(id);
        principal.RemoveAttribute(request.Domain, request.Key);
        await principalRepository.SaveChangesAsync();
        return principal.ToResponse();
    }

    /*
     |--------------------------------------------------------------------------------
     | Resolution
     |--------------------------------------------------------------------------------
     |
     | Resolves a principal by provider + externalId at request time.
     | Called internally by the Cerbos middleware — not exposed via HTTP.
     |
     */

    public async Task<ResolvedPrincipal> ResolveAsync(string provider, string externalId)
    {
        var principal = await principalRepository.GetByIdentityAsync(provider, externalId)
            ?? throw new AggregateNotFoundException(
                $"No principal found for identity '{provider}:{externalId}'.");

        return principalResolver.Resolve(principal);
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers
     |--------------------------------------------------------------------------------
     */

    private async Task<Principal> GetPrincipal(Guid id)
    {
        return await principalRepository.GetByIdAsync(id)
            ?? throw new AggregateNotFoundException($"Principal {id} not found.");
    }
}