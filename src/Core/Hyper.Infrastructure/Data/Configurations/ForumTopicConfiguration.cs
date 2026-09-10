using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class ForumTopicConfiguration : IEntityTypeConfiguration<ForumTopic>
{
    public void Configure(EntityTypeBuilder<ForumTopic> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(10000);

        entity.Property(e => e.Category)
            .HasMaxLength(100);

        entity.Property(e => e.Tags)
            .HasMaxLength(500);

        entity.Property(e => e.IsClosed)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.IsPinned)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.IsLocked)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.ViewsCount)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.PostsCount)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.LikesCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        entity.HasOne(d => d.Tenant)
            .WithMany()
            .HasForeignKey(d => d.TenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumTopic_Tenant");

        entity.HasOne(d => d.CreatorCustomerTenant)
            .WithMany()
            .HasForeignKey(d => d.CreatorCustomerTenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumTopic_CustomerTenant");

        entity.HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumTopic_Product");

        entity.HasMany(d => d.Posts)
            .WithOne(p => p.Topic)
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(d => d.Likes)
            .WithOne(p => p.Topic)
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(e => e.Category)
            .HasDatabaseName("IX_ForumTopic_Category");

        entity.HasIndex(e => e.IsPinned)
            .HasDatabaseName("IX_ForumTopic_IsPinned");

        entity.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_ForumTopic_TenantId");

        entity.HasIndex(e => e.ViewsCount)
            .HasDatabaseName("IX_ForumTopic_ViewsCount");
    }
}

