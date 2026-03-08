namespace Banking.Principals.Repositories.Resources;

internal class Principal
{
    public Guid Id { get; init; }

    public List<PrincipalIdentity> Identities;
    public List<PrincipalRole> Roles;
    public Dictionary<string, object> Attributes { get; set; }

    public DateTime CreatedAt { get; init; }

    public Principal()
    {
        Id = Guid.NewGuid();
        Identities = new();
        Roles = new();
        Attributes = new();
        CreatedAt = DateTime.UtcNow;
    }

    /*
     |--------------------------------------------------------------------------------
     | Identity
     |--------------------------------------------------------------------------------
     |
     | Identitities ties an external identity provider to an internal authority.
     | Within a systems ecosystem the principal represents a authoritative entity
     | which can be attached as owners, managers or viewers of data.
     |
     */

    public bool HasIdentity(string provider, string externalId)
    {
        return Identities.Any(
            (identity) => identity.Provider == provider && identity.ExternalId == externalId
        );
    }

    public PrincipalIdentity AddIdentity(string provider, string externalId)
    {
        var identity = new PrincipalIdentity(Id, provider, externalId);
        Identities.Add(identity);
        return identity;
    }

    public PrincipalIdentity? GetIdentity(string provider, string externalId)
    {
        return Identities.Find(
            (identity) => identity.Provider == provider && identity.ExternalId == externalId
        );
    }

    public void RemoveIdentity(string provider, string externalId)
    {
        var identity = GetIdentity(provider, externalId);
        if (identity is null)
        {
            return;
        }
        Identities.Remove(identity);
    }

    /*
     |--------------------------------------------------------------------------------
     | Role
     |--------------------------------------------------------------------------------
     |
     | Roles defines a principals ability to operate in the systems ecosystem. It's
     | one of many ways a principal is given access privileges to data and system
     | functionality.
     |
     */

    public bool HasRole(string value)
    {
        return Roles.Any((role) => role.Role == value);
    }

    public PrincipalRole AddRole(string value)
    {
        var role = new PrincipalRole(Id, value);
        Roles.Add(role);
        return role;
    }

    public PrincipalRole? GetRole(string value)
    {
        return Roles.Find((role) => role.Role == value);
    }

    public void RemoveRole(Guid roleId)
    {
        var role = Roles.Find((role) => role.Id == roleId);
        if (role is null)
        {
            return;
        }
        Roles.Remove(role);
    }
}
