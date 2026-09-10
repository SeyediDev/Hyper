using Hyper.Domain.Entities.Metrics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class CustomerProductMetricsConfiguration : IEntityTypeConfiguration<CustomerProductMetrics>
{
    public void Configure(EntityTypeBuilder<CustomerProductMetrics> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(cpm => cpm.CustomerLifetimeValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.CustomerAcquisitionCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.LtvToCacRatio)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.CustomerProfitMargin)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.ReferralValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.TotalRevenue)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.AverageOrderValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.PurchaseFrequency)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.RepeatPurchaseRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.RetentionRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.SatisfactionScore)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.EngagementScore)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.NextPurchaseProbability)
            .HasColumnType("decimal(18,2)");

        builder.Property(cpm => cpm.PredictedNextPurchaseValue)
            .HasColumnType("decimal(18,2)");
    }
}

