namespace Banking.Principal.AccessControl;

public interface IPrincipalContext
{
    ResolvedPrincipal Principal { get; }
    bool IsResolved { get; }
}