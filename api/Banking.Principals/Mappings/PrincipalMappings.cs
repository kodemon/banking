using Banking.Principal.DTO.Responses;

namespace Banking.Principal.Mappings;

internal static class PrincipalMappings
{
    internal static PrincipalResponse ToResponse(this Principal principal) =>
        new()
        {
            Id = principal.Id,
            Identities = principal.Identities.Select(i => i.ToResponse()),
            Roles = principal.Roles.Select(r => r.Role),
            Attributes = principal.Attributes.Select(a => new AttributeResponse
            {
                Domain = a.Domain,
                Key = a.Key,
                Value = a.Value
            }),
            CreatedAt = principal.CreatedAt
        };

    internal static IdentityResponse ToResponse(this PrincipalIdentity identity) =>
        new()
        {
            Provider = identity.Provider,
            ExternalId = identity.ExternalId,
            CreatedAt = identity.CreatedAt
        };
}