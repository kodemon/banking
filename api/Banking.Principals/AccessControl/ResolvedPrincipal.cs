namespace Banking.Principal.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | ResolvedPrincipal
 |--------------------------------------------------------------------------------
 |
 | The fully assembled principal produced at request time. Contains the
 | principal's bound identities, roles, and a domain-keyed map of resolved
 | attribute instances.
 |
 | This is the structure passed to the Cerbos client when performing
 | policy checks — each domain's resolved attributes sit under their
 | domain key in the attr map.
 |
 | Example attr map shape:
 |   {
 |     "user":    UserAccessAttributes    { UserId, Email, Address },
 |     "account": AccountAccessAttributes { ... }
 |   }
 |
 */

internal record ResolvedPrincipal(
    Guid Id,
    IReadOnlyCollection<ResolvedIdentity> Identities,
    IReadOnlyCollection<string> Roles,
    IReadOnlyDictionary<string, object> Attributes
);

internal record ResolvedIdentity(string Provider, string ExternalId);