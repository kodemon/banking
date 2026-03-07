using Banking.Api.Commands;
using Banking.Principal;
using Banking.Principal.AccessControl;
using Banking.Principal.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Principals")]
[Authorize]
internal class PrincipalsController(
    IMessageBus bus,
    PrincipalService principalService,
    IPrincipalContext principalContext
) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Principal
     |--------------------------------------------------------------------------------
     */

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PrincipalResponse>>> GetPrincipals()
    {
        var principals = await principalService.GetAllAsync();
        return Ok(principals);
    }

    [HttpGet("me")]
    public ActionResult<ResolvedPrincipal> GetMe()
    {
        if (!principalContext.IsResolved)
            return Forbid();
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
        await bus.InvokeAsync(
            new DeletePrincipalCommand(CorrelationId: Guid.NewGuid(), PrincipalId: id)
        );
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Identities
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/identities")]
    public async Task<IActionResult> AddIdentity(Guid id, [FromBody] AddIdentityRequest request)
    {
        await bus.InvokeAsync(
            new AddPrincipalIdentityCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: id,
                Provider: request.Provider,
                ExternalId: request.ExternalId
            )
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}/identities")]
    public async Task<IActionResult> RemoveIdentity(
        Guid id,
        [FromBody] RemoveIdentityRequest request
    )
    {
        await bus.InvokeAsync(
            new RemovePrincipalIdentityCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: id,
                Provider: request.Provider,
                ExternalId: request.ExternalId
            )
        );
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Roles
     |--------------------------------------------------------------------------------
     */

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AddRole(Guid id, [FromBody] AddRoleRequest request)
    {
        await bus.InvokeAsync(
            new AddPrincipalRoleCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: id,
                Role: request.Role
            )
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}/roles/{role}")]
    public async Task<IActionResult> RemoveRole(Guid id, string role)
    {
        await bus.InvokeAsync(
            new RemovePrincipalRoleCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: id,
                Role: role
            )
        );
        return NoContent();
    }

    /*
     |--------------------------------------------------------------------------------
     | Attributes
     |--------------------------------------------------------------------------------
     */

    [HttpPut("{id:guid}/attributes")]
    public async Task<IActionResult> SetAttribute(Guid id, [FromBody] SetAttributeRequest request)
    {
        await bus.InvokeAsync(
            new SetPrincipalAttributeCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: id,
                Domain: request.Domain,
                Key: request.Key,
                Value: request.Value
            )
        );
        return NoContent();
    }

    [HttpDelete("{id:guid}/attributes")]
    public async Task<IActionResult> RemoveAttribute(
        Guid id,
        [FromBody] RemoveAttributeRequest request
    )
    {
        await bus.InvokeAsync(
            new RemovePrincipalAttributeCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: id,
                Domain: request.Domain,
                Key: request.Key
            )
        );
        return NoContent();
    }
}

/*
 |--------------------------------------------------------------------------------
 | Request bodies
 |--------------------------------------------------------------------------------
 */

public record AddIdentityRequest(string Provider, string ExternalId);

public record RemoveIdentityRequest(string Provider, string ExternalId);

public record AddRoleRequest(string Role);

public record SetAttributeRequest(string Domain, string Key, string Value);

public record RemoveAttributeRequest(string Domain, string Key);
