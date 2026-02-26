namespace Banking.Users.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | UserAccessAttributes
 |--------------------------------------------------------------------------------
 |
 | Defines the full access attribute shape for the Users domain.
 |
 | Default values represent the most restrictive safe state — a principal
 | with no stored user attributes gets a fully restricted instance.
 | Permissions are explicitly granted, never assumed.
 |
 */

public record UserAccessAttributes
{
    public string UserId { get; init; } = string.Empty;
    public EmailPermissions Email { get; init; } = new();
    public AddressPermissions Address { get; init; } = new();
}

public record EmailPermissions
{
    public bool Update { get; init; } = false;
    public bool Delete { get; init; } = false;
}

public record AddressPermissions
{
    public bool Create { get; init; } = false;
    public bool Update { get; init; } = false;
    public bool Delete { get; init; } = false;
}