namespace Banking.OCSF.Events.Authentication;

/// <summary>
/// Describes the mechanism used to authenticate.
/// Maps to OCSF logon_type_id in the Authentication event class.
/// </summary>
public enum AuthenticationLogonType
{
    Unknown = 0,
    Interactive = 1,
    Network = 2,
    NetworkCleartext = 8,
    NewCredentials = 9,
    Other = 99,
}
