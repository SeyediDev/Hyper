using Hyper.Domain.Entities.Rewards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class RewardAssetConfiguration : IEntityTypeConfiguration<RewardAsset>
{
    public void Configure(EntityTypeBuilder<RewardAsset> builder)
    {
        builder.HasOne(r => r.EventLog)
            .WithMany()
            .HasForeignKey(r => r.EventLogId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

