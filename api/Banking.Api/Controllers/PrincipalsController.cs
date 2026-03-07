using Banking.Principals.AccessControl;
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
    public required IReadOnlyDictionary<string, object> Attributes { get; init; }

    public static PrincipalResponse From(ResolvedPrincipal principal) =>
        new()
        {
            Id = principal.Id,
            Identities = principal.Identities,
            Roles = principal.Roles,
            Attributes = principal.Attributes,
        };
}
