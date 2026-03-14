using Banking.Principals.Database.Models;

namespace Banking.Principals.Interfaces;

internal interface IPrincipalPasskeyCredentialRepository
{
    Task<PrincipalPasskeyCredential?> GetByCredentialIdAsync(
        string credentialId,
        CancellationToken ct = default
    );
    Task<List<PrincipalPasskeyCredential>> GetByPrincipalIdAsync(
        Guid principalId,
        CancellationToken ct = default
    );
    Task AddAsync(PrincipalPasskeyCredential credential, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
