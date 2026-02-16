using Banking.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class PersonalAccountHolderConfiguration : IEntityTypeConfiguration<PersonalAccountHolder>
{
    public void Configure(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder.HasKey(ah => ah.Id);

        SetHolderType(builder);
        SetCreatedAt(builder);

        SetUserRelation(builder);
        SetAccountRelation(builder);

        SetIndexes(builder);
    }

    // ### Properties

    public void SetHolderType(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder
            .Property(ah => ah.HolderType)
            .IsRequired();
    }

    public void SetCreatedAt(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder
            .Property(ah => ah.CreatedAt)
            .IsRequired();
    }

    // ### Relations

    public void SetUserRelation(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder
            .HasOne(ah => ah.User)
            .WithMany(u => u.AccountHoldings)
            .HasForeignKey(ah => ah.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetAccountRelation(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder
            .HasOne(ah => ah.Account)
            .WithMany(u => u.PersonalHolders)
            .HasForeignKey(ah => ah.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ### Indexes

    public void SetIndexes(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder.HasIndex(ah => ah.CreatedAt);
    }
}