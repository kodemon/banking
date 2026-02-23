using Banking.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(je => je.Id);

        SetNameProperty(builder);
        SetDateOfBirthProperty(builder);

        SetAddressRelations(builder);
        SetEmailRelations(builder);

        SetIndexes(builder);
    }

    // ### Properties

    public void SetNameProperty(EntityTypeBuilder<User> builder)
    {
        builder
            .OwnsOne(u => u.Name, name =>
            {
                name
                    .Property(n => n.Family)
                    .HasMaxLength(100)
                    .IsRequired();
                name
                    .Property(n => n.Given)
                    .HasMaxLength(100)
                    .IsRequired();
            });
    }

    public void SetDateOfBirthProperty(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(u => u.DateOfBirth)
            .IsRequired();
    }

    // ### Relations

    public void SetAddressRelations(EntityTypeBuilder<User> builder)
    {
        builder
            .HasMany(u => u.Addresses)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void SetEmailRelations(EntityTypeBuilder<User> builder)
    {
        builder
            .HasMany(u => u.Emails)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    // ### Indexes

    public void SetIndexes(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.CreatedAt);
    }
}