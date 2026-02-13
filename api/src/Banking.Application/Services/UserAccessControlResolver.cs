using Banking.Domain.AccessControl;
using Banking.Domain.Enums;
using Banking.Domain.ValueObjects;
using Banking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Application.Services;

/// <summary>
/// Resolves UserAccessControlState from multiple sources.
/// This is where you aggregate data from:
/// - UserAttributes table
/// - Account/AccountHolder relationships
/// - External IAM services
/// - Risk assessment APIs
/// - etc.
/// </summary>
public class UserAccessControlResolver
{
    private readonly BankingDbContext _context;

    public UserAccessControlResolver(BankingDbContext context)
    {
        _context = context;
    }

    public async Task<UserAccessControl> ResolveAsync(Guid userId)
    {
        var state = await ResolveStateAsync(userId);
        return new UserAccessControl(userId, state);
    }

    private async Task<UserAccessControlState> ResolveStateAsync(Guid userId)
    {
        // Source 1: User attributes
        var attributes = await _context.UserAttributes
            .Where(a => a.UserId == userId)
            .ToListAsync();

        var isSystemAdmin = GetBoolAttribute(attributes, UserAttributeKey.IsSystemAdministrator);
        var approvalLimit = GetMoneyAttribute(attributes, UserAttributeKey.TransactionApprovalLimit);

        // Source 2: Account relationships
        var accountHolderIds = await _context.AccountHolders
            .Where(ah => ah.UserId == userId)
            .Select(ah => ah.Id)
            .ToListAsync();

        var accountIds = await _context.Accounts
            .Where(a => a.Holders.Any(h => accountHolderIds.Contains(h.Id)))
            .Select(a => a.Id)
            .ToListAsync();

        // Source 3: External services (example - commented out)
        // var kycStatus = await _kycService.GetStatusAsync(userId);
        // var riskLevel = await _riskAssessmentService.GetRiskLevelAsync(userId);

        return new UserAccessControlState
        {
            IsSystemAdministrator = isSystemAdmin,
            TransactionApprovalLimit = approvalLimit,
            AccessibleAccountHolderIds = accountHolderIds.ToHashSet(),
            AccessibleAccountIds = accountIds.ToHashSet(),

            // Future external data:
            // HasPassedKyc = kycStatus.IsPassed,
            // RiskAssessment = riskLevel,
        };
    }

    private bool GetBoolAttribute(List<UserAttribute> attributes, UserAttributeKey key)
    {
        var value = attributes.FirstOrDefault(a => a.Key == key)?.Value;
        return bool.TryParse(value, out var result) && result;
    }

    private Money? GetMoneyAttribute(List<UserAttribute> attributes, UserAttributeKey key)
    {
        var value = attributes.FirstOrDefault(a => a.Key == key)?.Value;

        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        var parts = value.Split(',');
        if (parts.Length != 2 || !long.TryParse(parts[0], out var amount))
        {
            return null;
        }

        try
        {
            return new Money(amount, Currency.FromCode(parts[1]));
        }
        catch
        {
            return null;
        }
    }
}