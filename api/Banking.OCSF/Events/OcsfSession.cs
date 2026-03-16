namespace Banking.OCSF.Events;

/// <summary>
/// Represents session context attached to an authentication event.
/// Maps to the OCSF Session object.
/// </summary>
public class OcsfSession
{
    /// <summary>
    /// The internal session ID created after a successful login.
    /// </summary>
    public Guid? SessionId { get; init; }

    /// <summary>
    /// When the session was created.
    /// </summary>
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// When the session expires or expired.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// Whether the session was issued with MFA satisfied.
    /// </summary>
    public bool? MfaSatisfied { get; init; }
}
