using Banking.Principal.AccessControl;
using Banking.Principal.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Principal;

/*
 |--------------------------------------------------------------------------------
 | Principal Module
 |--------------------------------------------------------------------------------
 |
 | Single public entry point for the Banking.Principal module.
 |
 | PrincipalResolver is registered as a singleton and receives all
 | IAccessAttributeResolver implementations at construction time —
 | one per domain that has called services.AddSingleton<IAccessAttributeResolver>
 | in its own module registration.
 |
 | AddPrincipalModule() must be called after all domain modules so that
 | all resolvers are registered in DI before PrincipalResolver is constructed.
 |
 */

public static class PrincipalsModule
{
    public static IServiceCollection AddPrincipalsModule(this IServiceCollection services)
    {
        var moduleDirectory = Path.GetDirectoryName(
            typeof(PrincipalsModule).Assembly.Location)!;

        var dbPath = Path.Combine(moduleDirectory, "banking-principals.db");

        services.AddDbContext<PrincipalDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
        );

        services.AddScoped<IPrincipalRepository, PrincipalRepository>();
        services.AddScoped<PrincipalService>();

        /*
         |--------------------------------------------------------------------------------
         | Principal Resolver
         |--------------------------------------------------------------------------------
         |
         | Collects all registered domain resolvers. Singleton lifetime matches the
         | resolver registrations from each domain module.
         |
         */

        services.AddSingleton<PrincipalResolver>(sp =>
        {
            var resolvers = sp.GetServices<Banking.Shared.AccessControl.IAccessAttributeResolver>();
            return new PrincipalResolver(resolvers);
        });

        // Principal context — scoped so it's per-request
        services.AddScoped<PrincipalContext>();
        services.AddScoped<IPrincipalContext>(sp => sp.GetRequiredService<PrincipalContext>());

        // Claims transformation — runs automatically after JWT validation
        services.AddScoped<IClaimsTransformation, ZitadelClaimsTransformation>();

        return services;
    }
}