using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Banking.Api.Identity;

/// <summary>
/// Replaces [Authorize] for controllers that use the custom AuthMiddleware.
/// Returns 401 if no valid session was resolved for the request.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class RequireSessionAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        if (context.HttpContext.Items["IAuth"] is not IAuth)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}
