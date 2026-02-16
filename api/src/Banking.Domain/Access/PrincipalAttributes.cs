namespace Banking.Domain.Access;

public readonly record struct PrincipalAttributes
{
    // ### System Level

    public bool IsSystemAdministrator { get; init; }

    // ### Transaction Permissions

    public long? TransactionApprovalLimitAmount { get; init; }
    public string? TransactionApprovalLimitCurrency { get; init; }

    // ### Account Access (derived from relationships)

    public Guid[] AccessibleAccountIds { get; init; }
    public Guid[] AccessibleAccountHolderIds { get; init; }

    // ### Compliance & KYC

    public bool HasPassedKYC { get; init; }
    public string? RiskLevel { get; init; } // "low", "medium", "high"

    // ### Regional/Geolocation

    public string[] AllowedCountries { get; init; }
    public string? PrimaryCountry { get; init; }

    // ### Business Account Authorizations

    public Guid[] AuthorizedBusinessAccountHolderIds { get; init; }

    // ### Feature Flags / Toggles

    public bool RequiresTwoFactorAuth { get; init; }
    public bool CanTransferInternationally { get; init; }

    // Default constructor with safe defaults

    public PrincipalAttributes()
    {
        IsSystemAdministrator = false;

        TransactionApprovalLimitAmount = null;
        TransactionApprovalLimitCurrency = null;

        AccessibleAccountIds = Array.Empty<Guid>();
        AccessibleAccountHolderIds = Array.Empty<Guid>();

        HasPassedKYC = false;
        RiskLevel = "medium";

        AllowedCountries = Array.Empty<string>();
        PrimaryCountry = null;

        AuthorizedBusinessAccountHolderIds = Array.Empty<Guid>();

        RequiresTwoFactorAuth = false;
        CanTransferInternationally = false;
    }

    public Dictionary<string, object> ToCerbosAttributes()
    {
        var attrs = new Dictionary<string, object>();

        // System
        attrs["isSystemAdministrator"] = IsSystemAdministrator;

        // Transaction
        if (TransactionApprovalLimitAmount.HasValue && !string.IsNullOrEmpty(TransactionApprovalLimitCurrency))
        {
            attrs["transactionApprovalLimit"] = new
            {
                amount = TransactionApprovalLimitAmount.Value,
                currency = TransactionApprovalLimitCurrency
            };
        }

        // Accounts
        if (AccessibleAccountIds.Length > 0)
            attrs["accessibleAccountIds"] = AccessibleAccountIds.Select(id => id.ToString()).ToArray();

        if (AccessibleAccountHolderIds.Length > 0)
            attrs["accessibleAccountHolderIds"] = AccessibleAccountHolderIds.Select(id => id.ToString()).ToArray();

        // Compliance
        attrs["hasPassedKYC"] = HasPassedKYC;
        if (!string.IsNullOrEmpty(RiskLevel))
            attrs["riskLevel"] = RiskLevel;

        // Regional
        if (AllowedCountries.Length > 0)
            attrs["allowedCountries"] = AllowedCountries;

        if (!string.IsNullOrEmpty(PrimaryCountry))
            attrs["primaryCountry"] = PrimaryCountry;

        // Business
        if (AuthorizedBusinessAccountHolderIds.Length > 0)
            attrs["authorizedBusinessAccountHolderIds"] = AuthorizedBusinessAccountHolderIds.Select(id => id.ToString()).ToArray();

        // Features
        attrs["requiresTwoFactorAuth"] = RequiresTwoFactorAuth;
        attrs["canTransferInternationally"] = CanTransferInternationally;

        return attrs;
    }
}