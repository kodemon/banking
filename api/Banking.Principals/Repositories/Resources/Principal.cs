namespace Banking.Principals.Repositories.Resources;

internal class Principal
{
    public Guid Id { get; init; }

    public List<PrincipalIdentity> Identities = new();
    public List<PrincipalRole> Roles = new();
    public List<PrincipalAttribute> Attributes = new();

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public IReadOnlyDictionary<string, string> GetDomainAttributes(string domain)
    {
        return Attributes.Where(a => a.Domain == domain).ToDictionary(a => a.Key, a => a.Value);
    }
}
