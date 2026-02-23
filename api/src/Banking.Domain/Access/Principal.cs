namespace Banking.Domain.Access;

public class Principal
{
    public string Id { get; init; }
    public IReadOnlySet<string> Roles { get; init; }
    public PrincipalAttributes Attributes { get; init; }

    public Principal(Guid userId, PrincipalAttributes attributes)
    {
        Id = userId.ToString();
        Attributes = attributes;

        var roles = new HashSet<string> { "user" };
        if (attributes.IsSystemAdministrator)
        {
            roles.Add("admin");
        }
        Roles = roles;
    }

    public Dictionary<string, object> GetAttributes() => Attributes.ToCerbosAttributes();
}