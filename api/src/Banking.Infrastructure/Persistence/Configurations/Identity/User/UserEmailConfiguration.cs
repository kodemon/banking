using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class UserEmailConfiguration : IEntityTypeConfiguration<UserEmail>
{
    public void Configure(EntityTypeBuilder<UserEmail> builder)
    {
        builder.HasKey(je => je.Id);

        SetEmailProperty(builder);
        SetCreatedAtProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetEmailProperty(EntityTypeBuilder<UserEmail> builder)
    {
        builder.OwnsOne(e => e.Email, email =>
        {
            email.Property(e => e.Address).HasMaxLength(254).IsRequired();
            email.Property(e => e.Type).IsRequired();
            email.HasIndex(e => e.Address);
        });
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<UserEmail> builder)
    {
        builder.Property(e => e.CreatedAt).IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<UserEmail> builder)
    {
        builder.HasIndex(e => e.UserId);
    }
}