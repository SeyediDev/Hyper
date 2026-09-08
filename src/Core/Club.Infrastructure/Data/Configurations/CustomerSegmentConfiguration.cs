using Hyper.Domain.Entities.CustomerSegments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class CustomerSegmentConfiguration : IEntityTypeConfiguration<CustomerSegment>
{
    public void Configure(EntityTypeBuilder<CustomerSegment> builder)
    {
        builder.Property(cs => cs.GrowthRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(cs => cs.RetentionRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(cs => cs.EngagementRate)
            .HasColumnType("decimal(18,2)");
    }
}