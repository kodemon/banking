using Banking.Shared.Exceptions;

namespace Banking.Principal;

/*
 |--------------------------------------------------------------------------------
 | Principal [Aggregate Root]
 |--------------------------------------------------------------------------------
 |
 | Represents an internal security principal. A principal is the canonical
 | identity within this system — external IDP identities are login methods
 | that bind to a principal, not the principal itself.
 |
 | Multiple IDP identities can be bound to a single principal, allowing a
 | user to authenticate via Zitadel, Microsoft, Google etc. and always
 | resolve to the same principal with the same roles and attributes.
 |
 */

internal class Principal
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }

    private readonly List<PrincipalIdentity> _identities = new();
    public IReadOnlyCollection<PrincipalIdentity> Identities => _identities.AsReadOnly();

    private readonly List<PrincipalRole> _roles = new();
    public IReadOnlyCollection<PrincipalRole> Roles => _roles.AsReadOnly();

    private readonly List<PrincipalAttribute> _attributes = new();
    public IReadOnlyCollection<PrincipalAttribute> Attributes => _attributes.AsReadOnly();

    private Principal() { } // Required by EF Core

    public static Principal Create() => new() { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };


    /*
     |--------------------------------------------------------------------------------
     | Identities
     |--------------------------------------------------------------------------------
     |
     | Each identity binds an external IDP provider + user ID to this principal.
     | The combination of (Provider, ExternalId) must be unique — the same external
     | identity cannot be bound to two different principals.
     |
     */

    public PrincipalIdentity AddIdentity(string provider, string externalId)
    {
        if (_identities.Any(i => i.Provider == provider && i.ExternalId == externalId))
            throw new AggregateConflictException(
                $"Identity '{provider}:{externalId}' is already bound to principal {Id}.");

        var identity = new PrincipalIdentity
        {
            Id = Guid.NewGuid(),
            PrincipalId = Id,
            Provider = provider,
            ExternalId = externalId,
            CreatedAt = DateTime.UtcNow
        };

        _identities.Add(identity);
        return identity;
    }

    public void RemoveIdentity(string provider, string externalId)
    {
        if (_identities.Count == 1)
            throw new AggregateConflictException(
                $"Cannot remove the last identity from principal {Id}. Add another identity first.");

        var existing = _identities.FirstOrDefault(i => i.Provider == provider && i.ExternalId == externalId)
            ?? throw new AggregateNotFoundException(
                $"Identity '{provider}:{externalId}' not found on principal {Id}.");

        _identities.Remove(existing);
    }

    /*
     |--------------------------------------------------------------------------------
     | Roles
     |--------------------------------------------------------------------------------
     */

    public PrincipalRole AddRole(string role)
    {
        if (_roles.Any(r => r.Role == role))
            throw new AggregateConflictException($"Role '{role}' is already assigned to principal {Id}.");

        var principalRole = new PrincipalRole
        {
            Id = Guid.NewGuid(),
            PrincipalId = Id,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _roles.Add(principalRole);
        return principalRole;
    }

    public void RemoveRole(string role)
    {
        var existing = _roles.FirstOrDefault(r => r.Role == role)
            ?? throw new AggregateNotFoundException($"Role '{role}' not found on principal {Id}.");

        _roles.Remove(existing);
    }

    /*
     |--------------------------------------------------------------------------------
     | Attributes
     |--------------------------------------------------------------------------------
     |
     | Attributes are domain-scoped. Two domains can use the same key without
     | conflict since they are always stored and retrieved per domain.
     |
     | SetAttribute uses upsert semantics — existing values are overwritten.
     | Validation against domain rules happens in PrincipalService before
     | this method is called.
     |
     */

    public PrincipalAttribute SetAttribute(string domain, string key, string value)
    {
        var existing = _attributes.FirstOrDefault(a => a.Domain == domain && a.Key == key);

        if (existing is not null)
        {
            existing.Value = value;
            return existing;
        }

        var attribute = new PrincipalAttribute
        {
            Id = Guid.NewGuid(),
            PrincipalId = Id,
            Domain = domain,
            Key = key,
            Value = value,
            CreatedAt = DateTime.UtcNow
        };

        _attributes.Add(attribute);
        return attribute;
    }

    public void RemoveAttribute(string domain, string key)
    {
        var existing = _attributes.FirstOrDefault(a => a.Domain == domain && a.Key == key)
            ?? throw new AggregateNotFoundException(
                $"Attribute '{key}' in domain '{domain}' not found on principal {Id}.");

        _attributes.Remove(existing);
    }

    /// <summary>
    /// Returns all stored attributes for a specific domain, keyed by attribute key.
    /// Passed to the domain resolver during principal resolution.
    /// </summary>
    public IReadOnlyDictionary<string, string> GetDomainAttributes(string domain) =>
        _attributes
            .Where(a => a.Domain == domain)
            .ToDictionary(a => a.Key, a => a.Value);
}

/*
 |--------------------------------------------------------------------------------
 | Aggregates
 |--------------------------------------------------------------------------------
 */

internal class PrincipalIdentity
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    /// <summary>IDP provider name e.g. "zitadel", "microsoft", "google".</summary>
    public required string Provider { get; init; }

    /// <summary>The user identifier issued by the provider.</summary>
    public required string ExternalId { get; init; }

    public DateTime CreatedAt { get; init; }
}

internal class PrincipalRole
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }
    public required string Role { get; init; }
    public DateTime CreatedAt { get; init; }
}

internal class PrincipalAttribute
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }
    public required string Domain { get; init; }
    public required string Key { get; init; }
    public required string Value { get; set; }
    public DateTime CreatedAt { get; init; }
}