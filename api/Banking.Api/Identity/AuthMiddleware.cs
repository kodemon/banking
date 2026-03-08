namespace Banking.Api.Identity;

internal class AuthMiddleware(RequestDelegate next)
{
    private const string AuthKey = "IAuth";

    public async Task InvokeAsync(HttpContext context, IAuthResolver resolver)
    {
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            var auth = await resolver.Resolve(context.RequestServices);
            context.Items[AuthKey] = auth;
        }

        await next(context);
    }

    public static IAuth GetAuth(HttpContext context)
    {
        return context.Items[AuthKey] as IAuth
            ?? throw new InvalidOperationException(
                "IAuth has not been resolved for this request. Ensure UseAuthMiddleware() is registered after UseAuthentication()."
            );
    }
}
