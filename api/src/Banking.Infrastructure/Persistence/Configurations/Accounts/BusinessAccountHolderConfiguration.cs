using Banking.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessAccountHolderConfiguration : IEntityTypeConfiguration<BusinessAccountHolder>
{
    public void Configure(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder.HasKey(ah => ah.Id);

        SetHolderType(builder);
        SetCreatedAt(builder);

        SetBusinessRelation(builder);
        SetAccountRelation(builder);

        SetIndexes(builder);
    }

    // ### Properties

    public void SetHolderType(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder
            .Property(ah => ah.HolderType)
            .IsRequired();
    }

    public void SetCreatedAt(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder
            .Property(ah => ah.CreatedAt)
            .IsRequired();
    }

    // ### Relations

    public void SetBusinessRelation(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder
            .HasOne(ah => ah.Business)
            .WithMany(u => u.AccountHoldings)
            .HasForeignKey(ah => ah.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetAccountRelation(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder
            .HasOne(ah => ah.Account)
            .WithMany(u => u.BusinessHolders)
            .HasForeignKey(ah => ah.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ### Indexes

    public void SetIndexes(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder.HasIndex(ah => ah.CreatedAt);
    }
}