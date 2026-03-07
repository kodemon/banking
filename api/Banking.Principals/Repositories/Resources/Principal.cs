namespace Banking.Principals.Repositories.Resources;

public class Principal
{
    public Guid Id { get; private set; }

    public readonly List<PrincipalIdentity> Identities = new();
    public readonly List<PrincipalRole> Roles = new();
    public readonly List<PrincipalAttribute> Attributes = new();

    public DateTime CreatedAt { get; private set; }
}
