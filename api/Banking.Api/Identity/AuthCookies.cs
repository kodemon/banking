namespace Banking.Api.Identity;

internal static class AuthCookies
{
    public const string SessionCookieName = "session";

    public static bool TryGetSessionId(HttpRequest request, out Guid sessionId)
    {
        var raw = request.Cookies[SessionCookieName];
        if (!string.IsNullOrWhiteSpace(raw) && Guid.TryParse(raw, out sessionId))
        {
            return true;
        }
        sessionId = Guid.Empty;
        return false;
    }

    public static void SetSessionCookie(HttpResponse response, Guid sessionId) =>
        response.Cookies.Append(
            SessionCookieName,
            sessionId.ToString(),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(30),
            }
        );

    public static void ClearSessionCookie(HttpContext context)
    {
        context.Response.Cookies.Delete(
            SessionCookieName,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            }
        );
    }
}
