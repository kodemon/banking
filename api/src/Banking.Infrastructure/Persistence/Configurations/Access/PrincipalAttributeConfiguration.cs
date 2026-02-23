using Banking.Domain.Access;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class PrincipalAttributeConfiguration : IEntityTypeConfiguration<PrincipalAttribute>
{
    public void Configure(EntityTypeBuilder<PrincipalAttribute> builder)
    {
        builder.HasKey(a => a.Id);

        SetPrincipalId(builder);
        SetKeyProperty(builder);
        SetValueProperty(builder);

        SetIndexes(builder);
    }

    // ### Properties

    private void SetPrincipalId(EntityTypeBuilder<PrincipalAttribute> builder)
    {
        builder
            .Property(a => a.PrincipalId)
            .IsRequired();
    }

    private void SetKeyProperty(EntityTypeBuilder<PrincipalAttribute> builder)
    {
        builder
            .Property(a => a.Key)
            .IsRequired();
    }

    private void SetValueProperty(EntityTypeBuilder<PrincipalAttribute> builder)
    {
        builder
            .Property(a => a.Value)
            .IsRequired();
    }

    // ### Indexes

    private void SetIndexes(EntityTypeBuilder<PrincipalAttribute> builder)
    {
        builder.HasIndex(a => a.PrincipalId);
        builder.HasIndex(a => a.CreatedAt);
    }
}