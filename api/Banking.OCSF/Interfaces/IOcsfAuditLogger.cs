using Banking.OCSF.Events.Authentication;

namespace Banking.OCSF.Interfaces;

/// <summary>
/// Provides OCSF-compliant audit logging for security events.
///
/// Implementations are responsible for guaranteeing delivery — callers
/// fire and move on. No exceptions from the underlying transport should
/// propagate back to the caller.
/// </summary>
public interface IOcsfAuditLogger
{
    /// <summary>
    /// Publishes an OCSF Authentication event (Class 3002) to the audit pipeline.
    /// </summary>
    Task LogAuthenticationAsync(AuthenticationEvent @event, CancellationToken ct = default);
}
