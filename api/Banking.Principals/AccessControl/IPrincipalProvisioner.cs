namespace Banking.Principals.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | IPrincipalProvisioner
 |--------------------------------------------------------------------------------
 |
 | Defined in Banking.Principals so ZitadelClaimsTransformation can call it
 | without knowing about Banking.Api.Commands.
 |
 | Implemented in Banking.Api (PrincipalProvisioner) which has access to
 | IMessageBus and ZitadelMetadataService and issues the full command chain:
 |   CreatePrincipalCommand → PrincipalCreated event → read model written.
 |
 | Registered in PrincipalsModule as a scoped service — Banking.Api's
 | Program.cs calls services.AddScoped<IPrincipalProvisioner, PrincipalProvisioner>()
 | after AddPrincipalsModule() so the implementation is wired at host startup.
 |
 */

public interface IPrincipalProvisioner
{
    /// <summary>
    /// Creates a new principal bound to the given IDP identity and writes
    /// the principal ID back to the IDP's user metadata.
    /// Called by ZitadelClaimsTransformation when no principal exists for
    /// an authenticated JWT.
    /// </summary>
    Task ProvisionAsync(string provider, string externalId);
}
