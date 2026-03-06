namespace Banking.Principal.DTO.Requests;

/*
 |--------------------------------------------------------------------------------
 | Requests
 |--------------------------------------------------------------------------------
 */

public record CreatePrincipalRequest
{
    public required string Provider { get; init; }
    public required string ExternalId { get; init; }
}

public record AddIdentityRequest
{
    public required string Provider { get; init; }
    public required string ExternalId { get; init; }
}

public record RemoveIdentityRequest
{
    public required string Provider { get; init; }
    public required string ExternalId { get; init; }
}

public record AddRoleRequest
{
    public required string Role { get; init; }
}


public record SetAttributeRequest
{
    public required string Domain { get; init; }
    public required string Key { get; init; }
    public required string Value { get; init; }
}

public record RemoveAttributeRequest
{
    public required string Domain { get; init; }
    public required string Key { get; init; }
}