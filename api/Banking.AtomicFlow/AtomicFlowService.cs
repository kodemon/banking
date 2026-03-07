using Banking.AtomicFlow.Interfaces;
using Banking.AtomicFlow.Persistence;
using Banking.AtomicFlow.Repositories;
using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Banking.AtomicFlow;

/*
 |--------------------------------------------------------------------------------
 | AtomicFlow Service
 |--------------------------------------------------------------------------------
 |
 | Infrastructure registration for the AtomicFlow cross-domain transaction
 | coordinator. Not a domain module — AtomicFlow has no bounded context of
 | its own and exists solely to support atomic execution across domains.
 |
 | AddAtomicFlowService() should be called before any domain module that
 | needs to register rollback handlers via IAtomicFlowRollbackRegistry.
 |
 | Rollback handlers are registered per domain at startup:
 |
 |   services.AddAtomicFlowService();
 |
 |   services.AddSingleton<IAtomicFlowRollbackRegistry>(sp =>
 |   {
 |       var registry = sp.GetRequiredService<AtomicFlowRollbackRegistry>();
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

public static class AtomicFlowService
{
    public static IServiceCollection AddAtomicFlowService(this IServiceCollection services)
    {
        services.AddDbContext<AtomicFlowDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("atomic-flow"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );

        services.AddScoped<IAtomicFlowRepository, AtomicFlowRepository>();

        services.AddSingleton<AtomicFlowRollbackRegistry>();
        services.AddSingleton<IAtomicFlowRollbackRegistry>(sp =>
            sp.GetRequiredService<AtomicFlowRollbackRegistry>()
        );

        services.AddScoped<AtomicFlow>();

        services.AddHostedService<AtomicFlowRecoveryService>();

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
     | Call once per domain module assembly after AddAtomicFlowService():
     |
     |   services.AddAtomicFlowService();
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
