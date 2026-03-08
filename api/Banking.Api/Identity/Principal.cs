namespace Banking.Api.Identity;

public record ResolvedPrincipal(
    Guid Id,
    List<PrincipalIdentity> Identities,
    List<string> Roles,
    PrincipalAttributes Attributes
);

public record PrincipalIdentity(string Provider, string ExternalId);

public record PrincipalAttributes { }
