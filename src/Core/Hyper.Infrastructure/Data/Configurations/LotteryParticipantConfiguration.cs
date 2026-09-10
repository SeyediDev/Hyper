using Hyper.Domain.Entities.Lotteries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class LotteryParticipantConfiguration : IEntityTypeConfiguration<LotteryParticipant>
{
    public void Configure(EntityTypeBuilder<LotteryParticipant> builder)
    {
        builder.ToTable("LotteryParticipants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ParticipatedAt)
            .IsRequired();

        builder.Property(p => p.IsWinner)
            .HasDefaultValue(false);

        builder.Property(p => p.IsRewardDistributed)
            .HasDefaultValue(false);

        builder.HasOne(p => p.Lottery)
            .WithMany(l => l.Participants)
            .HasForeignKey(p => p.LotteryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.CustomerTenant)
            .WithMany()
            .HasForeignKey(p => p.CustomerTenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.LotteryReward)
            .WithMany()
            .HasForeignKey(p => p.LotteryRewardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Reward)
            .WithMany()
            .HasForeignKey(p => p.RewardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.LotteryId, p.ParticipatedAt });
        builder.HasIndex(p => new { p.CustomerTenantId, p.ParticipatedAt });
    }
}










