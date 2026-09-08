using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class FeedbackCommentConfiguration : IEntityTypeConfiguration<FeedbackComment>
{
    public void Configure(EntityTypeBuilder<FeedbackComment> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(2000);

        entity.Property(e => e.IsOfficial)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        entity.HasOne(d => d.Feedback)
            .WithMany(p => p.Comments)
            .HasForeignKey(d => d.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_FeedbackComment_CustomerFeedback");

        entity.HasOne(d => d.Customer)
            .WithMany()
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_FeedbackComment_Customer");

        entity.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_FeedbackComment_User");

        entity.HasOne(d => d.ParentComment)
            .WithMany(p => p.Replies)
            .HasForeignKey(d => d.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_FeedbackComment_ParentComment");

        // Indexes
        entity.HasIndex(e => e.FeedbackId)
            .HasDatabaseName("IX_FeedbackComment_FeedbackId");
    }
}

