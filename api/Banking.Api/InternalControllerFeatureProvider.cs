using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Banking.Api;

/*
 |--------------------------------------------------------------------------------
 | Internal Controller Feature Provider
 |--------------------------------------------------------------------------------
 |
 | By default ASP.NET Core only discovers public controllers. Since module
 | controllers are intentionally internal — enforcing that nothing outside the
 | module can reference them directly — we need to tell MVC to also register
 | non-public controller types from the registered application parts.
 |
 */

public class InternalControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeInfo.IsClass) return false;
        if (typeInfo.IsAbstract) return false;
        if (typeInfo.ContainsGenericParameters) return false;
        if (typeInfo.IsDefined(typeof(NonControllerAttribute))) return false;

        return typeInfo.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            || typeInfo.IsDefined(typeof(ControllerAttribute));
    }
}