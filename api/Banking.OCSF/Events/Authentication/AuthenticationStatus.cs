namespace Banking.OCSF.Events.Authentication;

/// <summary>
/// Defines the outcome status of an authentication event.
/// Values map directly to OCSF status_id.
/// </summary>
public enum AuthenticationStatus
{
    Unknown = 0,
    Success = 1,
    Failure = 2,
    Other = 99,
}
