namespace Banking.Principals.Database.Models;

internal class PrincipalPasskeyCredential
{
    public Guid Id { get; init; }
    public Guid PrincipalId { get; init; }

    /// <summary>Base64Url-encoded credential ID issued by the authenticator.</summary>
    public string CredentialId { get; init; }

    /// <summary>CBOR-encoded public key.</summary>
    public byte[] PublicKey { get; init; }

    /// <summary>Signature counter — used to detect cloned authenticators.</summary>
    public uint SignCount { get; set; }

    /// <summary>Human-readable name chosen at registration (e.g. "MacBook Touch ID").</summary>
    public string Name { get; init; }

    /// <summary>AAGUID identifies the authenticator model.</summary>
    public Guid AaGuid { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? LastUsedAt { get; set; }

    public PrincipalPasskeyCredential(
        Guid principalId,
        string credentialId,
        byte[] publicKey,
        uint signCount,
        string name,
        Guid aaGuid
    )
    {
        Id = Guid.NewGuid();
        PrincipalId = principalId;
        CredentialId = credentialId;
        PublicKey = publicKey;
        SignCount = signCount;
        Name = name;
        AaGuid = aaGuid;
        CreatedAt = DateTime.UtcNow;
    }
}
