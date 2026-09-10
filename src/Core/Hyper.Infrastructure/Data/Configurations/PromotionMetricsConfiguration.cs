using Hyper.Domain.Entities.Promotions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class PromotionMetricsConfiguration : IEntityTypeConfiguration<PromotionMetrics>
{
    public void Configure(EntityTypeBuilder<PromotionMetrics> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(pm => pm.DeliveryRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.OpenRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.ClickThroughRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.ConversionRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.CampaignCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.CampaignRevenue)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.ReturnOnInvestment)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.CustomerAcquisitionCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.CostPerConversion)
            .HasColumnType("decimal(18,2)");

        builder.Property(pm => pm.EffectivenessScore)
            .HasColumnType("decimal(18,2)");
    }
}

