namespace Banking.Principals.EventStore.Aggregates;

/*
 |--------------------------------------------------------------------------------
 | Principal [Aggregate Root]
 |--------------------------------------------------------------------------------
 |
 | Represents an internal security principal. A principal is the canonical
 | identity within this system — external IDP identities are login methods
 | that bind to a principal, not the principal itself.
 |
 | The aggregate reconstitutes itself by replaying its own domain events.
 | There is no separate "load from DB" path — the event stream is the
 | source of truth. The read model (Principals table) is a projection
 | written by the event handler after each append.
 |
 | Apply() methods are intentionally side-effect free — they only mutate
 | in-memory state. Persistence is handled by the event handler.
 |
 */

public class Principal { }
