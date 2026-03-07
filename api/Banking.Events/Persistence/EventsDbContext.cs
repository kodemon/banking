using Microsoft.EntityFrameworkCore;

namespace Banking.Events.Persistence;

/*
 |--------------------------------------------------------------------------------
 | EventsDbContext
 |--------------------------------------------------------------------------------
 |
 | Owns two concerns:
 |
 |   1. StoredEvent — the append-only domain event log.
 |
 |   2. Saga state tables — add a public DbSet<T> here for any future Saga
 |      type. Wolverine discovers the correct DbContext by scanning registered
 |      DbContext types for a matching DbSet<T> where T : Saga. No special
 |      Wolverine calls needed in OnModelCreating — just normal EF mappings.
 |
 | Wolverine's outbox envelope tables are created directly via
 | UseSqlitePersistence() in Program.cs and live in events.db outside
 | of EF Core migrations.
 |
 | This context never shares a connection with domain DbContexts. Each
 | domain writes its read models to its own SQLite file.
 |
 */

public class EventsDbContext : DbContext
{
    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options) { }

    internal DbSet<StoredEvent> Events => Set<StoredEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
