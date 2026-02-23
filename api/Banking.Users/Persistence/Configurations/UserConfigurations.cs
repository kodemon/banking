using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Users.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.Family).HasMaxLength(100).IsRequired();
            name.Property(n => n.Given).HasMaxLength(100).IsRequired();
        });

        builder.Property(u => u.DateOfBirth).IsRequired();

        builder.HasMany(u => u.Addresses)
            .WithOne()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Emails)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => u.CreatedAt);
    }
}

internal class UserEmailConfiguration : IEntityTypeConfiguration<UserEmail>
{
    public void Configure(EntityTypeBuilder<UserEmail> builder)
    {
        builder.HasKey(e => e.Id);

        builder.OwnsOne(e => e.Email, email =>
        {
            email.Property(e => e.Address).HasMaxLength(254).IsRequired();
            email.Property(e => e.Type).IsRequired();
            email.HasIndex(e => e.Address);
        });

        builder.Property(e => e.CreatedAt).IsRequired();
        builder.HasIndex(e => e.UserId);
    }
}

internal class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.HasKey(a => a.Id);

        builder.OwnsOne(a => a.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200).IsRequired();
            address.Property(a => a.City).HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasMaxLength(100).IsRequired();
            address.Property(a => a.Region).HasMaxLength(100);
        });

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.HasIndex(a => a.UserId);
    }
}