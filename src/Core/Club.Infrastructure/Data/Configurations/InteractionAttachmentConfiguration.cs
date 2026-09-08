using Hyper.Domain.Entities.CallCenter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class InteractionAttachmentConfiguration : IEntityTypeConfiguration<InteractionAttachment>
{
    public void Configure(EntityTypeBuilder<InteractionAttachment> builder)
    {
        builder.ToTable("InteractionAttachments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.AttachmentType)
            .HasMaxLength(50);

        builder.HasIndex(x => x.CustomerInteractionId);
        builder.HasIndex(x => x.DocumentId);
    }
}

