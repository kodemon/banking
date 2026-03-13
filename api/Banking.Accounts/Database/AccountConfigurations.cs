using Banking.Accounts.Repositories.Resources;
using Banking.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Accounts.Database;

internal class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder
            .Property(a => a.Number)
            .HasConversion(an => an.Value, value => new AccountNumber(value))
            .HasMaxLength(11)
            .IsRequired();

        builder.Property(a => a.Name).IsRequired();

        builder.Property(a => a.Type).IsRequired();

        builder.Property(a => a.Status).IsRequired();

        builder
            .Property(a => a.Currency)
            .HasConversion(currency => currency.Code, code => Currency.FromCode(code))
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(a => a.CreatedAt).IsRequired();

        builder
            .HasMany(a => a.AccountHolders)
            .WithOne()
            .HasForeignKey(h => h.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.Number).IsUnique();
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.CreatedAt);
    }
}

internal class PersonalAccountHolderConfiguration : IEntityTypeConfiguration<AccountHolder>
{
    public void Configure(EntityTypeBuilder<AccountHolder> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).ValueGeneratedNever();

        builder.Property(h => h.HolderId).IsRequired();

        builder.Property(h => h.HolderType).IsRequired();

        builder.Property(h => h.CreatedAt).IsRequired();

        builder.HasIndex(h => h.HolderId);
        builder.HasIndex(h => h.CreatedAt);
    }
}
