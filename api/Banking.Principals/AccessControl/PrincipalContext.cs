namespace Banking.Principal.AccessControl;

internal class PrincipalContext : IPrincipalContext
{
    private ResolvedPrincipal? _principal;

    public ResolvedPrincipal Principal =>
        _principal ?? throw new InvalidOperationException("Principal has not been resolved for this request.");

    public bool IsResolved => _principal is not null;

    internal void Set(ResolvedPrincipal principal) => _principal = principal;
}