using Banking.Api.Identity;
using Banking.Principals.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/principals")]
[Authorize]
[Tags("Principal")]
internal class PrincipalsController(IAuth auth, IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<ResolvedPrincipal>(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Me()
    {
        var principal = await mediator.Send(new GetPrincipalByIdQuery(auth.Principal.Id));
        if (principal is null)
        {
            return NotFound();
        }
        return Ok(principal);
    }
}
