using Hyper.Domain.Entities.CallCenter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class InteractionTypeConfiguration : IEntityTypeConfiguration<InteractionType>
{
    public void Configure(EntityTypeBuilder<InteractionType> builder)
    {
        builder.ToTable("InteractionTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Icon)
            .HasMaxLength(100);

        builder.Property(x => x.Color)
            .HasMaxLength(20);

        builder.HasIndex(x => new { x.TenantId, x.Key })
            .IsUnique();

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.IsActive);
    }
}

