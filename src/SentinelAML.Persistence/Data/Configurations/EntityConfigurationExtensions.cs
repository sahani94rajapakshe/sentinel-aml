using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SentinelAML.Domain.Common;

namespace SentinelAML.Persistence.Data.Configurations;

internal static class EntityConfigurationExtensions
{
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(e => e.CreatedAtUtc)
            .IsRequired()
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamp with time zone");
    }
}
