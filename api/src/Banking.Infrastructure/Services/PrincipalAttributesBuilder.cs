using Banking.Domain.Access;
using Banking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Services;

/// <summary>
/// Builds PrincipalAttributes from multiple sources:
/// - PrincipalAttribute table (stored key-value attributes)
/// - PersonalAccountHolder relationships
/// - BusinessAccountHolder relationships
/// - External services (KYC, risk assessment, etc.) - future
/// </summary>
public class PrincipalAttributesBuilder
{
    private readonly BankingDbContext _context;

    public PrincipalAttributesBuilder(BankingDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Build PrincipalAttributes for a User
    /// </summary>
    public async Task<PrincipalAttributes> BuildForUserAsync(Guid userId)
    {
        // Source 1: Stored attributes
        var storedAttributes = await _context.PrincipalAttributes
            .Where(a => a.PrincipalId == userId)
            .ToDictionaryAsync(a => a.Key, a => a.Value);

        // Source 2: Personal account relationships
        var personalAccountHolderIds = await _context.PersonalAccountHolders
            .Where(h => h.UserId == userId)
            .Select(h => h.Id)
            .ToArrayAsync();

        var accessibleAccountIds = await _context.PersonalAccountHolders
            .Where(h => h.UserId == userId)
            .Select(h => h.AccountId)
            .ToArrayAsync();

        // Source 3: Business account authorizations
        // Check if user is authorized on any business accounts via stored attributes
        var authorizedBusinessHolderIds = storedAttributes
            .Where(kv => kv.Key.StartsWith("AuthorizedBusinessAccountHolder:"))
            .Select(kv => Guid.TryParse(kv.Value, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToArray();

        // Source 4: External services (placeholder for future)
        // var kycStatus = await _kycService.CheckStatusAsync(userId);
        // var riskLevel = await _riskService.GetRiskLevelAsync(userId);

        return BuildPrincipalAttributes(
            storedAttributes,
            accessibleAccountIds,
            personalAccountHolderIds,
            authorizedBusinessHolderIds
        );
    }

    /// <summary>
    /// Build PrincipalAttributes for a Business
    /// </summary>
    public async Task<PrincipalAttributes> BuildForBusinessAsync(Guid businessId)
    {
        // Source 1: Stored attributes
        var storedAttributes = await _context.PrincipalAttributes
            .Where(a => a.PrincipalId == businessId)
            .ToDictionaryAsync(a => a.Key, a => a.Value);

        // Source 2: Business account relationships
        var businessAccountHolderIds = await _context.BusinessAccountHolders
            .Where(h => h.BusinessId == businessId)
            .Select(h => h.Id)
            .ToArrayAsync();

        var accessibleAccountIds = await _context.BusinessAccountHolders
            .Where(h => h.BusinessId == businessId)
            .Select(h => h.AccountId)
            .ToArrayAsync();

        // Business principals don't have PersonalAccountHolder relationships
        var emptyPersonalHolderIds = Array.Empty<Guid>();

        return BuildPrincipalAttributes(
            storedAttributes,
            accessibleAccountIds,
            emptyPersonalHolderIds,
            businessAccountHolderIds
        );
    }

    /// <summary>
    /// Internal builder that constructs the PrincipalAttributes from all sources
    /// </summary>
    private PrincipalAttributes BuildPrincipalAttributes(
        Dictionary<string, string> storedAttributes,
        Guid[] accessibleAccountIds,
        Guid[] accessibleAccountHolderIds,
        Guid[] authorizedBusinessAccountHolderIds)
    {
        return new PrincipalAttributes
        {
            // System Level
            IsSystemAdministrator = GetBool(storedAttributes, "IsSystemAdministrator"),

            // Transaction Permissions
            TransactionApprovalLimitAmount = GetLong(storedAttributes, "TransactionApprovalLimitAmount"),
            TransactionApprovalLimitCurrency = GetString(storedAttributes, "TransactionApprovalLimitCurrency"),

            // Account Access (derived from relationships)
            AccessibleAccountIds = accessibleAccountIds,
            AccessibleAccountHolderIds = accessibleAccountHolderIds,

            // Compliance & KYC
            HasPassedKYC = GetBool(storedAttributes, "HasPassedKYC"),
            RiskLevel = GetString(storedAttributes, "RiskLevel") ?? "medium",

            // Regional/Geolocation
            AllowedCountries = GetStringArray(storedAttributes, "AllowedCountries"),
            PrimaryCountry = GetString(storedAttributes, "PrimaryCountry"),

            // Business Account Authorizations
            AuthorizedBusinessAccountHolderIds = authorizedBusinessAccountHolderIds,

            // Feature Flags / Toggles
            RequiresTwoFactorAuth = GetBool(storedAttributes, "RequiresTwoFactorAuth"),
            CanTransferInternationally = GetBool(storedAttributes, "CanTransferInternationally")
        };
    }

    // ### Attribute Parsing Helpers

    private static bool GetBool(Dictionary<string, string> attrs, string key)
    {
        return attrs.TryGetValue(key, out var value) &&
               bool.TryParse(value, out var result) &&
               result;
    }

    private static string? GetString(Dictionary<string, string> attrs, string key)
    {
        return attrs.TryGetValue(key, out var value) ? value : null;
    }

    private static long? GetLong(Dictionary<string, string> attrs, string key)
    {
        return attrs.TryGetValue(key, out var value) && long.TryParse(value, out var result)
            ? result
            : null;
    }

    private static string[] GetStringArray(Dictionary<string, string> attrs, string key)
    {
        if (!attrs.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            return Array.Empty<string>();

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();
    }

    // ### Convenience Methods for Setting Attributes

    /// <summary>
    /// Set a simple attribute value for a principal
    /// </summary>
    public async Task SetAttributeAsync(Guid principalId, string key, string value)
    {
        var existing = await _context.PrincipalAttributes
            .FirstOrDefaultAsync(a => a.PrincipalId == principalId && a.Key == key);

        if (existing != null)
        {
            existing.Value = value;
        }
        else
        {
            _context.PrincipalAttributes.Add(new PrincipalAttribute
            {
                Id = Guid.NewGuid(),
                PrincipalId = principalId,
                Key = key,
                Value = value
            });
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Set multiple attributes at once
    /// </summary>
    public async Task SetAttributesAsync(Guid principalId, Dictionary<string, string> attributes)
    {
        foreach (var (key, value) in attributes)
        {
            await SetAttributeAsync(principalId, key, value);
        }
    }

    /// <summary>
    /// Convenience: Set transaction approval limit
    /// </summary>
    public async Task SetTransactionApprovalLimitAsync(Guid principalId, long amount, string currency)
    {
        await SetAttributeAsync(principalId, "TransactionApprovalLimitAmount", amount.ToString());
        await SetAttributeAsync(principalId, "TransactionApprovalLimitCurrency", currency);
    }

    /// <summary>
    /// Convenience: Set system administrator flag
    /// </summary>
    public async Task SetSystemAdministratorAsync(Guid principalId, bool isAdmin)
    {
        await SetAttributeAsync(principalId, "IsSystemAdministrator", isAdmin.ToString());
    }

    /// <summary>
    /// Convenience: Set allowed countries
    /// </summary>
    public async Task SetAllowedCountriesAsync(Guid principalId, string[] countries)
    {
        await SetAttributeAsync(principalId, "AllowedCountries", string.Join(",", countries));
    }

    /// <summary>
    /// Convenience: Set KYC status
    /// </summary>
    public async Task SetKYCStatusAsync(Guid principalId, bool hasPassed)
    {
        await SetAttributeAsync(principalId, "HasPassedKYC", hasPassed.ToString());
    }

    /// <summary>
    /// Convenience: Set risk level
    /// </summary>
    public async Task SetRiskLevelAsync(Guid principalId, string riskLevel)
    {
        if (!new[] { "low", "medium", "high" }.Contains(riskLevel.ToLower()))
        {
            throw new ArgumentException("Risk level must be 'low', 'medium', or 'high'", nameof(riskLevel));
        }

        await SetAttributeAsync(principalId, "RiskLevel", riskLevel.ToLower());
    }

    /// <summary>
    /// Authorize a user to act on behalf of a business account holder
    /// </summary>
    public async Task AuthorizeUserForBusinessAccountAsync(Guid userId, Guid businessAccountHolderId)
    {
        // Store as a separate attribute for each business account holder
        var key = $"AuthorizedBusinessAccountHolder:{businessAccountHolderId}";
        await SetAttributeAsync(userId, key, businessAccountHolderId.ToString());
    }

    /// <summary>
    /// Revoke user authorization for a business account
    /// </summary>
    public async Task RevokeUserAuthorizationForBusinessAccountAsync(Guid userId, Guid businessAccountHolderId)
    {
        var key = $"AuthorizedBusinessAccountHolder:{businessAccountHolderId}";
        var existing = await _context.PrincipalAttributes
            .FirstOrDefaultAsync(a => a.PrincipalId == userId && a.Key == key);

        if (existing != null)
        {
            _context.PrincipalAttributes.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}