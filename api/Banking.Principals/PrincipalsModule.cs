using Banking.Atomic;
using Banking.Principals.Database;
using Banking.Principals.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Principals;

public static class PrincipalsModule
{
    public static IServiceCollection AddPrincipalsModule(
        this IServiceCollection services,
        string pgConnection
    )
    {
        SetupDatabase(services, pgConnection);
        SetupAtomic(services);
        SetupCQRS(services);

        return services;
    }

    public static void SetupDatabase(IServiceCollection services, string pgConnection)
    {
        services.AddDbContext<PrincipalDbContext>(options =>
            options.UseNpgsql(
                pgConnection,
                pgOptions => pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
        services.AddScoped<IPrincipalRepository, PrincipalRepository>();
        services.AddScoped<IPrincipalIdentityRepository, PrincipalIdentityRepository>();
    }

    public static void SetupAtomic(IServiceCollection services)
    {
        services.AddRollbackRegistrations(typeof(PrincipalsModule).Assembly);
    }

    public static void SetupCQRS(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(PrincipalsModule).Assembly)
        );
    }
}
