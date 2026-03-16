namespace Banking.OCSF.Events.Authentication;

/// <summary>
/// Defines the specific authentication activity that occurred.
/// Values map directly to OCSF Authentication (3002) activity_id.
/// </summary>
public enum AuthenticationActivity
{
    Unknown = 0,
    Logon = 1,
    Logoff = 2,
    AuthenticationTicket = 3,
    ServiceTicket = 4,
    Other = 99,
}
