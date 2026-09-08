using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class ForumTopicLikeConfiguration : IEntityTypeConfiguration<ForumTopicLike>
{
    public void Configure(EntityTypeBuilder<ForumTopicLike> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.LikedDate)
            .IsRequired()
            .HasColumnType("datetime");

        // Relationships
        entity.HasOne(d => d.Topic)
            .WithMany(p => p.Likes)
            .HasForeignKey(d => d.TopicId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ForumTopicLike_ForumTopic");

        entity.HasOne(d => d.CustomerTenant)
            .WithMany()
            .HasForeignKey(d => d.CustomerTenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumTopicLike_CustomerTenant");

        // Indexes - جلوگیری از لایک مجدد
        entity.HasIndex(e => new { e.TopicId, e.CustomerTenantId })
            .IsUnique()
            .HasDatabaseName("IX_ForumTopicLike_Topic_CustomerTenant_Unique");
    }
}

