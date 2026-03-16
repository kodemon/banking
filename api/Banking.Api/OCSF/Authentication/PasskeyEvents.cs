using Banking.OCSF.Events;
using Banking.OCSF.Events.Authentication;
using Banking.Principals.Database.Models;

namespace Banking.OCSF.Authentication;

/// <summary>
/// Builds OCSF AuthenticationEvents for passkey registration and login flows.
///
/// Intentionally static — no state, no injected dependencies. The controller
/// owns the IOcsfAuditLogger call and supplies HttpContext-derived values
/// (IP, user agent) directly. This keeps the class pure and trivially testable.
/// </summary>
internal static class PasskeyAuditEvents
{
    /// <summary>
    /// Registration failed at Fido2 credential verification.
    /// Severity 3 (Medium) — failed attempt, identity not yet established.
    /// </summary>
    public static AuthenticationEvent CredentialVerificationFailed(
        string? userId,
        string errorDetail,
        OcsfNetworkEndpoint endpoint
    ) =>
        new(
            activity: AuthenticationActivity.Logon,
            status: AuthenticationStatus.Failure,
            severityId: 3,
            message: "Passkey registration failed during credential verification",
            actor: new OcsfActor { DisplayName = userId },
            logonType: AuthenticationLogonType.Network,
            mfaUsed: false, // MFA not satisfied — verification never completed
            statusDetail: errorDetail,
            srcEndpoint: endpoint
        );

    /// <summary>
    /// Registration succeeded — principal and first session created.
    /// Severity 1 (Informational) — expected happy path.
    /// </summary>
    public static AuthenticationEvent RegistrationSucceeded(
        Guid principalId,
        string credentialId,
        PrincipalSession session,
        OcsfNetworkEndpoint endpoint
    ) =>
        new(
            activity: AuthenticationActivity.Logon,
            status: AuthenticationStatus.Success,
            severityId: 1,
            message: "Passkey registration completed and session created",
            actor: new OcsfActor
            {
                PrincipalId = principalId,
                IdentityProvider = "passkey",
                ExternalId = credentialId,
            },
            logonType: AuthenticationLogonType.Network,
            mfaUsed: true, // Passkey satisfies MFA — possession + user verification
            session: new OcsfSession
            {
                SessionId = session.Id,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                MfaSatisfied = true,
            },
            srcEndpoint: endpoint
        );

    /// <summary>
    /// Login attempted with a credential ID not found in the system.
    /// Severity 3 (Medium) — could indicate credential enumeration.
    /// </summary>
    public static AuthenticationEvent CredentialNotFound(
        string credentialId,
        OcsfNetworkEndpoint endpoint
    ) =>
        new(
            activity: AuthenticationActivity.Logon,
            status: AuthenticationStatus.Failure,
            severityId: 3,
            message: "Passkey login failed — credential not recognised",
            actor: new OcsfActor { ExternalId = credentialId, IdentityProvider = "passkey" },
            logonType: AuthenticationLogonType.Network,
            mfaUsed: false,
            statusDetail: "Credential ID not found in the system",
            srcEndpoint: endpoint
        );

    /// <summary>
    /// Login failed — credential was found but Fido2 assertion verification failed.
    /// Severity 4 (High) — cryptographic failure against a known credential is
    /// more suspicious than an unknown credential; could indicate a replay attack.
    /// </summary>
    public static AuthenticationEvent AssertionVerificationFailed(
        Guid principalId,
        string credentialId,
        string errorDetail,
        OcsfNetworkEndpoint endpoint
    ) =>
        new(
            activity: AuthenticationActivity.Logon,
            status: AuthenticationStatus.Failure,
            severityId: 4,
            message: "Passkey login failed — assertion verification failed",
            actor: new OcsfActor
            {
                PrincipalId = principalId,
                IdentityProvider = "passkey",
                ExternalId = credentialId,
            },
            logonType: AuthenticationLogonType.Network,
            mfaUsed: false,
            statusDetail: errorDetail,
            srcEndpoint: endpoint
        );

    /// <summary>
    /// Login succeeded — session created.
    /// Severity 1 (Informational) — expected happy path.
    /// </summary>
    public static AuthenticationEvent LoginSucceeded(
        Guid principalId,
        string credentialId,
        PrincipalSession session,
        OcsfNetworkEndpoint endpoint
    ) =>
        new(
            activity: AuthenticationActivity.Logon,
            status: AuthenticationStatus.Success,
            severityId: 1,
            message: "Passkey login successful",
            actor: new OcsfActor
            {
                PrincipalId = principalId,
                IdentityProvider = "passkey",
                ExternalId = credentialId,
            },
            logonType: AuthenticationLogonType.Network,
            mfaUsed: true,
            session: new OcsfSession
            {
                SessionId = session.Id,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                MfaSatisfied = true,
            },
            srcEndpoint: endpoint
        );
}
