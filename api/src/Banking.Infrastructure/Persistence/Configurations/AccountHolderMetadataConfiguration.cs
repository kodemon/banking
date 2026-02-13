using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

internal class AccountHolderIndividualIdentityConfiguration : IEntityTypeConfiguration<AccountHolderIndividualIdentity>
{
    public void Configure(EntityTypeBuilder<AccountHolderIndividualIdentity> builder)
    {
        builder.HasKey(i => i.Id);

        SetDateOfBirthProperty(builder);
        SetNameProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetDateOfBirthProperty(EntityTypeBuilder<AccountHolderIndividualIdentity> builder)
    {
        builder
            .Property(i => i.DateOfBirth)
            .IsRequired();
    }

    private void SetNameProperty(EntityTypeBuilder<AccountHolderIndividualIdentity> builder)
    {
        builder.OwnsOne(i => i.Name, name =>
        {
            name.Property(n => n.Family)
                .HasMaxLength(100)
                .IsRequired();

            name.Property(n => n.Given)
                .HasMaxLength(100)
                .IsRequired();
        });
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<AccountHolderIndividualIdentity> builder)
    {
        builder.HasIndex(i => i.AccountHolderId);
    }
}

internal class AccountHolderBusinessIdentityConfiguration : IEntityTypeConfiguration<AccountHolderBusinessIdentity>
{
    public void Configure(EntityTypeBuilder<AccountHolderBusinessIdentity> builder)
    {
        builder.HasKey(b => b.Id);

        SetNameProperty(builder);
        SetOrganizationNumberProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetNameProperty(EntityTypeBuilder<AccountHolderBusinessIdentity> builder)
    {
        builder
            .Property(b => b.Name)
            .HasMaxLength(200)
            .IsRequired();
    }

    private void SetOrganizationNumberProperty(EntityTypeBuilder<AccountHolderBusinessIdentity> builder)
    {
        builder
            .Property(b => b.OrganizationNumber)
            .HasMaxLength(50)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<AccountHolderBusinessIdentity> builder)
    {
        builder.HasIndex(b => b.AccountHolderId);
        builder.HasIndex(b => b.OrganizationNumber).IsUnique();
    }
}

internal class AccountHolderAddressConfiguration : IEntityTypeConfiguration<AccountHolderAddress>
{
    public void Configure(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder.HasKey(a => a.Id);

        SetStreetProperty(builder);
        SetCityProperty(builder);
        SetPostalCodeProperty(builder);
        SetCountryProperty(builder);
        SetRegionProperty(builder);
        SetCreatedAtProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetStreetProperty(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder
            .Property(a => a.Street)
            .HasMaxLength(200)
            .IsRequired();
    }

    private void SetCityProperty(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder
            .Property(a => a.City)
            .HasMaxLength(100)
            .IsRequired();
    }

    private void SetPostalCodeProperty(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder
            .Property(a => a.PostalCode)
            .HasMaxLength(20)
            .IsRequired();
    }

    private void SetCountryProperty(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder
            .Property(a => a.Country)
            .HasMaxLength(100)
            .IsRequired();
    }

    private void SetRegionProperty(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder
            .Property(a => a.Region)
            .HasMaxLength(100);
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder
            .Property(a => a.CreatedAt)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<AccountHolderAddress> builder)
    {
        builder.HasIndex(a => a.AccountHolderId);
    }
}

internal class AccountHolderEmailConfiguration : IEntityTypeConfiguration<AccountHolderEmail>
{
    public void Configure(EntityTypeBuilder<AccountHolderEmail> builder)
    {
        builder.HasKey(e => e.Id);

        SetAddressProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetAddressProperty(EntityTypeBuilder<AccountHolderEmail> builder)
    {
        builder
            .Property(e => e.Address)
            .HasMaxLength(255)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<AccountHolderEmail> builder)
    {
        builder.HasIndex(e => e.AccountHolderId);
        builder.HasIndex(e => e.Address);
    }
}

internal class AccountHolderPhoneConfiguration : IEntityTypeConfiguration<AccountHolderPhone>
{
    public void Configure(EntityTypeBuilder<AccountHolderPhone> builder)
    {
        builder.HasKey(p => p.Id);

        SetCountryCodeProperty(builder);
        SetNumberProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetCountryCodeProperty(EntityTypeBuilder<AccountHolderPhone> builder)
    {
        builder
            .Property(p => p.CountryCode)
            .HasMaxLength(5)
            .IsRequired();
    }

    private void SetNumberProperty(EntityTypeBuilder<AccountHolderPhone> builder)
    {
        builder
            .Property(p => p.Number)
            .HasMaxLength(20)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<AccountHolderPhone> builder)
    {
        builder.HasIndex(p => p.AccountHolderId);
    }
}