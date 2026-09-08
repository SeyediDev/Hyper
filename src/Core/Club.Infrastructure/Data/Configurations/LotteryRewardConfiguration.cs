using Hyper.Domain.Entities.Lotteries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class LotteryRewardConfiguration : IEntityTypeConfiguration<LotteryReward>
{
    public void Configure(EntityTypeBuilder<LotteryReward> builder)
    {
        builder.ToTable("LotteryRewards");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.SegmentLabel)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(r => r.SegmentMessage)
            .HasMaxLength(256);

        builder.Property(r => r.SegmentColor)
            .IsRequired()
            .HasMaxLength(16)
            .HasDefaultValue("#f97316");

        builder.Property(r => r.SegmentTextColor)
            .IsRequired()
            .HasMaxLength(16)
            .HasDefaultValue("#ffffff");

        builder.Property(r => r.SegmentIcon)
            .HasMaxLength(16);

        builder.Property(r => r.IsJackpot)
            .HasDefaultValue(false);

        builder.Property(r => r.Amount)
            .HasDefaultValue(1);

        builder.Property(r => r.WinRate)
            .IsRequired();

        builder.Property(r => r.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(r => r.IsActive)
            .HasDefaultValue(true);

        builder.HasOne(r => r.Reward)
            .WithMany()
            .HasForeignKey(r => r.RewardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.LotteryId, r.IsActive });
    }
}










