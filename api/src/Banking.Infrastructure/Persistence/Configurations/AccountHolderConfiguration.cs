using Banking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class AccountHolderConfiguration : IEntityTypeConfiguration<AccountHolder>
{
    public void Configure(EntityTypeBuilder<AccountHolder> builder)
    {
        builder.HasKey(holder => holder.Id);
    }
}