namespace Banking.Principals.AccessControl;

public interface IPrincipalContext
{
    ResolvedPrincipal Principal { get; }
    bool IsResolved { get; }
}
