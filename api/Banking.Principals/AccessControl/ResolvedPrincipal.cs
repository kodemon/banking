namespace Banking.Principals.AccessControl;

public record ResolvedPrincipal(
    Guid Id,
    IReadOnlyCollection<ResolvedIdentity> Identities,
    IReadOnlyCollection<string> Roles,
    IReadOnlyDictionary<string, object> Attributes
);

public record ResolvedIdentity(string Provider, string ExternalId);
