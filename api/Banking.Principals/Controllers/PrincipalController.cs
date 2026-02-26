using Banking.Principal.DTO.Requests;
using Banking.Principal.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Principal;

/// <summary>Manages security principals, their IDP identity bindings, roles, and access control attributes.</summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Principal")]
[Authorize]
internal class PrincipalController(PrincipalService principalService) : ControllerBase
{
    /*
     |--------------------------------------------------------------------------------
     | Principal
     |--------------------------------------------------------------------------------
     */

    /// <summary>Creates a new principal with an initial IDP identity binding.</summary>
    /// <param name="request">The provider and external ID of the initial identity.</param>
    /// <response code="201">Principal created successfully.</response>
    /// <response code="409">This IDP identity is already bound to a principal.</response>
    [HttpPost]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PrincipalResponse>> CreatePrincipal([FromBody] CreatePrincipalRequest request)
    {
        var principal = await principalService.CreatePrincipalAsync(request);
        return CreatedAtAction(nameof(GetPrincipal), new { id = principal.Id }, principal);
    }

    /// <summary>Returns all principals.</summary>
    /// <response code="200">List of principals.</response>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PrincipalResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PrincipalResponse>>> GetPrincipals()
    {
        var principals = await principalService.GetAllAsync();
        return Ok(principals);
    }

    /// <summary>Returns a principal by ID.</summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <response code="200">Principal found.</response>
    /// <response code="404">Principal not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrincipalResponse>> GetPrincipal(Guid id)
    {
        var principal = await principalService.GetByIdAsync(id);
        return Ok(principal);
    }

    /// <summary>Deletes a principal and all associated identities, roles, and attributes.</summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <response code="204">Principal deleted successfully.</response>
    /// <response code="404">Principal not found.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>Binds an additional IDP identity to a principal.</summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <param name="request">The provider and external ID to bind.</param>
    /// <response code="200">Identity bound successfully.</response>
    /// <response code="404">Principal not found.</response>
    /// <response code="409">This IDP identity is already bound to a principal.</response>
    [HttpPost("{id:guid}/identities")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PrincipalResponse>> AddIdentity(Guid id, [FromBody] AddIdentityRequest request)
    {
        var principal = await principalService.AddIdentityAsync(id, request);
        return Ok(principal);
    }

    /// <summary>
    /// Removes an IDP identity binding from a principal.
    /// The last identity cannot be removed — bind a new one first.
    /// </summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <param name="request">The provider and external ID to unbind.</param>
    /// <response code="200">Identity removed successfully.</response>
    /// <response code="404">Principal or identity not found.</response>
    /// <response code="409">Cannot remove the last identity from a principal.</response>
    [HttpDelete("{id:guid}/identities")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    /// <summary>Assigns a role to a principal.</summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <param name="request">The role to assign.</param>
    /// <response code="200">Role assigned successfully.</response>
    /// <response code="404">Principal not found.</response>
    /// <response code="409">Role is already assigned to this principal.</response>
    [HttpPost("{id:guid}/roles")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PrincipalResponse>> AddRole(Guid id, [FromBody] AddRoleRequest request)
    {
        var principal = await principalService.AddRoleAsync(id, request);
        return Ok(principal);
    }

    /// <summary>Removes a role from a principal.</summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <param name="role">The role name to remove.</param>
    /// <response code="200">Role removed successfully.</response>
    /// <response code="404">Principal or role not found.</response>
    [HttpDelete("{id:guid}/roles/{role}")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Sets a domain-scoped access control attribute on a principal.
    /// The key and value are validated by the owning domain before persisting.
    /// Existing values are overwritten.
    /// </summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <param name="request">The domain, key, and value to set.</param>
    /// <response code="200">Attribute set successfully.</response>
    /// <response code="400">Key or value failed domain validation.</response>
    /// <response code="404">Principal or domain not found.</response>
    [HttpPut("{id:guid}/attributes")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrincipalResponse>> SetAttribute(
        Guid id,
        [FromBody] SetAttributeRequest request)
    {
        var principal = await principalService.SetAttributeAsync(id, request);
        return Ok(principal);
    }

    /// <summary>Removes a domain-scoped access control attribute from a principal.</summary>
    /// <param name="id">The principal's unique identifier.</param>
    /// <param name="request">The domain and key to remove.</param>
    /// <response code="200">Attribute removed successfully.</response>
    /// <response code="404">Principal or attribute not found.</response>
    [HttpDelete("{id:guid}/attributes")]
    [ProducesResponseType<PrincipalResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PrincipalResponse>> RemoveAttribute(
        Guid id,
        [FromBody] RemoveAttributeRequest request)
    {
        var principal = await principalService.RemoveAttributeAsync(id, request);
        return Ok(principal);
    }
}