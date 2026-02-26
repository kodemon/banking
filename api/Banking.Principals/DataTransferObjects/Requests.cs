namespace Banking.Principal.DTO.Requests;

/*
 |--------------------------------------------------------------------------------
 | Requests
 |--------------------------------------------------------------------------------
 */

/// <summary>Creates a new principal with an initial IDP identity binding.</summary>
public record CreatePrincipalRequest
{
    /// <summary>The IDP provider name e.g. "zitadel", "microsoft", "google".</summary>
    public required string Provider { get; init; }

    /// <summary>The user identifier issued by the provider.</summary>
    public required string ExternalId { get; init; }
}

/// <summary>Binds an additional IDP identity to a principal.</summary>
public record AddIdentityRequest
{
    /// <summary>The IDP provider name e.g. "zitadel", "microsoft", "google".</summary>
    public required string Provider { get; init; }

    /// <summary>The user identifier issued by the provider.</summary>
    public required string ExternalId { get; init; }
}

/// <summary>Removes an IDP identity binding from a principal.</summary>
public record RemoveIdentityRequest
{
    /// <summary>The IDP provider name.</summary>
    public required string Provider { get; init; }

    /// <summary>The external user identifier to unbind.</summary>
    public required string ExternalId { get; init; }
}

/// <summary>Assigns a role to a principal.</summary>
public record AddRoleRequest
{
    /// <summary>The role name to assign.</summary>
    public required string Role { get; init; }
}

/// <summary>
/// Sets a domain-scoped access control attribute on a principal.
/// The key and value are validated by the owning domain's resolver before persisting.
/// </summary>
public record SetAttributeRequest
{
    /// <summary>The domain that owns this attribute e.g. "user", "account".</summary>
    public required string Domain { get; init; }

    /// <summary>The attribute key within the domain.</summary>
    public required string Key { get; init; }

    /// <summary>The attribute value. Complex types should be serialized as JSON.</summary>
    public required string Value { get; init; }
}

/// <summary>Removes a domain-scoped access control attribute from a principal.</summary>
public record RemoveAttributeRequest
{
    /// <summary>The domain that owns this attribute.</summary>
    public required string Domain { get; init; }

    /// <summary>The attribute key to remove.</summary>
    public required string Key { get; init; }
}