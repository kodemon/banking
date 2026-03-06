using Banking.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Banking.Principal.AccessControl;

internal class ZitadelClaimsTransformation(
    PrincipalService principalService,
    PrincipalContext principalContext,
    ZitadelMetadataService zitadelMetadata,
    ILogger<ZitadelClaimsTransformation> logger) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal claimsPrincipal)
    {
        if (principalContext.IsResolved)
        {
            return claimsPrincipal;
        }

        var sub = claimsPrincipal.FindFirst("sub")?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null)
        {
            logger.LogWarning("JWT has no sub claim, skipping principal resolution");
            return claimsPrincipal;
        }

        try
        {
            // Resolve using the identity bound in your Principal aggregate
            var resolved = await principalService.ResolveAsync("zitadel", sub);
            principalContext.Set(resolved);

            // Attach principal ID and roles as claims so [Authorize(Roles = "...")] works too
            var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
            identity.AddClaim(new Claim("principal_id", resolved.Id.ToString()));

            foreach (var role in resolved.Roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }
        catch (AggregateNotFoundException)
        {
            logger.LogInformation("No principal found for Zitadel sub {Sub} — provisioning new principal", sub);

            var resolved = await ProvisionAsync(sub);
            AttachPrincipal(claimsPrincipal, resolved);
        }

        return claimsPrincipal;
    }

    private async Task<ResolvedPrincipal> ProvisionAsync(string sub)
    {
        // 1. Create the principal with the Zitadel identity bound
        var response = await principalService.CreatePrincipalAsync(new DTO.Requests.CreatePrincipalRequest
        {
            Provider = "zitadel",
            ExternalId = sub
        });

        // 2. Write principalId back to Zitadel user metadata
        await zitadelMetadata.SetPrincipalIdAsync(sub, response.Id);

        logger.LogInformation("Provisioned principal {PrincipalId} for Zitadel sub {Sub}", response.Id, sub);

        // 3. Resolve the full principal
        return await principalService.ResolveAsync("zitadel", sub);
    }

    private void AttachPrincipal(ClaimsPrincipal claimsPrincipal, ResolvedPrincipal resolved)
    {
        principalContext.Set(resolved);

        var identity = (ClaimsIdentity)claimsPrincipal.Identity!;

        identity.AddClaim(new Claim("principal_id", resolved.Id.ToString()));

        foreach (var role in resolved.Roles)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}