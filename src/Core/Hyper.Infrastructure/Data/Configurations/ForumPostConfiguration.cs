using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class ForumPostConfiguration : IEntityTypeConfiguration<ForumPost>
{
    public void Configure(EntityTypeBuilder<ForumPost> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(10000);

        entity.Property(e => e.IsBestAnswer)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.IsApproved)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(e => e.LikesCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        entity.HasOne(d => d.Topic)
            .WithMany(p => p.Posts)
            .HasForeignKey(d => d.TopicId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ForumPost_ForumTopic");

        entity.HasOne(d => d.CustomerTenant)
            .WithMany()
            .HasForeignKey(d => d.CustomerTenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumPost_CustomerTenant");

        entity.HasOne(d => d.ParentPost)
            .WithMany(p => p.Replies)
            .HasForeignKey(d => d.ParentPostId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumPost_ParentPost");

        entity.HasMany(d => d.Likes)
            .WithOne(p => p.Post)
            .HasForeignKey(p => p.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(e => e.TopicId)
            .HasDatabaseName("IX_ForumPost_TopicId");

        entity.HasIndex(e => e.IsBestAnswer)
            .HasDatabaseName("IX_ForumPost_IsBestAnswer");
    }
}

