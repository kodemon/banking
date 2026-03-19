using Cerbos.Sdk;
using Cerbos.Sdk.Builder;
using Cerbos.Sdk.Utility;

namespace Banking.Api.Identity;

internal interface IAuth
{
    public Principal Principal { get; init; }

    public Task<bool> IsAllowed(
        string resource,
        string id,
        string action,
        Attributes? attributes = null
    );
}

internal class Auth(Principal principal, ICerbosClient cerbos) : IAuth
{
    public Principal Principal { get; init; } = principal;

    public async Task<bool> IsAllowed(
        string resource,
        string id,
        string action,
        Attributes? attributes = null
    )
    {
        var cerbosPrincipal = Cerbos.Sdk.Builder.Principal.NewInstance(
            Principal.Id.ToString(),
            Principal.Roles.ToArray()
        );

        var resourceEntry = ResourceEntry.NewInstance(resource, id).WithActions(action);

        if (attributes is not null)
        {
            resourceEntry = resourceEntry.WithAttributes(attributes.Commit());
        }

        var result = await cerbos.CheckResourcesAsync(
            CheckResourcesRequest
                .NewInstance()
                .WithRequestId(RequestId.Generate())
                .WithPrincipal(cerbosPrincipal)
                .WithResourceEntries(resourceEntry)
        );

        return result.Find(id)?.IsAllowed(action) ?? false;
    }
}
