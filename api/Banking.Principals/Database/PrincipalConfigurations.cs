using System.Text.Json;
using Banking.Principals.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Principals.Database;

internal class PrincipalConfiguration : IEntityTypeConfiguration<Principal>
{
    public void Configure(EntityTypeBuilder<Principal> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder
            .HasMany(p => p.Identities)
            .WithOne()
            .HasForeignKey(i => i.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Roles)
            .WithOne()
            .HasForeignKey(r => r.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(p => p.Attributes)
            .HasColumnType("TEXT")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v =>
                    JsonSerializer.Deserialize<Dictionary<string, object>>(
                        v,
                        JsonSerializerOptions.Default
                    ) ?? new Dictionary<string, object>()
            )
            .IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired();
    }
}

internal class PrincipalIdentityConfiguration : IEntityTypeConfiguration<PrincipalIdentity>
{
    public void Configure(EntityTypeBuilder<PrincipalIdentity> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Provider).HasMaxLength(100).IsRequired();
        builder.Property(i => i.ExternalId).HasMaxLength(256).IsRequired();

        builder.Property(i => i.CreatedAt).IsRequired();

        builder.HasIndex(i => new { i.Provider, i.ExternalId }).IsUnique();
        builder.HasIndex(i => i.PrincipalId);
    }
}

internal class PasskeyCredentialConfiguration : IEntityTypeConfiguration<PrincipalPasskeyCredential>
{
    public void Configure(EntityTypeBuilder<PrincipalPasskeyCredential> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.CredentialId).HasMaxLength(512).IsRequired();
        builder.Property(c => c.PublicKey).IsRequired();
        builder.Property(c => c.SignCount).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.AaGuid).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.CredentialId).IsUnique();
        builder.HasIndex(c => c.PrincipalId);
    }
}

internal class SessionConfiguration : IEntityTypeConfiguration<PrincipalSession>
{
    public void Configure(EntityTypeBuilder<PrincipalSession> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.PrincipalId).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.ExpiresAt).IsRequired();

        builder.HasIndex(s => s.PrincipalId);
        builder.HasIndex(s => s.ExpiresAt);
    }
}

internal class PrincipalRoleConfiguration : IEntityTypeConfiguration<PrincipalRole>
{
    public void Configure(EntityTypeBuilder<PrincipalRole> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Role).HasMaxLength(100).IsRequired();

        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasIndex(r => new { r.PrincipalId, r.Role }).IsUnique();
    }
}
