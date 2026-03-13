using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Banking.Api.Identity.Bff;

/*
 |--------------------------------------------------------------------------------
 | Zitadel Token Client
 |--------------------------------------------------------------------------------
 |
 | ...
 |
 */

internal interface IZitadelTokenClient
{
    Task<TokenResponse> ExchangeCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken ct = default
    );

    Task<TokenResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
}

internal class ZitadelTokenClient(HttpClient http, IConfiguration configuration)
    : IZitadelTokenClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private string Authority =>
        configuration["Zitadel:Authority"]
        ?? throw new InvalidOperationException("Zitadel:Authority not configured");

    private string ClientId =>
        configuration["Zitadel:ClientId"]
        ?? throw new InvalidOperationException("Zitadel:ClientId not configured");

    private string ClientSecret =>
        configuration["Zitadel:ClientSecret"]
        ?? throw new InvalidOperationException("Zitadel:ClientSecret not configured");

    private string RedirectUri =>
        configuration["Bff:RedirectUri"]
        ?? throw new InvalidOperationException("Bff:RedirectUri not configured");

    public async Task<TokenResponse> ExchangeCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken ct = default
    )
    {
        var body = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = RedirectUri,
            ["code_verifier"] = codeVerifier,
            ["client_id"] = ClientId,
            ["client_secret"] = ClientSecret,
        };

        return await PostTokenAsync(body, ct);
    }

    public async Task<TokenResponse> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default
    )
    {
        var body = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = ClientId,
            ["client_secret"] = ClientSecret,
        };

        return await PostTokenAsync(body, ct);
    }

    private async Task<TokenResponse> PostTokenAsync(
        Dictionary<string, string> body,
        CancellationToken ct
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{Authority}/oauth/v2/token")
        {
            Content = new FormUrlEncodedContent(body),
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await http.SendAsync(request, ct);
        var json = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Zitadel token endpoint returned {response.StatusCode}: {json}"
            );
        }

        return JsonSerializer.Deserialize<TokenResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException("Zitadel returned an empty token response");
    }
}

internal record TokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }
}
