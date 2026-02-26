namespace Banking.Shared.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | IAccessAttributeResolver
 |--------------------------------------------------------------------------------
 |
 | Contract each domain implements to participate in principal attribute
 | resolution and validation.
 |
 | Banking.Principal has no knowledge of domain attribute shapes — it only
 | works through this interface. Domains own both directions:
 |
 |   Read  — Resolve() hydrates raw stored key/value pairs into a typed
 |            domain access attributes instance.
 |
 |   Write — Validate() ensures a proposed key/value is acceptable before
 |            Banking.Principal persists it.
 |
 | The returned object from Resolve() is passed into the Cerbos principal
 | attr map under the domain's namespace key.
 |
 */

public interface IAccessAttributeResolver
{
    /// <summary>
    /// Domain namespace key. Used to scope stored attributes and to key
    /// the resolved instance in the Cerbos principal attr map.
    /// e.g. "user", "account", "business"
    /// </summary>
    string Domain { get; }

    /// <summary>
    /// Hydrates raw stored key/value pairs for this domain into a typed
    /// access attributes instance. Missing keys should fall back to the
    /// record's default values — never throw on missing data.
    /// </summary>
    object Resolve(IReadOnlyDictionary<string, string> rawValues);

    /// <summary>
    /// Validates a proposed attribute key/value pair against this domain's
    /// schema before Banking.Principal persists it. The domain is the sole
    /// authority on what is valid within its namespace.
    /// </summary>
    AttributeValidationResult Validate(string key, object value);
}