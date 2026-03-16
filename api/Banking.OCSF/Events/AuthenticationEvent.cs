using Banking.OCSF.Events;

namespace Banking.OCSF.Events.Authentication;

/// <summary>
/// Represents an OCSF Authentication event (Class UID 3002).
///
/// Authentication events report authentication session activities such as
/// user login/logout attempts, token issuance, and session lifecycle events.
///
/// OCSF spec: https://schema.ocsf.io/classes/authentication
/// </summary>
public class AuthenticationEvent
{
    /*
     |--------------------------------------------------------------------------
     | OCSF Base Event Fields (required on every event class)
     |--------------------------------------------------------------------------
     */

    /// <summary>
    /// OCSF Class UID. Always 3002 for Authentication events.
    /// </summary>
    public int ClassUid { get; init; } = 3002;

    /// <summary>
    /// OCSF Category UID. Always 3 (Identity & Access Management).
    /// </summary>
    public int CategoryUid { get; init; } = 3;

    /// <summary>
    /// Unique identifier for this specific event instance.
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// The time the event occurred, in UTC.
    /// </summary>
    public DateTime OccurredAt { get; init; }

    /// <summary>
    /// OCSF severity_id. 1=Informational, 2=Low, 3=Medium, 4=High, 5=Critical.
    /// Failed logins are typically 3 (Medium); repeated failures should escalate.
    /// </summary>
    public int SeverityId { get; init; }

    /// <summary>
    /// Human-readable description of the event.
    /// </summary>
    public string Message { get; init; }

    /*
     |--------------------------------------------------------------------------
     | Authentication-Specific Fields (OCSF class 3002)
     |--------------------------------------------------------------------------
     */

    /// <summary>
    /// The specific authentication activity that occurred.
    /// </summary>
    public AuthenticationActivity Activity { get; init; }

    /// <summary>
    /// The outcome of the authentication attempt.
    /// </summary>
    public AuthenticationStatus Status { get; init; }

    /// <summary>
    /// Optional detail on why the authentication failed.
    /// Only populated on failure events.
    /// </summary>
    public string? StatusDetail { get; init; }

    /// <summary>
    /// The mechanism used to authenticate.
    /// </summary>
    public AuthenticationLogonType LogonType { get; init; }

    /// <summary>
    /// Whether MFA was used during this authentication.
    /// </summary>
    public bool MfaUsed { get; init; }

    /*
     |--------------------------------------------------------------------------
     | OCSF Object Fields
     |--------------------------------------------------------------------------
     */

    /// <summary>
    /// The actor (user) who performed or attempted the authentication.
    /// </summary>
    public OcsfActor Actor { get; init; }

    /// <summary>
    /// Session context. Populated on successful logins and session events.
    /// </summary>
    public OcsfSession? Session { get; init; }

    /// <summary>
    /// Network context of where the request originated.
    /// </summary>
    public OcsfNetworkEndpoint? SrcEndpoint { get; init; }

    /*
     |--------------------------------------------------------------------------
     | Constructor
     |--------------------------------------------------------------------------
     */

    public AuthenticationEvent(
        AuthenticationActivity activity,
        AuthenticationStatus status,
        int severityId,
        string message,
        OcsfActor actor,
        AuthenticationLogonType logonType = AuthenticationLogonType.Network,
        bool mfaUsed = false,
        string? statusDetail = null,
        OcsfSession? session = null,
        OcsfNetworkEndpoint? srcEndpoint = null
    )
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        Activity = activity;
        Status = status;
        SeverityId = severityId;
        Message = message;
        Actor = actor;
        LogonType = logonType;
        MfaUsed = mfaUsed;
        StatusDetail = statusDetail;
        Session = session;
        SrcEndpoint = srcEndpoint;
    }
}
