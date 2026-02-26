using Banking.Shared.AccessControl;

namespace Banking.Users.AccessControl;

/*
 |--------------------------------------------------------------------------------
 | UserAccessAttributeResolver
 |--------------------------------------------------------------------------------
 |
 | Owns both directions of user domain attribute management:
 |
 |   Resolve  — builds a UserAccessAttributes instance from raw stored values.
 |              Missing keys fall back to record defaults (most restrictive).
 |
 |   Validate — ensures a proposed key is known and the value is the correct
 |              shape for that key before Banking.Principal persists it.
 |
 | Known keys:
 |   user_id   →  string
 |   email     →  EmailPermissions  (JSON)
 |   address   →  AddressPermissions (JSON)
 |
 */

internal class UserAccessAttributeResolver : AccessAttributeResolver<UserAccessAttributes>
{
    public override string Domain => "user";

    /*
     |--------------------------------------------------------------------------------
     | Resolve
     |--------------------------------------------------------------------------------
     */

    public override UserAccessAttributes Resolve(IReadOnlyDictionary<string, string> rawValues) =>
        new()
        {
            UserId = GetString(rawValues, "user_id"),
            Email = GetObject<EmailPermissions>(rawValues, "email"),
            Address = GetObject<AddressPermissions>(rawValues, "address")
        };

    /*
     |--------------------------------------------------------------------------------
     | Validate
     |--------------------------------------------------------------------------------
     */

    public override AttributeValidationResult Validate(string key, object value)
    {
        return key switch
        {
            "user_id" => value is string
                ? AttributeValidationResult.Success()
                : AttributeValidationResult.Fail($"'user_id' must be a string."),

            "email" => value is EmailPermissions
                ? AttributeValidationResult.Success()
                : TryValidateJson<EmailPermissions>(value),

            "address" => value is AddressPermissions
                ? AttributeValidationResult.Success()
                : TryValidateJson<AddressPermissions>(value),

            _ => AttributeValidationResult.Fail(
                $"Unknown attribute key '{key}' in domain '{Domain}'.")
        };
    }

    /*
     |--------------------------------------------------------------------------------
     | Helpers
     |--------------------------------------------------------------------------------
     */

    private static AttributeValidationResult TryValidateJson<T>(object value)
    {
        if (value is not string raw)
        {
            return AttributeValidationResult.Fail(
                        $"Expected '{typeof(T).Name}' or a JSON string representation.");
        }
        return TryDeserialize<T>(raw, out _)
            ? AttributeValidationResult.Success()
            : AttributeValidationResult.Fail(
                $"Value could not be deserialized as '{typeof(T).Name}'.");
    }
}