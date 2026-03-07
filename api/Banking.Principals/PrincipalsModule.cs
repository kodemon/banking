using Banking.AtomicFlow;
using Banking.Principals.AccessControl;
using Banking.Principals.Database;
using Banking.Principals.Repositories;
using Banking.Shared.AccessControl;
using Banking.Shared.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zitadel.Credentials;

namespace Banking.Principals;

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
    public static IServiceCollection AddPrincipalsModule(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<PrincipalDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("principals"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );

        services.AddScoped<IPrincipalRepository, PrincipalRepository>();
        services.AddScoped<IPrincipalIdentityRepository, PrincipalIdentityRepository>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(PrincipalsModule).Assembly)
        );

        services.AddRollbackRegistrations(typeof(PrincipalsModule).Assembly);

        /*
         |--------------------------------------------------------------------------------
         | Zitadel
         |--------------------------------------------------------------------------------
         */

        var authority = configuration["Zitadel:Authority"]!;
        var keyFilePath = configuration["Zitadel:ServiceAccountKeyPath"]!;
        var serviceAccount = ServiceAccount.LoadFromJsonFile(keyFilePath);

        services.AddSingleton(serviceAccount);
        services
            .AddHttpClient<ZitadelMetadataService>(client =>
            {
                client.BaseAddress = new Uri(authority);
            })
            .AddTypedClient(
                (http, sp) =>
                    new ZitadelMetadataService(
                        http,
                        sp.GetRequiredService<ServiceAccount>(),
                        authority
                    )
            );

        /*
         |--------------------------------------------------------------------------------
         | Principal Resolver
         |--------------------------------------------------------------------------------
         |
         | Collects all registered domain resolvers. Singleton lifetime matches the
         | resolver registrations from each domain module.
         |
         */

        services.AddSingleton(sp =>
        {
            var resolvers = sp.GetServices<IAccessAttributeResolver>();
            return new PrincipalResolver(resolvers);
        });

        services.AddScoped<PrincipalProvisioner>();
        services.AddScoped<PrincipalContext>();
        services.AddScoped<IPrincipalContext>(sp => sp.GetRequiredService<PrincipalContext>());

        services.AddScoped<IClaimsTransformation, ZitadelClaimsTransformation>();

        return services;
    }
}
