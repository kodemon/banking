using Banking.Principal.AccessControl;
using Banking.Principal.DTO.Requests;
using Banking.Principal.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Principal;

[ApiController]
[Route("api/[controller]")]
[Tags("Principal")]
[Authorize]
internal class PrincipalController(PrincipalService principalService, IPrincipalContext principalContext) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Principal
     |--------------------------------------------------------------------------------
     */

    [HttpPost]
    public async Task<ActionResult<PrincipalResponse>> CreatePrincipal([FromBody] CreatePrincipalRequest request)
    {
        var principal = await principalService.CreatePrincipalAsync(request);
        return CreatedAtAction(nameof(GetPrincipal), new { id = principal.Id }, principal);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrincipalResponse>>> GetPrincipals()
    {
        var principals = await principalService.GetAllAsync();
        return Ok(principals);
    }

    [HttpGet("me")]
    public ActionResult<PrincipalResponse> GetMe()
    {
        if (!principalContext.IsResolved)
        {
            return Forbid();
        }
        return Ok(principalContext.Principal);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PrincipalResponse>> GetPrincipal(Guid id)
    {
        var principal = await principalService.GetByIdAsync(id);
        return Ok(principal);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePrincipal(Guid id)
    {
        await principalService.DeleteAsync(id);
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Identities
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/identities")]
    public async Task<ActionResult<PrincipalResponse>> AddIdentity(Guid id, [FromBody] AddIdentityRequest request)
    {
        var principal = await principalService.AddIdentityAsync(id, request);
        return Ok(principal);
    }

    [HttpDelete("{id:guid}/identities")]
    public async Task<ActionResult<PrincipalResponse>> RemoveIdentity(Guid id, [FromBody] RemoveIdentityRequest request)
    {
        var principal = await principalService.RemoveIdentityAsync(id, request);
        return Ok(principal);
    }

    /*
     |--------------------------------------------------------------------------------
     | Roles
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/roles")]
    public async Task<ActionResult<PrincipalResponse>> AddRole(Guid id, [FromBody] AddRoleRequest request)
    {
        var principal = await principalService.AddRoleAsync(id, request);
        return Ok(principal);
    }

    [HttpDelete("{id:guid}/roles/{role}")]
    public async Task<ActionResult<PrincipalResponse>> RemoveRole(Guid id, string role)
    {
        var principal = await principalService.RemoveRoleAsync(id, role);
        return Ok(principal);
    }

    /*
     |--------------------------------------------------------------------------------
     | Attributes
     |--------------------------------------------------------------------------------
     */

    [HttpPut("{id:guid}/attributes")]
    public async Task<ActionResult<PrincipalResponse>> SetAttribute(
        Guid id,
        [FromBody] SetAttributeRequest request)
    {
        var principal = await principalService.SetAttributeAsync(id, request);
        return Ok(principal);
    }

    [HttpDelete("{id:guid}/attributes")]
    public async Task<ActionResult<PrincipalResponse>> RemoveAttribute(
        Guid id,
        [FromBody] RemoveAttributeRequest request)
    {
        var principal = await principalService.RemoveAttributeAsync(id, request);
        return Ok(principal);
    }
}