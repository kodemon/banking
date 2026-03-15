using Banking.Api.Identity;
using Banking.Principals.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
internal class AuthController(IMediator mediator) : ControllerBase
{
    [HttpGet("session")]
    [ProducesResponseType<SessionResponse>(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Session(CancellationToken ct)
    {
        if (!AuthContext.TryGetAuth(HttpContext, out var auth))
        {
            return Unauthorized();
        }
        return Ok(new SessionResponse(auth.Principal.Id));
    }

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        if (!AuthCookies.TryGetSessionId(Request, out var sessionId))
        {
            return Ok();
        }

        await mediator.Send(new DeleteSessionCommand(sessionId), ct);

        AuthCookies.ClearSessionCookie(HttpContext);

        return Ok();
    }
}
