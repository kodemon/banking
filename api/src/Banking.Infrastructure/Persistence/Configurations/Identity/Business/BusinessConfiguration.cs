using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.HasKey(je => je.Id);

        SetNameProperty(builder);
        SetOrganizationNumber(builder);

        SetAddressRelations(builder);
        SetEmailRelations(builder);
        SetPhoneRelations(builder);

        SetIndexes(builder);
    }

    // ### Properties

    public void SetNameProperty(EntityTypeBuilder<Business> builder)
    {
        builder
            .Property(b => b.Name)
            .IsRequired();
    }

    public void SetOrganizationNumber(EntityTypeBuilder<Business> builder)
    {
        builder
            .Property(b => b.OrganizationNumber)
            .IsRequired();
    }

    // ### Relations

    public void SetAddressRelations(EntityTypeBuilder<Business> builder)
    {
        builder
            .HasMany(u => u.Addresses)
            .WithOne()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetEmailRelations(EntityTypeBuilder<Business> builder)
    {
        builder
            .HasMany(u => u.Emails)
            .WithOne()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetPhoneRelations(EntityTypeBuilder<Business> builder)
    {
        builder
            .HasMany(u => u.Phones)
            .WithOne()
            .HasForeignKey(a => a.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ### Indexes

    public void SetIndexes(EntityTypeBuilder<Business> builder)
    {
        builder.HasIndex(u => u.CreatedAt);
    }

}