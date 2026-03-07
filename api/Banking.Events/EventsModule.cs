using Banking.Events.Persistence;
using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.EntityFrameworkCore;

namespace Banking.Events;

/*
 |--------------------------------------------------------------------------------
 | Events Module
 |--------------------------------------------------------------------------------
 |
 | Registers EventsDbContext — the Wolverine infrastructure database.
 |
 | This context owns:
 |   - Wolverine outbox envelope tables (IncomingMessage, OutgoingMessage)
 |   - Wolverine node heartbeat tables (WolverineNode, WolverineNodeActivity)
 |   - Saga state tables (add DbSet<TSaga> here when sagas are introduced)
 |
 | Domain events are NOT stored here. Each domain writes its own event log
 | to its own SQLite file via EventStore<TContext>.
 |
 | Call order in Program.cs:
 |   1. builder.Services.AddEventsModule()
 |   2. builder.Host.UseWolverine(opts => {
 |          opts.PersistMessagesWithSqlite(connStr)
 |          opts.UseEntityFrameworkCoreTransactions()
 |      })
 |
 */

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(this IServiceCollection services)
    {
        services.AddDbContextWithWolverineIntegration<EventsDbContext>(options =>
            options.UseSqlite(
                SQLiteConnection.Load("events"),
                sqliteOptions =>
                    sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
        return services;
    }
}
