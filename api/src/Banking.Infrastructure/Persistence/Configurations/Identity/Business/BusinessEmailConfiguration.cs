using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessEmailConfiguration : IEntityTypeConfiguration<BusinessEmail>
{
    public void Configure(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder.HasKey(je => je.Id);

        SetAddressProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetAddressProperty(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder
            .Property(e => e.Address)
            .HasMaxLength(255)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder.HasIndex(e => e.BusinessId);
        builder.HasIndex(e => e.Address);
    }
}