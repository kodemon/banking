namespace Banking.Shared.AccessControl;

public record AttributeValidationResult
{
    public bool IsValid { get; init; }
    public string? Error { get; init; }

    public static AttributeValidationResult Success() => new() { IsValid = true };
    public static AttributeValidationResult Fail(string error) => new() { IsValid = false, Error = error };
}