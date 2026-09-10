using Hyper.Domain.Entities.Promotions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class PromotionRecipientConfiguration : IEntityTypeConfiguration<PromotionRecipient>
{
    public void Configure(EntityTypeBuilder<PromotionRecipient> builder)
    {
        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(pr => pr.EngagementScore)
            .HasColumnType("decimal(18,2)");
    }
}

