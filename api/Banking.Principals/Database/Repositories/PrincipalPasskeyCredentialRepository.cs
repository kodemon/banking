using Banking.Principals.Database;
using Banking.Principals.Database.Models;
using Banking.Principals.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Banking.Principals.Repositories;

internal class PrincipalPasskeyCredentialRepository(PrincipalDbContext db)
    : IPasskeyCredentialRepository
{
    public Task<PrincipalPasskeyCredential?> GetByCredentialIdAsync(
        string credentialId,
        CancellationToken ct = default
    ) =>
        db.PrincipalPasskeyCredentials.FirstOrDefaultAsync(c => c.CredentialId == credentialId, ct);

    public Task<List<PrincipalPasskeyCredential>> GetByPrincipalIdAsync(
        Guid principalId,
        CancellationToken ct = default
    ) => db.PrincipalPasskeyCredentials.Where(c => c.PrincipalId == principalId).ToListAsync(ct);

    public async Task AddAsync(
        PrincipalPasskeyCredential credential,
        CancellationToken ct = default
    ) => await db.PrincipalPasskeyCredentials.AddAsync(credential, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
