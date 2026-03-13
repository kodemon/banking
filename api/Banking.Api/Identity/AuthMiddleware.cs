using System.Text.Encodings.Web;
using Banking.Api.Exceptions;
using Banking.Api.Identity.Bff;
using Banking.Principals.Commands;
using Banking.Principals.Queries;
using Cerbos.Sdk;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Banking.Api.Identity;

internal class AuthMiddleware(
    RequestDelegate next,
    IConfiguration configuration,
    ILogger<AuthMiddleware> logger,
    IOptionsMonitor<JwtBearerOptions> jwtOptions
)
{
    internal const string SessionCookieName = "bff_session";
    private const string AuthKey = "IAuth";

    public async Task InvokeAsync(
        HttpContext context,
        IBffSessionStore store,
        IZitadelTokenClient tokenClient,
        IMediator mediator,
        ICerbosClient cerbos
    )
    {
        logger.LogInformation(
            "[Auth] ← {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );

        // Step 1: resolve Bearer token — from session cookie or existing Authorization header.
        var token = await ResolveTokenAsync(context, store, tokenClient);

        // Step 2: validate the JWT directly if we have a token.
        if (!string.IsNullOrEmpty(token))
        {
            var principal = await ValidateTokenAsync(token, context.RequestAborted);
            if (principal is not null)
            {
                context.User = principal;
                logger.LogInformation(
                    "[Auth] JWT valid — sub={Sub}, iss={Iss}",
                    context.User.FindFirst("sub")?.Value,
                    context.User.FindFirst("iss")?.Value
                );
            }
            else
            {
                logger.LogWarning("[Auth] JWT validation failed");
            }
        }

        // Step 3: resolve or create the principal.
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            var sub = context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(sub))
                throw new UnauthorizedException("JWT is missing the required 'sub' claim.");

            var issuer =
                context.User.FindFirst("iss")?.Value
                ?? configuration["Zitadel:Authority"]
                ?? throw new UnauthorizedException("Cannot determine identity provider issuer.");

            logger.LogInformation(
                "[Auth] Looking up principal — issuer={Issuer}, sub={Sub}",
                issuer,
                sub
            );
            var dbPrincipal = await mediator.Send(new GetPrincipalByProviderQuery(issuer, sub));
            logger.LogInformation(
                "[Auth] GetPrincipalByProviderQuery result: {Found}",
                dbPrincipal is not null ? "found" : "not found"
            );

            if (dbPrincipal is null)
            {
                logger.LogInformation(
                    "[Auth] Creating principal for issuer={Issuer}, sub={Sub}",
                    issuer,
                    sub
                );
                dbPrincipal = await mediator.Send(new CreatePrincipalCommand(issuer, sub));
                logger.LogInformation(
                    "[Auth] CreatePrincipalCommand result: {Result}",
                    dbPrincipal is not null ? $"created id={dbPrincipal.Id}" : "null"
                );
            }

            if (dbPrincipal is null)
            {
                logger.LogError(
                    "[Auth] Principal could not be resolved or created — rejecting request"
                );
                throw new UnauthorizedException(
                    $"Unable to resolve principal for identity '{issuer}' / '{sub}'."
                );
            }

            context.Items[AuthKey] = new Auth(
                new ResolvedPrincipal(
                    dbPrincipal.Id,
                    dbPrincipal
                        .Identities.Select(i => new PrincipalIdentity(i.Provider, i.ExternalId))
                        .ToList(),
                    dbPrincipal.Roles.Select(r => r.Role).ToList(),
                    new PrincipalAttributes()
                ),
                cerbos
            );

            logger.LogInformation("[Auth] Principal resolved — id={PrincipalId}", dbPrincipal.Id);
        }
        else
        {
            logger.LogInformation(
                "[Auth] Request is unauthenticated — skipping principal resolution"
            );
        }

        await next(context);
    }

    public static IAuth GetAuth(HttpContext context) =>
        context.Items[AuthKey] as IAuth
        ?? throw new InvalidOperationException(
            "IAuth has not been resolved for this request. Ensure AuthMiddleware is registered."
        );

    private async Task<string?> ResolveTokenAsync(
        HttpContext context,
        IBffSessionStore store,
        IZitadelTokenClient tokenClient
    )
    {
        var sessionId = context.Request.Cookies[SessionCookieName];
        logger.LogInformation(
            "[Auth] Session cookie present: {Present}",
            !string.IsNullOrWhiteSpace(sessionId)
        );

        if (string.IsNullOrWhiteSpace(sessionId))
            return null;

        var session = await store.GetAsync(sessionId, context.RequestAborted);

        if (session is null)
        {
            logger.LogWarning(
                "[Auth] Session {SessionId} not found in Valkey — clearing cookie",
                sessionId
            );
            context.Response.Cookies.Delete(SessionCookieName);
            throw new UnauthorizedException("Session not found or expired. Please log in again.");
        }

        logger.LogInformation(
            "[Auth] Session loaded — sub={Sub}, expiresAt={ExpiresAt}, isExpired={IsExpired}, isExpiringSoon={IsExpiringSoon}",
            session.Sub,
            session.ExpiresAt,
            session.IsExpired,
            session.IsExpiringSoon
        );

        if (session.IsExpired || session.IsExpiringSoon)
        {
            logger.LogInformation("[Auth] Refreshing session {SessionId}", sessionId);
            session = await RefreshSessionAsync(
                sessionId,
                session,
                store,
                tokenClient,
                context.RequestAborted
            );
            logger.LogInformation(
                "[Auth] Session refreshed, new expiresAt={ExpiresAt}",
                session.ExpiresAt
            );
        }

        logger.LogInformation("[Auth] Token resolved, length={Length}", session.AccessToken.Length);
        return session.AccessToken;
    }

    private async Task<System.Security.Claims.ClaimsPrincipal?> ValidateTokenAsync(
        string token,
        CancellationToken ct
    )
    {
        var options = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme);

        // Ensure signing keys are loaded from the OIDC discovery document.
        if (options.ConfigurationManager is not null)
        {
            var config = await options.ConfigurationManager.GetConfigurationAsync(ct);
            options.TokenValidationParameters.IssuerSigningKeys = config.SigningKeys;
        }

        foreach (var validator in options.TokenHandlers)
        {
            try
            {
                var result = await validator.ValidateTokenAsync(
                    token,
                    options.TokenValidationParameters
                );
                if (result.IsValid)
                    return new System.Security.Claims.ClaimsPrincipal(result.ClaimsIdentity);

                logger.LogWarning(
                    "[Auth] Token handler validation failed: {Reason}",
                    result.Exception?.Message
                );
            }
            catch (Exception ex)
            {
                logger.LogWarning("[Auth] Token handler threw: {Message}", ex.Message);
            }
        }

        return null;
    }

    private static async Task<BffSession> RefreshSessionAsync(
        string sessionId,
        BffSession session,
        IBffSessionStore store,
        IZitadelTokenClient tokenClient,
        CancellationToken ct
    )
    {
        try
        {
            var tokens = await tokenClient.RefreshAsync(session.RefreshToken, ct);

            var refreshed = session with
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokens.ExpiresIn),
            };

            await store.SetAsync(sessionId, refreshed, ct);
            return refreshed;
        }
        catch
        {
            await store.DeleteAsync(sessionId, ct);
            throw new UnauthorizedException("Session expired. Please log in again.");
        }
    }
}
