namespace Banking.Api.Identity.Bff;

internal record BffSession
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }

    public required string Sub { get; init; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
    public bool IsExpiringSoon => DateTimeOffset.UtcNow >= ExpiresAt.AddSeconds(-30);
}
