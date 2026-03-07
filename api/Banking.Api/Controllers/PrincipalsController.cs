using Banking.Principals.AccessControl;
using Banking.Users.AccessControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/principals")]
[Authorize]
[Tags("Principal")]
internal class PrincipalsController(IPrincipalContext principalContext) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<PrincipalResponse>(200)]
    [ProducesResponseType(404)]
    public ActionResult Me()
    {
        if (principalContext.IsResolved == false)
        {
            return NotFound();
        }

        return Ok(PrincipalResponse.From(principalContext.Principal));
    }
}

internal record PrincipalResponse
{
    public required Guid Id { get; init; }

    public required IReadOnlyCollection<ResolvedIdentity> Identities { get; init; }
    public required IReadOnlyCollection<string> Roles { get; init; }
    public required PrincipalAttributesResponse Attributes { get; init; }

    public static PrincipalResponse From(ResolvedPrincipal principal) =>
        new()
        {
            Id = principal.Id,
            Identities = principal.Identities,
            Roles = principal.Roles,
            Attributes = PrincipalAttributesResponse.From(principal),
        };
}

internal record PrincipalAttributesResponse
{
    public UserAccessAttributes? User { get; init; }

    public static PrincipalAttributesResponse From(ResolvedPrincipal principal) =>
        new() { User = principal.GetAttribute<UserAccessAttributes>("user") };
}

internal static class ResolvedPrincipalExtensions
{
    public static T? GetAttribute<T>(this ResolvedPrincipal principal, string domain)
        where T : class
    {
        return principal.Attributes.TryGetValue(domain, out var value) ? value as T : null;
    }
}
