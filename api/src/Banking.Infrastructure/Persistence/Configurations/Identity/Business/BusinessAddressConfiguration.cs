using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessAddressConfiguration : IEntityTypeConfiguration<BusinessAddress>
{
    public void Configure(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder.HasKey(je => je.Id);

        SetAddressProperty(builder);
        SetCreatedAtProperty(builder);

        SetIndexes(builder);

    }

    // ### Properties

    private void SetAddressProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder.OwnsOne(a => a.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasMaxLength(100).IsRequired();
            address.Property(a => a.Region).HasMaxLength(100);
        });
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder
            .Property(a => a.CreatedAt)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder.HasIndex(a => a.BusinessId);
    }
}