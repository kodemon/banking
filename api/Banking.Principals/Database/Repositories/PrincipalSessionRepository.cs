using Banking.Principals.Database;
using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Repositories;

internal class PrincipalSessionRepository(PrincipalDbContext db) : IPrincipalSessionRepository
{
    public Task<PrincipalSession?> GetByIdAsync(Guid sessionId, CancellationToken ct = default) =>
        db.PrincipalSessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);

    public async Task AddAsync(PrincipalSession session, CancellationToken ct = default) =>
        await db.PrincipalSessions.AddAsync(session, ct);

    public async Task DeleteAsync(Guid sessionId, CancellationToken ct = default)
    {
        var session = await db.PrincipalSessions.FindAsync([sessionId], ct);
        if (session is not null)
        {
            db.PrincipalSessions.Remove(session);
        }
    }

    public Task DeleteExpiredAsync(CancellationToken ct = default) =>
        db.PrincipalSessions.Where(s => s.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
