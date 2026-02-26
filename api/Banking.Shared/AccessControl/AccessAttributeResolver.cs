using System.Text.Json;

namespace Banking.Shared.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | AccessAttributeResolver<TAttributes>
 |--------------------------------------------------------------------------------
 |
 | Base implementation of IAccessAttributeResolver providing a common
 | serialization convention for read and write paths.
 |
 | Domains derive from this and implement:
 |   - Domain property
 |   - Resolve(rawValues) returning a typed TAttributes instance
 |   - Validate(key, value) enforcing domain rules
 |
 | Storage convention:
 |   Scalar values  →  stored as plain string         e.g. "true", "xyz"
 |   Complex values →  stored as JSON                 e.g. {"update":true,"delete":false}
 |
 | Usage:
 |   public class UserAccessAttributeResolver
 |       : AccessAttributeResolver<UserAccessAttributes>
 |   {
 |       public override string Domain => "user";
 |
 |       public override UserAccessAttributes Resolve(
 |           IReadOnlyDictionary<string, string> rawValues) { ... }
 |
 |       public override AttributeValidationResult Validate(
 |           string key, object value) { ... }
 |   }
 |
 */

public abstract class AccessAttributeResolver<TAttributes> : IAccessAttributeResolver
    where TAttributes : class
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public abstract string Domain { get; }

    public abstract TAttributes Resolve(IReadOnlyDictionary<string, string> rawValues);

    public abstract AttributeValidationResult Validate(string key, object value);

    // Explicit interface implementation routes through the typed Resolve
    object IAccessAttributeResolver.Resolve(IReadOnlyDictionary<string, string> rawValues)
    {
        return Resolve(rawValues);
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers for domain resolvers
     |--------------------------------------------------------------------------------
     */

    /// <summary>
    /// Reads a scalar string value from raw storage, returning a fallback if missing.
    /// </summary>
    protected static string GetString(
        IReadOnlyDictionary<string, string> raw,
        string key,
        string fallback = "") =>
        raw.TryGetValue(key, out var value) ? value : fallback;

    /// <summary>
    /// Reads and deserializes a complex JSON value, returning a new default instance
    /// if the key is missing or deserialization fails.
    /// </summary>
    protected static T GetObject<T>(
        IReadOnlyDictionary<string, string> raw,
        string key) where T : new()
    {
        if (!raw.TryGetValue(key, out var json))
        {
            return new T();
        }
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? new T();
        }
        catch
        {
            return new T();
        }
    }

    /// <summary>
    /// Serializes a complex value to JSON for storage.
    /// </summary>
    protected static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, JsonOptions);
    }

    /// <summary>
    /// Attempts to deserialize a raw JSON string into T.
    /// Returns false if the string is not valid JSON for that type.
    /// </summary>
    protected static bool TryDeserialize<T>(string raw, out T? result)
    {
        try
        {
            result = JsonSerializer.Deserialize<T>(raw, JsonOptions);
            return result is not null;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}