using Banking.Principals.Database.Models;

namespace Banking.Principals.Interfaces;

internal interface IPrincipalSessionRepository
{
    Task<PrincipalSession?> GetByIdAsync(Guid sessionId, CancellationToken ct = default);
    Task AddAsync(PrincipalSession session, CancellationToken ct = default);
    Task DeleteAsync(Guid sessionId, CancellationToken ct = default);
    Task DeleteExpiredAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
