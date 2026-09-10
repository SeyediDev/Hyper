using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class CustomerFeedbackConfiguration : IEntityTypeConfiguration<CustomerFeedback>
{
    public void Configure(EntityTypeBuilder<CustomerFeedback> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(4000);

        entity.Property(e => e.Category)
            .HasMaxLength(100);

        entity.Property(e => e.FeedbackType)
            .IsRequired();

        entity.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(FeedbackStatus.New)
            .HasSentinel((FeedbackStatus)0); // Use 0 as sentinel since enum starts at 1

        entity.Property(e => e.Priority)
            .IsRequired()
            .HasDefaultValue(FeedbackPriority.Medium)
            .HasSentinel((FeedbackPriority)0); // Use 0 as sentinel since enum starts at 1

        entity.Property(e => e.IsPublic)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.LikesCount)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.CommentsCount)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.Response)
            .HasMaxLength(4000);

        entity.Property(e => e.InternalNotes)
            .HasMaxLength(2000);

        entity.Property(e => e.Tags)
            .HasMaxLength(500);

        entity.Property(e => e.ResponseDate)
            .HasColumnType("datetime");

        entity.Property(e => e.ResolvedDate)
            .HasColumnType("datetime");

        // Relationships
        entity.HasOne(d => d.Tenant)
            .WithMany()
            .HasForeignKey(d => d.TenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_CustomerFeedback_Tenant");

        entity.HasOne(d => d.CustomerTenant)
            .WithMany()
            .HasForeignKey(d => d.CustomerTenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_CustomerFeedback_CustomerTenant");

        entity.HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_CustomerFeedback_Product");

        entity.HasOne(d => d.AssignedToUser)
            .WithMany()
            .HasForeignKey(d => d.AssignedToUserId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_CustomerFeedback_User");

        entity.HasMany(d => d.Comments)
            .WithOne(p => p.Feedback)
            .HasForeignKey(p => p.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(d => d.Attachments)
            .WithOne(p => p.Feedback)
            .HasForeignKey(p => p.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(d => d.Likes)
            .WithOne(p => p.Feedback)
            .HasForeignKey(p => p.FeedbackId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(e => e.Status)
            .HasDatabaseName("IX_CustomerFeedback_Status");

        entity.HasIndex(e => e.FeedbackType)
            .HasDatabaseName("IX_CustomerFeedback_FeedbackType");

        entity.HasIndex(e => e.Priority)
            .HasDatabaseName("IX_CustomerFeedback_Priority");

        entity.HasIndex(e => new { e.TenantId, e.CustomerTenantId })
            .HasDatabaseName("IX_CustomerFeedback_Tenant_CustomerTenant");
    }
}

