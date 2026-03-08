using Banking.Atomic.Repositories.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Atomic.Persistence;

internal class AtomicRecordConfiguration : IEntityTypeConfiguration<AtomicRecord>
{
    public void Configure(EntityTypeBuilder<AtomicRecord> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.FlowId).IsRequired();
        builder.Property(r => r.TaskName).IsRequired();
        builder.Property(r => r.RollbackJson).IsRequired();
        builder.Property(r => r.RollbackType).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasIndex(r => r.FlowId);
    }
}
