using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class BusinessEmailConfiguration : IEntityTypeConfiguration<BusinessEmail>
{
    public void Configure(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder.HasKey(je => je.Id);

        SetEmailProperty(builder);
        SetCreatedAtProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetEmailProperty(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder.OwnsOne(e => e.Email, email =>
        {
            email.Property(e => e.Address).HasMaxLength(254).IsRequired();
            email.Property(e => e.Type).IsRequired();
            email.HasIndex(e => e.Address);
        });
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder.Property(e => e.CreatedAt).IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<BusinessEmail> builder)
    {
        builder.HasIndex(e => e.BusinessId);
    }
}