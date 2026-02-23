using Banking.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Transactions.Persistence.Configurations;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code)
            )
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.Amount).IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasMany(t => t.JournalEntries)
            .WithOne()
            .HasForeignKey(je => je.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.ReferenceNumber).IsUnique();
    }
}

internal class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(je => je.Id);

        builder.Property(je => je.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(je => je.ParticipantId).IsRequired();
        builder.Property(je => je.CreatedAt).IsRequired();

        builder.HasIndex(je => je.ParticipantId);
        builder.HasIndex(je => je.TransactionId);
        builder.HasIndex(je => je.CreatedAt);
        builder.HasIndex(je => new { je.ParticipantId, je.CreatedAt });
    }
}