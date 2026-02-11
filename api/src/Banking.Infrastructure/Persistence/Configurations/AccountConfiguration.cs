using Banking.Domain.Entities;
using Banking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(account => account.Id);

        builder
            .Property(account => account.Currency)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code)
            );
    }
}