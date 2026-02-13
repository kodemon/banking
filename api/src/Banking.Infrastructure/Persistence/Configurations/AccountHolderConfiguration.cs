using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class AccountHolderConfiguration : IEntityTypeConfiguration<AccountHolder>
{
    public void Configure(EntityTypeBuilder<AccountHolder> builder)
    {
        builder.HasKey(ah => ah.Id);

        SetCreatedAt(builder);

        SetIndividualIdentitiesRelations(builder);
        SetBusinessIdentitiesRelations(builder);
        SetAddressesRelations(builder);
        SetEmailsRelations(builder);
        SetPhonesRelations(builder);

        SetIndexes(builder);
    }

    // ### Properties

    public void SetCreatedAt(EntityTypeBuilder<AccountHolder> builder)
    {
        builder
            .Property(ah => ah.CreatedAt)
            .IsRequired();
    }

    // ### Relations

    public void SetIndividualIdentitiesRelations(EntityTypeBuilder<AccountHolder> builder)
    {
        builder
            .HasMany(ah => ah.IndividualIdentities)
            .WithOne()
            .HasForeignKey(i => i.AccountHolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetBusinessIdentitiesRelations(EntityTypeBuilder<AccountHolder> builder)
    {
        builder
            .HasMany(ah => ah.BusinessIdentities)
            .WithOne()
            .HasForeignKey(b => b.AccountHolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetAddressesRelations(EntityTypeBuilder<AccountHolder> builder)
    {
        builder
            .HasMany(ah => ah.Addresses)
            .WithOne()
            .HasForeignKey(a => a.AccountHolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetEmailsRelations(EntityTypeBuilder<AccountHolder> builder)
    {
        builder
            .HasMany(ah => ah.Emails)
            .WithOne()
            .HasForeignKey(e => e.AccountHolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetPhonesRelations(EntityTypeBuilder<AccountHolder> builder)
    {
        builder
            .HasMany(ah => ah.Phones)
            .WithOne()
            .HasForeignKey(p => p.AccountHolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ### Indexes

    public void SetIndexes(EntityTypeBuilder<AccountHolder> builder)
    {
        builder.HasIndex(ah => ah.CreatedAt);
    }
}