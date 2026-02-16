using Banking.Domain.Transactions;
using Banking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(a => a.Id);

        SetCurrencyProperty(builder);
        SetTypeProperty(builder);
        SetStatuProperty(builder);
        SetAmountProperty(builder);
        SetDescriptionProperty(builder);
        SetReferenceNumberProperty(builder);
        SetCreatedAtProperty(builder);

        SetJournalEntriesRelations(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetCurrencyProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code)
            )
            .HasMaxLength(3)
            .IsRequired();
    }

    private void SetTypeProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.Type)
            .HasConversion<string>()
            .IsRequired();
    }

    private void SetStatuProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();
    }

    private void SetAmountProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.Amount)
            .IsRequired();
    }

    private void SetDescriptionProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired();
    }

    private void SetReferenceNumberProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.ReferenceNumber)
            .HasMaxLength(100);
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(t => t.CreatedAt)
            .IsRequired();
    }

    // ### Relations

    private void SetJournalEntriesRelations(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .HasMany(t => t.JournalEntries)
            .WithOne(je => je.Transaction)
            .HasForeignKey(je => je.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.ReferenceNumber);
    }
}