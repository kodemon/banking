namespace Banking.Users.AccessControl;

public record UserAccessAttributes
{
    public string? UserId { get; init; }
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
