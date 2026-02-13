using Banking.Domain.ValueObjects;

namespace Banking.Domain.AccessControl;

public readonly record struct UserAccessControlState
{
    // ### System-level permissions

    public bool IsSystemAdministrator { get; init; }

    // ### Transaction permissions

    public Money? TransactionApprovalLimit { get; init; }

    // ### Account access (resolved from relationships)

    public IReadOnlySet<Guid> AccessibleAccountHolderIds { get; init; }
    public IReadOnlySet<Guid> AccessibleAccountIds { get; init; }

    // Future: External service data
    // public bool HasPassedKyc { get; init; }
    // public RiskLevel RiskAssessment { get; init; }
    // public IReadOnlyList<string> AssignedPermissions { get; init; }

    public UserAccessControlState()
    {
        IsSystemAdministrator = false;
        TransactionApprovalLimit = null;
        AccessibleAccountHolderIds = new HashSet<Guid>();
        AccessibleAccountIds = new HashSet<Guid>();
    }
}