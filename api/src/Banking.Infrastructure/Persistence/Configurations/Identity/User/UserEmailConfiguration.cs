using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class UserEmailConfiguration : IEntityTypeConfiguration<UserEmail>
{
    public void Configure(EntityTypeBuilder<UserEmail> builder)
    {
        builder.HasKey(je => je.Id);

        SetAddressProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetAddressProperty(EntityTypeBuilder<UserEmail> builder)
    {
        builder
            .Property(e => e.Address)
            .HasMaxLength(255)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<UserEmail> builder)
    {
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Address }).IsUnique();
        builder.HasIndex(e => e.Address);
    }
}