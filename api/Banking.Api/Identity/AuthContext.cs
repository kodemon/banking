namespace Banking.Api.Identity;

internal static class AuthContext
{
    public const string AuthKey = "IAuth";

    public static bool TryGetAuth(HttpContext context, out IAuth auth)
    {
        var raw = context.Items[AuthKey] as IAuth;
        auth = raw!;
        return raw is not null;
    }

    public static IAuth GetAuth(HttpContext context)
    {
        if (!TryGetAuth(context, out var auth))
        {
            throw new InvalidOperationException(
                "IAuth has not been resolved for this request. Ensure AuthMiddleware is registered."
            );
        }
        return auth;
    }
}
