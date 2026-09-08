using Hyper.Domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class RewardConfiguration : IEntityTypeConfiguration<Reward>
{
    public void Configure(EntityTypeBuilder<Reward> builder)
    {
        builder.HasOne(r => r.Picture)
            .WithMany()
            .HasForeignKey(r => r.PictureId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Configure decimal properties with explicit precision and scale
        // to avoid EF Core warnings about silent truncation
        builder.Property(r => r.ConversionRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.AverageRating)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.PopularityScore)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.ActualCost)
            .HasColumnType("decimal(18,2)");

        builder.Property(r => r.ProfitMargin)
            .HasColumnType("decimal(18,2)");
    }
}

