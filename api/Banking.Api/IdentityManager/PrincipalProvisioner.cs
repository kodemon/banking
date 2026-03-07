using Banking.Api.Commands;
using Banking.Events.Persistence;
using Banking.Principal.AccessControl;
using Banking.Principals.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Banking.Api;

/*
 |--------------------------------------------------------------------------------
 | PrincipalProvisioner
 |--------------------------------------------------------------------------------
 |
 | Implements IPrincipalProvisioner for Banking.Api. Called by
 | ZitadelClaimsTransformation when an authenticated JWT has no matching
 | principal in the read model.
 |
 | Before issuing CreatePrincipalCommand, we query the event store directly
 | for an existing PrincipalCreated event with matching (provider, externalId).
 | This is the correct guard in an event-sourced system — the event log is
 | always consistent, whereas the read-model projection is eventually consistent
 | and must never be used for business-logic decisions.
 |
 | The domain knowledge (what event type to look for, which payload fields to
 | match) lives here in Banking.Api — not in IEventStore, which is pure
 | infrastructure and must remain domain-agnostic.
 |
 */

internal class PrincipalProvisioner(EventsDbContext db, IMessageBus bus) : IPrincipalProvisioner
{
    private static readonly string PrincipalCreatedEventType = typeof(PrincipalCreated).FullName!;

    public async Task ProvisionAsync(string provider, string externalId)
    {
        var alreadyCreated = await db
            .Events.FromSqlRaw(
                """
                SELECT * FROM "Events"
                WHERE "EventType" = {0}
                  AND json_extract("Payload", '$.provider') = {1}
                  AND json_extract("Payload", '$.externalId') = {2}
                LIMIT 1
                """,
                PrincipalCreatedEventType,
                provider,
                externalId
            )
            .AnyAsync();

        if (alreadyCreated)
        {
            return;
        }

        await bus.InvokeAsync(
            new CreatePrincipalCommand(
                CorrelationId: Guid.NewGuid(),
                PrincipalId: Guid.NewGuid(),
                Provider: provider,
                ExternalId: externalId
            )
        );
    }
}
