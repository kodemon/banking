namespace Banking.Principal.DTO.Responses;

/*
 |--------------------------------------------------------------------------------
 | Responses
 |--------------------------------------------------------------------------------
 */

public record PrincipalResponse
{
    public required Guid Id { get; init; }

    public required IEnumerable<IdentityResponse> Identities { get; init; }
    public required IEnumerable<string> Roles { get; init; }
    public required IEnumerable<AttributeResponse> Attributes { get; init; }

    public required DateTime CreatedAt { get; init; }
}

public record IdentityResponse
{
    public required string Provider { get; init; }
    public required string ExternalId { get; init; }
    public required DateTime CreatedAt { get; init; }
}

public record AttributeResponse
{
    public required string Domain { get; init; }
    public required string Key { get; init; }
    public required string Value { get; init; }
}