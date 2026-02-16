using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
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

    private void SetStreetProperty(EntityTypeBuilder<UserAddress> builder)
    {
        builder
            .Property(a => a.Street)
            .HasMaxLength(200)
            .IsRequired();
    }

    private void SetCityProperty(EntityTypeBuilder<UserAddress> builder)
    {
        builder
            .Property(a => a.City)
            .HasMaxLength(100)
            .IsRequired();
    }

    private void SetPostalCodeProperty(EntityTypeBuilder<UserAddress> builder)
    {
        builder
            .Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();
    }

    private void SetCountryProperty(EntityTypeBuilder<UserAddress> builder)
    {
        builder
            .Property(a => a.Country)
            .HasMaxLength(100)
            .IsRequired();
    }

    private void SetRegionProperty(EntityTypeBuilder<UserAddress> builder)
    {
        builder
            .Property(a => a.Region)
            .HasMaxLength(100);
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<UserAddress> builder)
    {
        builder
            .Property(a => a.CreatedAt)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<UserAddress> builder)
    {
        builder.HasIndex(a => a.UserId);
    }
}