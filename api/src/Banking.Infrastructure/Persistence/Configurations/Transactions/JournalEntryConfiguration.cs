using Banking.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasKey(je => je.Id);

        SetTypeProperty(builder);
        SetCreatedAtProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetTypeProperty(EntityTypeBuilder<JournalEntry> builder)
    {
        builder
            .Property(je => je.Type)
            .HasConversion<string>()
            .IsRequired();
    }

    private void SetCreatedAtProperty(EntityTypeBuilder<JournalEntry> builder)
    {
        builder
            .Property(je => je.CreatedAt)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.HasIndex(je => je.AccountId);
        builder.HasIndex(je => je.TransactionId);
        builder.HasIndex(je => je.CreatedAt);

        // Composite index for balance calculations
        builder.HasIndex(je => new { je.AccountId, je.CreatedAt });
    }
}