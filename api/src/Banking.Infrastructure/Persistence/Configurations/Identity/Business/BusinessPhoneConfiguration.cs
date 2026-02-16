using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessPhoneConfiguration : IEntityTypeConfiguration<BusinessPhone>
{
    public void Configure(EntityTypeBuilder<BusinessPhone> builder)
    {
        builder.HasKey(je => je.Id);

        SetCountryCodeProperty(builder);
        SetNumberProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetCountryCodeProperty(EntityTypeBuilder<BusinessPhone> builder)
    {
        builder
            .Property(p => p.CountryCode)
            .HasMaxLength(5)
            .IsRequired();
    }

    private void SetNumberProperty(EntityTypeBuilder<BusinessPhone> builder)
    {
        builder
            .Property(p => p.Number)
            .HasMaxLength(20)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<BusinessPhone> builder)
    {
        builder.HasIndex(p => p.BusinessId);
    }
}