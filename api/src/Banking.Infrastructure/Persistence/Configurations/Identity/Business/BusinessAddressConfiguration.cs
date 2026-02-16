using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessAddressConfiguration : IEntityTypeConfiguration<BusinessAddress>
{
    public void Configure(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder.HasKey(je => je.Id);

        SetStreetProperty(builder);
        SetCityProperty(builder);
        SetPostalCodeProperty(builder);
        SetCountryProperty(builder);
        SetRegionProperty(builder);
        SetCreatedAtProperty(builder);

        SetIndexes(builder);

    }

    // ### Properties

    private void SetStreetProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder
            .Property(a => a.Street)
            .HasMaxLength(200)
            .IsRequired();
    }

    private void SetCityProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder
            .Property(a => a.City)
            .HasMaxLength(100)
            .IsRequired();
    }

    private void SetPostalCodeProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder
            .Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();
    }

    private void SetCountryProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder
            .Property(a => a.Country)
            .HasMaxLength(100)
            .IsRequired();
    }

    private void SetRegionProperty(EntityTypeBuilder<BusinessAddress> builder)
    {
        builder
            .Property(a => a.Region)
            .HasMaxLength(100);
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