using Banking.Principals.Queries;
using Cerbos.Sdk;
using MediatR;

namespace Banking.Api.Identity;

/*
 |--------------------------------------------------------------------------------
 | Auth Middleware
 |--------------------------------------------------------------------------------
 |
 | Runs on every request before ASP.NET's own authentication pipeline.
 |
 | The browser holds the session ID in a single HttpOnly cookie ("session").
 | On every request this middleware:
 |
 |   1. Reads the cookie and parses the session ID.
 |   2. Looks up the session in the principals database via MediatR.
 |   3. Resolves the application Principal and stores IAuth in context.Items
 |      so downstream controllers can call IAuth.IsAllowed() without
 |      repeating the lookup.
 |
 */

internal class AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
{
    internal const string SessionCookieName = "session";
    private const string AuthKey = "IAuth";

    public async Task InvokeAsync(HttpContext context, IMediator mediator, ICerbosClient cerbos)
    {
        logger.LogInformation(
            "[Auth] ← {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );

        var sessionId = GetSessionId(context.Request);

        if (sessionId.HasValue)
        {
            var session = await mediator.Send(
                new GetSessionByIdQuery(sessionId.Value),
                context.RequestAborted
            );

            if (session is null || session.IsExpired)
            {
                logger.LogWarning(
                    "[Auth] Session invalid or expired — clearing cookie, sessionId={SessionId}",
                    sessionId
                );
                ClearSessionCookie(context);
            }
            else
            {
                var principal = await mediator.Send(
                    new GetPrincipalByIdQuery(session.PrincipalId),
                    context.RequestAborted
                );

                if (principal is null)
                {
                    logger.LogWarning(
                        "[Auth] Session valid but principal not found — principalId={PrincipalId}",
                        session.PrincipalId
                    );
                    ClearSessionCookie(context);
                }
                else
                {
                    context.Items[AuthKey] = new Auth(
                        new ResolvedPrincipal(
                            principal.Id,
                            principal
                                .Identities.Select(i => new PrincipalIdentity(
                                    i.Provider,
                                    i.ExternalId
                                ))
                                .ToList(),
                            principal.Roles.Select(r => r.Role).ToList(),
                            new PrincipalAttributes()
                        ),
                        cerbos
                    );

                    logger.LogInformation(
                        "[Auth] Principal resolved — id={PrincipalId}",
                        principal.Id
                    );
                }
            }
        }
        else
        {
            logger.LogInformation("[Auth] No session cookie — request is unauthenticated");
        }

        await next(context);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    public static IAuth GetAuth(HttpContext context) =>
        context.Items[AuthKey] as IAuth
        ?? throw new InvalidOperationException(
            "IAuth has not been resolved for this request. Ensure AuthMiddleware is registered."
        );

    public static Guid? GetSessionId(HttpRequest request)
    {
        var raw = request.Cookies[SessionCookieName];
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (Guid.TryParse(raw, out var sessionId))
        {
            return sessionId;
        }

        return null;
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

    public static void ClearSessionCookie(HttpContext context) =>
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
