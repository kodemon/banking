using Banking.Atomic;
using Banking.Principals.Database;
using Banking.Principals.Repositories;
using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Principals;

public static class PrincipalsModule
{
    public static IServiceCollection AddPrincipalsModule(this IServiceCollection services)
    {
        SetupDatabase(services);
        SetupAtomic(services);
        SetupCQRS(services);

        return services;
    }

    public static void SetupDatabase(IServiceCollection services)
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
