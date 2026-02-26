using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Principal.Persistence.Configurations;

internal class PrincipalConfiguration : IEntityTypeConfiguration<Principal>
{
    public void Configure(EntityTypeBuilder<Principal> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasMany(p => p.Identities)
            .WithOne()
            .HasForeignKey(i => i.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Roles)
            .WithOne()
            .HasForeignKey(r => r.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Attributes)
            .WithOne()
            .HasForeignKey(a => a.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.CreatedAt).IsRequired();
    }
}

internal class PrincipalIdentityConfiguration : IEntityTypeConfiguration<PrincipalIdentity>
{
    public void Configure(EntityTypeBuilder<PrincipalIdentity> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Provider)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.ExternalId)
            .HasMaxLength(256)
            .IsRequired();

        // A given external identity can only be bound to one principal
        builder.HasIndex(i => new { i.Provider, i.ExternalId })
            .IsUnique();

        builder.HasIndex(i => i.PrincipalId);

        builder.Property(i => i.CreatedAt).IsRequired();
    }
}

internal class PrincipalRoleConfiguration : IEntityTypeConfiguration<PrincipalRole>
{
    public void Configure(EntityTypeBuilder<PrincipalRole> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Role)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(r => new { r.PrincipalId, r.Role })
            .IsUnique();

        builder.Property(r => r.CreatedAt).IsRequired();
    }
}

internal class PrincipalAttributeConfiguration : IEntityTypeConfiguration<PrincipalAttribute>
{
    public void Configure(EntityTypeBuilder<PrincipalAttribute> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Domain)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Key)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Value)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasIndex(a => new { a.PrincipalId, a.Domain, a.Key })
            .IsUnique();

        builder.HasIndex(a => new { a.PrincipalId, a.Domain });

        builder.Property(a => a.CreatedAt).IsRequired();
    }
}