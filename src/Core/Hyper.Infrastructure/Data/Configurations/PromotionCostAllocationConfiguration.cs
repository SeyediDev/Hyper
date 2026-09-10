using Hyper.Domain.Entities.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class PromotionCostAllocationConfiguration : IEntityTypeConfiguration<PromotionCostAllocation>
{
    public void Configure(EntityTypeBuilder<PromotionCostAllocation> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(pca => pca.CostPercentage)
            .HasColumnType("decimal(18,2)");

        builder.Property(pca => pca.AllocatedCost)
            .HasColumnType("decimal(18,2)");
    }
}

