namespace Banking.Principal.DTO.Responses;

/*
 |--------------------------------------------------------------------------------
 | Responses
 |--------------------------------------------------------------------------------
 */

/// <summary>Represents a security principal with its bound identities, roles, and access control attributes.</summary>
public record PrincipalResponse
{
    /// <summary>Unique internal identifier for this principal.</summary>
    public required Guid Id { get; init; }

    /// <summary>All IDP identities bound to this principal.</summary>
    public required IEnumerable<IdentityResponse> Identities { get; init; }

    /// <summary>All roles currently assigned to this principal.</summary>
    public required IEnumerable<string> Roles { get; init; }

    /// <summary>All domain-scoped access control attributes assigned to this principal.</summary>
    public required IEnumerable<AttributeResponse> Attributes { get; init; }

    /// <summary>UTC timestamp of when the principal was created.</summary>
    public required DateTime CreatedAt { get; init; }
}

/// <summary>An external IDP identity bound to a principal.</summary>
public record IdentityResponse
{
    /// <summary>The IDP provider name e.g. "zitadel", "microsoft", "google".</summary>
    public required string Provider { get; init; }

    /// <summary>The user identifier issued by the provider.</summary>
    public required string ExternalId { get; init; }

    /// <summary>UTC timestamp of when this identity was bound.</summary>
    public required DateTime CreatedAt { get; init; }
}

/// <summary>A domain-scoped access control attribute entry.</summary>
public record AttributeResponse
{
    /// <summary>The domain that owns this attribute e.g. "user", "account".</summary>
    public required string Domain { get; init; }

    /// <summary>The attribute key within the domain.</summary>
    public required string Key { get; init; }

    /// <summary>The stored attribute value. Complex types are JSON serialized.</summary>
    public required string Value { get; init; }
}