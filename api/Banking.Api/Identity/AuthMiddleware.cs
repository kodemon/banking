using Banking.Principals.Commands;
using Banking.Principals.Queries;
using Cerbos.Sdk;
using MediatR;

namespace Banking.Api.Identity;

internal class AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IMediator mediator, ICerbosClient cerbos)
    {
        var principal = await GetSessionPrincipal(context, mediator);
        if (principal is not null)
        {
            logger.LogDebug("Principal resolved — id={PrincipalId}", principal.Id);
            context.Items[AuthContext.AuthKey] = new Auth(principal, cerbos);
        }

        await next(context);
    }

    private async Task<Principal?> GetSessionPrincipal(HttpContext context, IMediator mediator)
    {
        if (!AuthCookies.TryGetSessionId(context.Request, out var sessionId))
        {
            logger.LogInformation("No session cookie — request is unauthenticated");
            return null;
        }

        var session = await mediator.Send(
            new GetSessionByIdQuery(sessionId),
            context.RequestAborted
        );

        if (session is null)
        {
            logger.LogWarning(
                "Session invalid — clearing cookie, sessionId={SessionId}",
                sessionId
            );
            AuthCookies.ClearSessionCookie(context);
            return null;
        }

        if (session.IsExpired)
        {
            logger.LogWarning(
                "Session expired — clearing cookie, sessionId={SessionId}",
                sessionId
            );
            await mediator.Send(new DeleteSessionCommand(sessionId), context.RequestAborted);
            AuthCookies.ClearSessionCookie(context);
            return null;
        }

        var principal = await mediator.Send(
            new GetPrincipalByIdQuery(session.PrincipalId),
            context.RequestAborted
        );

        if (principal is null)
        {
            logger.LogWarning(
                "Principal not found — principalId={PrincipalId}",
                session.PrincipalId
            );
            AuthCookies.ClearSessionCookie(context);
            return null;
        }

        return new Principal(
            principal.Id,
            principal
                .Identities.Select(i => new PrincipalIdentity(i.Provider, i.ExternalId))
                .ToList(),
            principal.Roles.Select(r => r.Role).ToList(),
            new PrincipalAttributes()
        );
    }
}
