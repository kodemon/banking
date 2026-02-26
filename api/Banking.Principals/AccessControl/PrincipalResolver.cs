using Banking.Shared.AccessControl;

namespace Banking.Principal.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | PrincipalResolver
 |--------------------------------------------------------------------------------
 |
 | Assembles a ResolvedPrincipal at request time by:
 |
 |   1. Mapping all bound IDP identities
 |   2. Grouping the principal's stored attributes by domain
 |   3. Passing each domain's raw key/value slice to its registered resolver
 |   4. Collecting the typed attribute instances into the attr map
 |
 | Domains without stored attributes still participate — their resolver
 | returns a default instance, ensuring policy checks always have a
 | consistent attribute shape to work with.
 |
 */

internal class PrincipalResolver(IEnumerable<IAccessAttributeResolver> resolvers)
{
    private readonly IReadOnlyDictionary<string, IAccessAttributeResolver> _resolvers =
        resolvers.ToDictionary(r => r.Domain, StringComparer.OrdinalIgnoreCase);

    public ResolvedPrincipal Resolve(Principal principal)
    {
        var identities = principal.Identities
            .Select(i => new ResolvedIdentity(i.Provider, i.ExternalId))
            .ToList()
            .AsReadOnly();

        var attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var (domain, resolver) in _resolvers)
        {
            var rawValues = principal.GetDomainAttributes(domain);
            attributes[domain] = resolver.Resolve(rawValues);
        }

        return new ResolvedPrincipal(
            principal.Id,
            identities,
            principal.Roles.Select(r => r.Role).ToList().AsReadOnly(),
            attributes
        );
    }

    /// <summary>
    /// Returns the resolver for a given domain, or null if no domain is registered.
    /// Used by PrincipalService to validate attribute updates.
    /// </summary>
    public IAccessAttributeResolver? GetResolver(string domain) =>
        _resolvers.TryGetValue(domain, out var resolver) ? resolver : null;
}