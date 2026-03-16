namespace Banking.OCSF.Events;

/// <summary>
/// Captures network context of where an event originated.
/// Maps to the OCSF Network Endpoint object.
/// </summary>
public class OcsfNetworkEndpoint
{
    /// <summary>
    /// The IP address of the client.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// The user agent string of the client browser or app.
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// The geographic country code if resolvable (e.g. "NO", "US").
    /// </summary>
    public string? CountryCode { get; init; }
}
