using Hyper.Domain.Entities.Tenants.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class TenantAttributeMonthlyAggregationPersianConfiguration : IEntityTypeConfiguration<TenantAttributeMonthlyAggregationPersian>
{
    public void Configure(EntityTypeBuilder<TenantAttributeMonthlyAggregationPersian> builder)
    {
        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Attribute)
            .WithMany()
            .HasForeignKey(e => e.AttributeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CustomerTenant)
            .WithMany()
            .HasForeignKey(e => e.CustomerTenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Segment)
            .WithMany()
            .HasForeignKey(e => e.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ProductCategory)
            .WithMany()
            .HasForeignKey(e => e.ProductCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Product)
            .WithMany()
            .HasForeignKey(e => e.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EventType)
            .WithMany()
            .HasForeignKey(e => e.EventTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Channel)
            .WithMany()
            .HasForeignKey(e => e.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Sum)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Average)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.MinValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.MaxValue)
            .HasColumnType("decimal(18,2)");
    }
}
