using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class FeedbackAttachmentConfiguration : IEntityTypeConfiguration<FeedbackAttachment>
{
    public void Configure(EntityTypeBuilder<FeedbackAttachment> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Description)
            .HasMaxLength(500);

        // Relationships
        entity.HasOne(d => d.Feedback)
            .WithMany(p => p.Attachments)
            .HasForeignKey(d => d.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_FeedbackAttachment_CustomerFeedback");

        entity.HasOne(d => d.Document)
            .WithMany()
            .HasForeignKey(d => d.DocumentId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_FeedbackAttachment_Document");

        // Indexes
        entity.HasIndex(e => e.FeedbackId)
            .HasDatabaseName("IX_FeedbackAttachment_FeedbackId");
    }
}

