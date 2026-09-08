using Hyper.Domain.Entities.Lotteries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class LotteryConfiguration : IEntityTypeConfiguration<Lottery>
{
    public void Configure(EntityTypeBuilder<Lottery> builder)
    {
        builder.ToTable("Lotteries");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(l => l.Description)
            .HasMaxLength(512);

        builder.Property(l => l.WheelTheme)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue("sunset");

        builder.Property(l => l.WheelSubtitle)
            .HasMaxLength(256);

        builder.Property(l => l.WheelButtonLabel)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue("شروع چرخش");

        builder.Property(l => l.WheelCallToAction)
            .HasMaxLength(256);

        builder.Property(l => l.WheelCelebrationMessage)
            .HasMaxLength(512);

        builder.Property(l => l.WheelBackgroundColor)
            .IsRequired()
            .HasMaxLength(16)
            .HasDefaultValue("#FDF2F8");

        builder.Property(l => l.WheelCenterIcon)
            .HasMaxLength(16)
            .HasDefaultValue("🎉");

        builder.Property(l => l.SpinDurationSeconds)
            .HasDefaultValue(6);

        builder.Property(l => l.MaxDailySpins)
            .HasDefaultValue(3);

        builder.Property(l => l.MaxTotalSpins)
            .IsRequired(false);

        builder.Property(l => l.LotteryType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(l => l.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(l => l.Promotion)
            .WithMany()
            .HasForeignKey(l => l.PromotionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.LotteryRewards)
            .WithOne(r => r.Lottery)
            .HasForeignKey(r => r.LotteryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.Participants)
            .WithOne(p => p.Lottery)
            .HasForeignKey(p => p.LotteryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => new { l.PromotionId, l.LotteryType });
        builder.HasIndex(l => l.IsActive);
    }
}










