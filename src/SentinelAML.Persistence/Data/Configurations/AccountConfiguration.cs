using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAML.Domain.Entities;

namespace SentinelAML.Persistence.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");

        builder.ConfigureBaseEntity();

        builder.Property(a => a.CustomerId)
            .IsRequired();

        builder.Property(a => a.AccountNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Balance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(a => a.Customer)
            .WithMany(c => c.Accounts)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.OutgoingTransactions)
            .WithOne(t => t.FromAccount)
            .HasForeignKey(t => t.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.IncomingTransactions)
            .WithOne(t => t.ToAccount)
            .HasForeignKey(t => t.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(a => a.OutgoingTransactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(a => a.IncomingTransactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(a => a.AccountNumber)
            .IsUnique();
    }
}
