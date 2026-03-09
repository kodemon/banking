using Banking.Atomic.Interfaces;
using Banking.Atomic.Libraries;
using Banking.Atomic.Persistence;
using Banking.Atomic.Repositories;
using Banking.Atomic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.Atomic;

/*
 |--------------------------------------------------------------------------------
 | Atomic Service
 |--------------------------------------------------------------------------------
 |
 | Infrastructure registration for the Atomic cross-domain transaction
 | coordinator. Not a domain module — Atomic has no bounded context of
 | its own and exists solely to support atomic execution across domains.
 |
 | AddAtomicService() should be called before any domain module that
 | needs to register rollback handlers via IAtomicRollbackRegistry.
 |
 | Rollback handlers are registered per domain at startup:
 |
 |   services.AddAtomicService();
 |
 |   services.AddSingleton<IAtomicRollbackRegistry>(sp =>
 |   {
 |       var registry = sp.GetRequiredService<AtomicRollbackRegistry>();
 |       var userService = sp.GetRequiredService<UserService>();
 |
 |       registry.Register<CreateUserRollback>("create-user",
 |           async (rollback) => await userService.DeleteUserAsync(rollback.Id));
 |
 |       return registry;
 |   });
 |
 */

public sealed record PendingRollbackRegistrations(List<Type> Types);

public static class AtomicService
{
    public static IServiceCollection AddAtomicService(
        this IServiceCollection services,
        string pgConnection
    )
    {
        services.AddDbContext<AtomicDbContext>(options =>
            options.UseNpgsql(
                pgConnection,
                pgOptions => pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );

        services.AddScoped<IAtomicRepository, AtomicRepository>();

        services.AddSingleton<AtomicRollbackRegistry>();
        services.AddSingleton<IAtomicRollbackRegistry>(sp =>
            sp.GetRequiredService<AtomicRollbackRegistry>()
        );

        services.AddScoped<Libraries.Atomic>();

        services.AddHostedService<AtomicRecoveryService>();

        return services;
    }

    /*
     |--------------------------------------------------------------------------------
     | AddRollbackRegistrations
     |--------------------------------------------------------------------------------
     |
     | Scans the given assembly for all IRollbackRegistration implementations
     | and calls their static Register() method.
     |
     | Call once per domain module assembly after AddAtomicService():
     |
     |   services.AddAtomicService();
     |   services.AddRollbackRegistrations(typeof(PrincipalsModule).Assembly);
     |
     */

    public static IServiceCollection AddRollbackRegistrations(
        this IServiceCollection services,
        System.Reflection.Assembly assembly
    )
    {
        var registrationTypes = assembly
            .GetTypes()
            .Where(t =>
                t is { IsAbstract: false, IsInterface: false }
                && t.IsAssignableTo(typeof(IRollbackRegistration))
            )
            .ToList();

        // Store types for eager resolution after the container is built.
        // Registration must be triggered via UseRollbackRegistrations() in Program.cs.
        services.AddSingleton(new PendingRollbackRegistrations(registrationTypes));

        return services;
    }
}
