using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Banking.Principal.AccessControl;

internal class ZitadelClaimsTransformation(
    PrincipalService principalService,
    PrincipalContext principalContext) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal claimsPrincipal)
    {
        // Only run once per request
        if (principalContext.IsResolved)
            return claimsPrincipal;

        var sub = claimsPrincipal.FindFirst("sub")?.Value;
        if (sub is null)
            return claimsPrincipal;

        try
        {
            // Resolve using the identity bound in your Principal aggregate
            var resolved = await principalService.ResolveAsync("zitadel", sub);
            principalContext.Set(resolved);

            // Attach principal ID and roles as claims so [Authorize(Roles = "...")] works too
            var identity = (ClaimsIdentity)claimsPrincipal.Identity!;
            identity.AddClaim(new Claim("principal_id", resolved.Id.ToString()));

            foreach (var role in resolved.Roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        catch (Banking.Shared.Exceptions.AggregateNotFoundException)
        {
            // Zitadel session is valid but no principal is linked yet
            // Controllers can check IPrincipalContext.IsResolved and handle accordingly
        }

        return claimsPrincipal;
    }
}