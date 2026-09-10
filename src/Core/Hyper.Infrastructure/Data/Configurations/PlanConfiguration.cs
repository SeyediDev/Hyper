using Hyper.Domain.Entities.Promotions.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.PriceInPoints)
            .IsRequired();

        builder.Property(p => p.ValidityDays)
            .IsRequired();

        builder.Property(p => p.DiscountType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.DiscountValue)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.IsGlobalDiscount)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(p => p.Promotion)
            .WithMany()
            .HasForeignKey(p => p.PromotionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Point)
            .WithMany()
            .HasForeignKey(p => p.PointId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CustomerSegment)
            .WithMany()
            .HasForeignKey(p => p.CustomerSegmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Picture)
            .WithMany()
            .HasForeignKey(p => p.PictureId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Indexes
        builder.HasIndex(p => p.PromotionId);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => new { p.PromotionId, p.IsActive });
    }
}

