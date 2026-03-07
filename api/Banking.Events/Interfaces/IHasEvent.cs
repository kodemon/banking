using Microsoft.EntityFrameworkCore;

namespace Banking.Events.Interface;

/*
 |--------------------------------------------------------------------------------
 | IHasEvents
 |--------------------------------------------------------------------------------
 |
 | Implemented by each domain's DbContext to expose its StoredEvent table.
 | EventStore takes IHasEvents directly — no generic DbContext type parameter
 | is needed, which means domain DbContexts can remain internal to their
 | own assembly while still being usable from Banking.Api command handlers.
 |
 */

public interface IHasEvents
{
    DbSet<StoredEvent> Events { get; }
}
