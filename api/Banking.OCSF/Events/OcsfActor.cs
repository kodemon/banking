namespace Banking.OCSF.Events;

/// <summary>
/// Represents the actor (user or system) that initiated an event.
/// Maps to the OCSF Actor object.
/// </summary>
public class OcsfActor
{
    /// <summary>
    /// The internal principal ID within the banking system.
    /// </summary>
    public Guid? PrincipalId { get; init; }

    /// <summary>
    /// The identity provider used (e.g. "passkey", "password", "google").
    /// </summary>
    public string? IdentityProvider { get; init; }

    /// <summary>
    /// The external identity ID from the provider.
    /// </summary>
    public string? ExternalId { get; init; }

    /// <summary>
    /// Human-readable name or email if available.
    /// </summary>
    public string? DisplayName { get; init; }
}
