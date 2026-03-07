using System.Security.Claims;
using Banking.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Banking.Principals.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | ZitadelClaimsTransformation
 |--------------------------------------------------------------------------------
 |
 | Runs on every authenticated request. Resolves the Zitadel sub claim to an
 | internal Principal and populates PrincipalContext for the request.
 |
 | Auto-provisioning is delegated to IPrincipalProvisioner — an interface
 | defined here but implemented in Banking.Api. This avoids a circular
 | dependency: Banking.Principals cannot reference Banking.Api.Commands,
 | so the command dispatch stays in Banking.Api where it belongs.
 |
 */

internal class ZitadelClaimsTransformation(
    PrincipalService principalService,
    PrincipalContext principalContext,
    IPrincipalProvisioner provisioner,
    ILogger<ZitadelClaimsTransformation> logger
) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal claimsPrincipal)
    {
        if (principalContext.IsResolved)
            return claimsPrincipal;

        var sub =
            claimsPrincipal.FindFirst("sub")?.Value
            ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (sub is null)
        {
            logger.LogWarning("JWT has no sub claim, skipping principal resolution");
            return claimsPrincipal;
        }

        try
        {
            var resolved = await principalService.ResolveAsync("zitadel", sub);
            Attach(claimsPrincipal, resolved);
        }
        catch (AggregateNotFoundException)
        {
            logger.LogInformation("No principal found for Zitadel sub {Sub} — provisioning", sub);

            // Delegates to Banking.Api's implementation which issues the command
            await provisioner.ProvisionAsync("zitadel", sub);

            var resolved = await principalService.ResolveAsync("zitadel", sub);
            Attach(claimsPrincipal, resolved);
        }

        return claimsPrincipal;
    }

    private void Attach(ClaimsPrincipal claimsPrincipal, ResolvedPrincipal resolved)
    {
        principalContext.Set(resolved);

        var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
        identity.AddClaim(new Claim("principal_id", resolved.Id.ToString()));

        foreach (var role in resolved.Roles)
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
    }
}
