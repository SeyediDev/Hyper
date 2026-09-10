using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

internal class ForumPostLikeConfiguration : IEntityTypeConfiguration<ForumPostLike>
{
    public void Configure(EntityTypeBuilder<ForumPostLike> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.LikedDate)
            .IsRequired()
            .HasColumnType("datetime");

        // Relationships
        entity.HasOne(d => d.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(d => d.PostId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_ForumPostLike_ForumPost");

        entity.HasOne(d => d.CustomerTenant)
            .WithMany()
            .HasForeignKey(d => d.CustomerTenantId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_ForumPostLike_CustomerTenant");

        // Indexes - جلوگیری از لایک مجدد
        entity.HasIndex(e => new { e.PostId, e.CustomerTenantId })
            .IsUnique()
            .HasDatabaseName("IX_ForumPostLike_Post_CustomerTenant_Unique");
    }
}

