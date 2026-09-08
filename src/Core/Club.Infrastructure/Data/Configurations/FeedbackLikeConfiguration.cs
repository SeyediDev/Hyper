using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class FeedbackLikeConfiguration : IEntityTypeConfiguration<FeedbackLike>
{
    public void Configure(EntityTypeBuilder<FeedbackLike> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.LikedDate)
            .IsRequired()
            .HasColumnType("datetime");

        // Relationships
        entity.HasOne(d => d.Feedback)
            .WithMany(p => p.Likes)
            .HasForeignKey(d => d.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_FeedbackLike_CustomerFeedback");

        entity.HasOne(d => d.CustomerTenant)
            .WithMany()
            .HasForeignKey(d => d.CustomerTenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_FeedbackLike_CustomerTenant");

        // Indexes - جلوگیری از لایک مجدد
        entity.HasIndex(e => new { e.FeedbackId, e.CustomerTenantId })
            .IsUnique()
            .HasDatabaseName("IX_FeedbackLike_Feedback_CustomerTenant_Unique");
    }
}

