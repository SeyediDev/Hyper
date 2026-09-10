using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class SurveyItemConfiguration : IEntityTypeConfiguration<SurveyItem>
{
    public void Configure(EntityTypeBuilder<SurveyItem> entity)
    {
        entity.ToTable("SurveyItems");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.OptionText)
            .IsRequired()
            .HasMaxLength(500);

        entity.Property(e => e.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.IsCorrectAnswer)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.VoteCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Relationships
        entity.HasOne(d => d.Survey)
            .WithMany(p => p.Items)
            .HasForeignKey(d => d.SurveyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        entity.HasOne(d => d.Picture)
            .WithMany()
            .HasForeignKey(d => d.PictureId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        entity.HasMany(d => d.Participations)
            .WithOne(p => p.SelectedItem)
            .HasForeignKey(p => p.SelectedItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        entity.HasIndex(e => e.SurveyId);
        entity.HasIndex(e => e.DisplayOrder);
    }
}



