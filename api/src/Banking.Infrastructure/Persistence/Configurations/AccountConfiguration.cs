using Banking.Domain.Entities;
using Banking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        SetCurrencyProperty(builder);
        SetTypeProperty(builder);
        SetStatusProperty(builder);
        SetCreatedAtProperty(builder);

        SetAccountHolderRelationship(builder);
        SetJournalEntriesRelationship(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetCurrencyProperty(EntityTypeBuilder<Account> builder)
    {
        builder
            .Property(a => a.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code)
            )
            .HasMaxLength(3)
            .IsRequired();
    }

    private void SetTypeProperty(EntityTypeBuilder<Account> builder)
    {
        builder
            .Property(a => a.Type)
            .HasConversion<string>()
            .IsRequired();
    }

    private void SetStatusProperty(EntityTypeBuilder<Account> builder)
    {
        builder
            .Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<Account> builder)
    {
        builder
            .Property(a => a.CreatedAt)
            .IsRequired();
    }

    // ### Relations

    private void SetAccountHolderRelationship(EntityTypeBuilder<Account> builder)
    {
        builder
            .HasMany(a => a.Holders)
            .WithMany(h => h.Accounts)
            .UsingEntity(j => j.ToTable("AccountHolders_Accounts"));
    }

    private void SetJournalEntriesRelationship(EntityTypeBuilder<Account> builder)
    {
        builder
            .HasMany(a => a.JournalEntries)
            .WithOne(je => je.Account)
            .HasForeignKey(je => je.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<Account> builder)
    {
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.CreatedAt);
    }
}