using Banking.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Accounts.Persistence.Configurations;

internal class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(a => a.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code)
            )
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasMany(a => a.PersonalHolders)
            .WithOne()
            .HasForeignKey(h => h.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.BusinessHolders)
            .WithOne()
            .HasForeignKey(h => h.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.CreatedAt);
    }
}

internal class PersonalAccountHolderConfiguration : IEntityTypeConfiguration<PersonalAccountHolder>
{
    public void Configure(EntityTypeBuilder<PersonalAccountHolder> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.UserId).IsRequired();
        builder.Property(h => h.HolderType).IsRequired();
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.HasIndex(h => h.UserId);
        builder.HasIndex(h => h.CreatedAt);
    }
}

internal class BusinessAccountHolderConfiguration : IEntityTypeConfiguration<BusinessAccountHolder>
{
    public void Configure(EntityTypeBuilder<BusinessAccountHolder> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.BusinessId).IsRequired();
        builder.Property(h => h.HolderType).IsRequired();
        builder.Property(h => h.CreatedAt).IsRequired();
        builder.HasIndex(h => h.BusinessId);
        builder.HasIndex(h => h.CreatedAt);
    }
}