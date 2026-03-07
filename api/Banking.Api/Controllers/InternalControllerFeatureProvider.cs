using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Banking.Api;

/*
 |--------------------------------------------------------------------------------
 | InternalControllerFeatureProvider
 |--------------------------------------------------------------------------------
 |
 | By default ASP.NET Core only discovers public controllers. This provider
 | extends discovery to include internal controllers, allowing all domain
 | types — services, repositories, controllers — to stay internal and prevent
 | accidental cross-module coupling.
 |
 */

internal class InternalControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo) =>
        typeInfo.IsClass
        && !typeInfo.IsAbstract
        && !typeInfo.IsGenericTypeDefinition
        && !typeInfo.ContainsGenericParameters
        && typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(typeInfo);
}
