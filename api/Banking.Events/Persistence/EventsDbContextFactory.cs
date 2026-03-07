using Banking.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Banking.Events.Persistence;

/*
 |--------------------------------------------------------------------------------
 | EventsDbContext Factory
 |--------------------------------------------------------------------------------
 |
 | Used exclusively by EF Core tooling for migration management:
 |
 |   dotnet ef migrations add Events-init \
 |     --project Banking.Events \
 |     --output-dir Persistence/Migrations
 |
 |   dotnet ef database update --project Banking.Events
 |
 | At runtime the context is configured via AddEventsModule() in Program.cs.
 |
 */

public class EventsDbContextFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<EventsDbContext>()
            .UseSqlite(SQLiteConnection.Load("events"))
            .Options;

        return new EventsDbContext(options);
    }
}
