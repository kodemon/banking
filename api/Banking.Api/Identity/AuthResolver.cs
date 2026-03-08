using Banking.Api.Exceptions;
using Banking.Principals.Commands;
using Banking.Principals.Queries;
using Banking.Principals.Repositories.Resources;
using Cerbos.Sdk;
using MediatR;

namespace Banking.Api.Identity;

internal interface IAuthResolver
{
    Task<IAuth> Resolve(IServiceProvider sp);
}

internal class ZitadelAuthResolver(
    IHttpContextAccessor httpContextAccessor,
    IMediator mediator,
    IConfiguration configuration,
    ICerbosClient cerbos
) : IAuthResolver
{
    public async Task<IAuth> Resolve(IServiceProvider sp)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null || !(httpContext.User.Identity?.IsAuthenticated ?? false))
        {
            throw new UnauthorizedException("No authenticated session found.");
        }

        var sub = httpContext.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub))
        {
            throw new UnauthorizedException("JWT is missing the required 'sub' claim.");
        }

        var issuer =
            httpContext.User.FindFirst("iss")?.Value
            ?? configuration["Zitadel:Authority"]
            ?? throw new UnauthorizedException("Cannot determine identity provider issuer.");

        var principal = await mediator.Send(new GetPrincipalByProviderQuery(issuer, sub));
        if (principal is not null)
        {
            return new Auth(ParsePrincipal(principal), cerbos);
        }

        principal = await mediator.Send(new CreatePrincipalCommand(issuer, sub));
        if (principal is not null)
        {
            return new Auth(ParsePrincipal(principal), cerbos);
        }

        throw new UnauthorizedException(
            $"Unable to resolve principal for identity '{issuer}' / '{sub}'."
        );
    }

    private ResolvedPrincipal ParsePrincipal(Principal principal) =>
        new(
            principal.Id,
            principal
                .Identities.Select(i => new PrincipalIdentity(i.Provider, i.ExternalId))
                .ToList(),
            principal.Roles.Select(r => r.Role).ToList(),
            new PrincipalAttributes()
        );
}
