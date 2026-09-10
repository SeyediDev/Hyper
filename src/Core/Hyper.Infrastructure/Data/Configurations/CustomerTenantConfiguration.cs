using Hyper.Domain.Entities.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class CustomerTenantConfiguration : IEntityTypeConfiguration<CustomerTenant>
{
    public void Configure(EntityTypeBuilder<CustomerTenant> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(ct => ct.TotalTransactionValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.CustomerLifetimeValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.AverageOrderValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.PurchaseFrequency)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.CustomerAcquisitionCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.LtvToCacRatio)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.CustomerProfitMargin)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.ReferralValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.RetentionRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.RepeatPurchaseRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.EngagementScore)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.LoyaltyScore)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.ChurnRiskScore)
            .HasColumnType("decimal(18,2)");

        builder.Property(ct => ct.SatisfactionScore)
            .HasColumnType("decimal(18,2)");
    }
}

