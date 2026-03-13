using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Banking.Api.Identity;
using Banking.Api.Identity.Bff;
using Microsoft.AspNetCore.Mvc;

/*
 |--------------------------------------------------------------------------------
 | Auth Controller
 |--------------------------------------------------------------------------------
 |
 | BFF authentication endpoints.
 |
 | GET  /auth/login    — Builds the Zitadel authorize URL and redirects the
 |                       browser.
 | GET  /auth/callback — Exchanges the code, stores tokens in Valkey, sets
 |                       session cookie.
 | POST /auth/logout   — Deletes the Valkey session, clears the cookie, redirects
 |                       to Zitadel end-session.
 | GET  /auth/session  — Returns 200 if a valid session cookie exists, 401
 |                       otherwise.
 |
 | PKCE state is stored server-side in Valkey (keyed by the OIDC state parameter)
 | rather than in a cookie. This allows SameSite=Strict on all cookies because
 | no cookie needs to survive the cross-site redirect from Zitadel back to
 | /auth/callback.
 |
 */

[ApiController]
[Route("auth")]
[Tags("Auth")]
internal class AuthController(
    IBffSessionStore sessionStore,
    IPkceStore pkceStore,
    IZitadelTokenClient tokenClient,
    IConfiguration configuration,
    ILogger<AuthController> logger
) : ControllerBase
{
    // -------------------------------------------------------------------------
    // GET /auth/login
    // -------------------------------------------------------------------------

    [HttpGet("login")]
    public async Task<IActionResult> Login(
        [FromQuery(Name = "return_to")] string? returnTo = null,
        CancellationToken ct = default
    )
    {
        var authority = Require("Zitadel:Authority");
        var clientId = Require("Zitadel:ClientId");
        var redirectUri = Require("Bff:RedirectUri");
        var scope = configuration["Zitadel:Scope"] ?? "openid profile email offline_access";

        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(codeVerifier);

        // state carries two things: an opaque key for the Valkey PKCE lookup,
        // and the return path — encoded together so Zitadel passes it back intact.
        var stateKey = GenerateStateKey();
        var returnPath = string.IsNullOrWhiteSpace(returnTo) ? "/" : returnTo;

        // Store the PKCE verifier server-side. No cookie needed — the state
        // parameter from Zitadel's redirect is enough to look it up.
        await pkceStore.SetAsync(stateKey, new PkceEntry(codeVerifier, returnPath), ct);

        var authorizeUrl =
            $"{authority}/oauth/v2/authorize"
            + $"?response_type=code"
            + $"&client_id={Uri.EscapeDataString(clientId)}"
            + $"&redirect_uri={Uri.EscapeDataString(redirectUri)}"
            + $"&scope={Uri.EscapeDataString(scope)}"
            + $"&code_challenge={codeChallenge}"
            + $"&code_challenge_method=S256"
            + $"&state={Uri.EscapeDataString(stateKey)}";

        return Redirect(authorizeUrl);
    }

    // -------------------------------------------------------------------------
    // GET /auth/callback
    // -------------------------------------------------------------------------

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(
        [FromQuery] string code,
        [FromQuery] string state,
        CancellationToken ct
    )
    {
        // Retrieve and immediately delete the PKCE entry — single use.
        var pkce = await pkceStore.GetAsync(state, ct);
        await pkceStore.DeleteAsync(state, ct);

        if (pkce is null)
        {
            return BadRequest("PKCE state missing or expired. Please try logging in again.");
        }

        // Exchange the authorization code for tokens.
        var tokens = await tokenClient.ExchangeCodeAsync(code, pkce.CodeVerifier, ct);

        // Extract the 'sub' claim directly from the freshly issued access token.
        var sub = ExtractSub(tokens.AccessToken);
        var aud = ExtractClaim(tokens.AccessToken, "aud");

        logger.LogInformation(
            "[Auth:Callback] token_type={TokenType}, sub={Sub}, aud={Aud}, token_preview={Preview}",
            tokens.AccessToken.Contains('.') ? "JWT" : "opaque",
            sub,
            aud,
            tokens.AccessToken[..Math.Min(40, tokens.AccessToken.Length)]
        );

        // Persist the session in Valkey — tokens never leave the server.
        var sessionId = GenerateSessionId();
        await sessionStore.SetAsync(
            sessionId,
            new BffSession
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokens.ExpiresIn),
                Sub = sub,
            },
            ct
        );

        // SameSite=Strict is safe here because this cookie is only ever set and
        // read within the same domain — the Zitadel redirect lands on the server
        // endpoint (/auth/callback), which sets the cookie in its response.
        // The browser then makes its next same-site request with the cookie present.
        Response.Cookies.Append(
            AuthMiddleware.SessionCookieName,
            sessionId,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                // Browser max-age matches the Valkey TTL so the browser does not
                // prune the cookie before Valkey expires the session.
                MaxAge = TimeSpan.FromDays(configuration.GetValue<int>("Bff:SessionTtlDays", 7)),
            }
        );

        var returnTo = Uri.IsWellFormedUriString(pkce.ReturnPath, UriKind.Relative)
            ? pkce.ReturnPath
            : "/";

        return Redirect(returnTo);
    }

    // -------------------------------------------------------------------------
    // POST /auth/logout
    // -------------------------------------------------------------------------

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var sessionId = Request.Cookies[AuthMiddleware.SessionCookieName];

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            await sessionStore.DeleteAsync(sessionId, ct);
        }

        Response.Cookies.Delete(
            AuthMiddleware.SessionCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            }
        );

        var authority = Require("Zitadel:Authority");
        var clientId = Require("Zitadel:ClientId");
        var postLogoutUri = Require("Bff:PostLogoutRedirectUri");

        var endSessionUrl =
            $"{authority}/oidc/v1/end_session"
            + $"?client_id={Uri.EscapeDataString(clientId)}"
            + $"&post_logout_redirect_uri={Uri.EscapeDataString(postLogoutUri)}";

        return Redirect(endSessionUrl);
    }

    // -------------------------------------------------------------------------
    // GET /auth/session
    // -------------------------------------------------------------------------

    [HttpGet("session")]
    public async Task<IActionResult> Session(CancellationToken ct)
    {
        var sessionId = Request.Cookies[AuthMiddleware.SessionCookieName];
        if (string.IsNullOrWhiteSpace(sessionId))
            return Unauthorized();

        var session = await sessionStore.GetAsync(sessionId, ct);
        if (session is null || session.IsExpired)
            return Unauthorized();

        // Ensure the JWT was actually validated by the middleware.
        if (!(HttpContext.User.Identity?.IsAuthenticated ?? false))
            return Unauthorized();

        return Ok(new { session.Sub });
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private string Require(string key) =>
        configuration[key]
        ?? throw new InvalidOperationException($"Configuration key '{key}' is not set.");

    private static string GenerateSessionId() => GenerateUrlSafeToken(32);

    private static string GenerateStateKey() => GenerateUrlSafeToken(16);

    private static string GenerateUrlSafeToken(int byteLength) =>
        Convert
            .ToBase64String(RandomNumberGenerator.GetBytes(byteLength))
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

    private static string GenerateCodeVerifier() => GenerateUrlSafeToken(64);

    private static string GenerateCodeChallenge(string verifier)
    {
        var bytes = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    private static string ExtractSub(string jwt) => ExtractClaim(jwt, "sub");

    private static string ExtractClaim(string jwt, string claim)
    {
        var parts = jwt.Split('.');
        if (parts.Length < 2)
            return string.Empty;

        var payload = parts[1]
            .PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=')
            .Replace('-', '+')
            .Replace('_', '/');

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty(claim, out var value))
            return string.Empty;

        // aud can be a string or an array
        return value.ValueKind == JsonValueKind.Array
            ? string.Join(", ", value.EnumerateArray().Select(v => v.GetString()))
            : value.GetString() ?? string.Empty;
    }
}
