using Banking.Principals.EventStore.Events;
using Banking.Principals.Repositories;
using Wolverine.Attributes;

namespace Banking.Principals.Handlers;

[WolverineHandler]
public class PrincipalEventHandlers(PrincipalRepository repository)
{
    public async Task Handle(PrincipalCreated evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    public async Task Handle(PrincipalDeleted evt)
    {
        var principal = await db
            .Principals.Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == evt.PrincipalId);

        if (principal is not null)
            db.Principals.Remove(principal);
    }

    public async Task Handle(PrincipalIdentityAdded evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    public async Task Handle(PrincipalIdentityRemoved evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    public async Task Handle(PrincipalRoleAdded evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    public async Task Handle(PrincipalRoleRemoved evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    public async Task Handle(PrincipalAttributeSet evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    public async Task Handle(PrincipalAttributeRemoved evt)
    {
        var principal = await ReconstitutePrincipal(evt.PrincipalId);
        await UpsertReadModel(principal);
    }

    /*
     |--------------------------------------------------------------------------------
     | Reconstitution
     |--------------------------------------------------------------------------------
     */

    private async Task<Principal> ReconstitutePrincipal(Guid principalId)
    {
        var storedEvents = await db
            .Events.Where(e => e.StreamId == principalId)
            .OrderBy(e => e.Version)
            .ToListAsync();

        var domainEvents = storedEvents
            .Select(Deserialise)
            .Where(e => e is not null)
            .Select(e => e!);

        return Principal.Reconstitute(domainEvents);
    }

    private static object? Deserialise(StoredEvent stored) =>
        PrincipalEventDeserialisers.Deserialise(stored);

    /*
     |--------------------------------------------------------------------------------
     | Read model upsert
     |--------------------------------------------------------------------------------
     |
     | Writes the current aggregate state into the read-model tables.
     | Uses AddOrUpdate semantics: existing rows are updated in place,
     | rows no longer present in the aggregate are removed, new rows are inserted.
     |
     */

    private async Task UpsertReadModel(Principal principal)
    {
        var existing = await db
            .Principals.Include(p => p.Identities)
            .Include(p => p.Roles)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == principal.Id);

        if (existing is null)
        {
            db.Principals.Add(principal);
            return;
        }

        // Sync identities
        SyncCollection(
            existing.Identities,
            principal.Identities,
            (a, b) => a.Id == b.Id,
            add => db.Set<PrincipalIdentity>().Add(add),
            remove => db.Set<PrincipalIdentity>().Remove(remove)
        );

        // Sync roles
        SyncCollection(
            existing.Roles,
            principal.Roles,
            (a, b) => a.Id == b.Id,
            add => db.Set<PrincipalRole>().Add(add),
            remove => db.Set<PrincipalRole>().Remove(remove)
        );

        // Sync attributes — update value in place if key matches
        foreach (var current in principal.Attributes)
        {
            var existingAttr = existing.Attributes.FirstOrDefault(a => a.Id == current.Id);
            if (existingAttr is null)
                db.Set<PrincipalAttribute>().Add(current);
            else if (existingAttr.Value != current.Value)
                existingAttr.Value = current.Value;
        }
        foreach (
            var removed in existing.Attributes.Where(a =>
                principal.Attributes.All(c => c.Id != a.Id)
            )
        )
            db.Set<PrincipalAttribute>().Remove(removed);
    }

    private static void SyncCollection<T>(
        IReadOnlyCollection<T> existing,
        IReadOnlyCollection<T> current,
        Func<T, T, bool> match,
        Action<T> add,
        Action<T> remove
    )
    {
        foreach (var item in current.Where(c => existing.All(e => !match(e, c))))
            add(item);
        foreach (var item in existing.Where(e => current.All(c => !match(e, c))))
            remove(item);
    }
}
