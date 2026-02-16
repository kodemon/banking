using Banking.Domain.Access;
using Banking.Infrastructure.Persistence;
using Banking.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Banking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly PrincipalAttributesBuilder _attributesBuilder;
    private readonly BankingDbContext _context;

    public AccountsController(
        PrincipalAttributesBuilder attributesBuilder,
        BankingDbContext context)
    {
        _attributesBuilder = attributesBuilder;
        _context = context;
    }

    [HttpGet("{accountId}")]
    public async Task<IActionResult> GetAccount(Guid accountId)
    {
        var userId = GetCurrentUserId(); // From JWT claims

        // Build principal attributes
        var attributes = await _attributesBuilder.BuildForUserAsync(userId);
        var principal = new Principal(userId, attributes);

        // Check if user has access to this account
        if (!principal.Attributes.AccessibleAccountIds.Contains(accountId))
        {
            return Forbid("You do not have access to this account");
        }

        // Get account
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null) return NotFound();

        return Ok(account);
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst("sub")?.Value;
        return Guid.Parse(claim!);
    }
}