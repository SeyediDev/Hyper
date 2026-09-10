using Hyper.Domain.Entities.CallCenter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class InteractionFollowUpConfiguration : IEntityTypeConfiguration<InteractionFollowUp>
{
    public void Configure(EntityTypeBuilder<InteractionFollowUp> builder)
    {
        builder.ToTable("InteractionFollowUps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Notes)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Result)
            .HasMaxLength(500);

        builder.HasIndex(x => x.CustomerInteractionId);
        builder.HasIndex(x => x.AgentUserId);
        builder.HasIndex(x => x.FollowUpDate);
        builder.HasIndex(x => x.IsCompleted);
    }
}

