using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Persistence.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.ConfigureBaseEntity();

        builder.Property(t => t.FromAccountId)
            .IsRequired();

        builder.Property(t => t.ToAccountId)
            .IsRequired();

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.RiskScore)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.TransactionDate)
            .IsRequired();
    }
}
