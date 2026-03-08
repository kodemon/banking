using Cerbos.Sdk;
using Cerbos.Sdk.Builder;
using Cerbos.Sdk.Utility;

namespace Banking.Api.Identity;

internal interface IAuth
{
    public ResolvedPrincipal Principal { get; init; }

    public Task<bool> IsAllowed(string resource, string id, string action);
}

internal class Auth(ResolvedPrincipal principal, ICerbosClient cerbos) : IAuth
{
    public ResolvedPrincipal Principal { get; init; } = principal;

    public async Task<bool> IsAllowed(string resource, string id, string action)
    {
        var cerbosPrincipal = Cerbos.Sdk.Builder.Principal.NewInstance(
            Principal.Id.ToString(),
            Principal.Roles.ToArray()
        );

        var result = await cerbos.CheckResourcesAsync(
            CheckResourcesRequest
                .NewInstance()
                .WithRequestId(RequestId.Generate())
                .WithPrincipal(cerbosPrincipal)
                .WithResourceEntries(ResourceEntry.NewInstance(resource, id).WithActions(action))
        );

        return result.Find(id)?.IsAllowed(action) ?? false;
    }
}
