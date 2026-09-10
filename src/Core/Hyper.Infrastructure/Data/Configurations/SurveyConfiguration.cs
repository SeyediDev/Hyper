using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;

public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
{
    public void Configure(EntityTypeBuilder<Survey> entity)
    {
        entity.ToTable("Surveys");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(e => e.Description)
            .HasMaxLength(1000);

        entity.Property(e => e.SurveyType)
            .IsRequired()
            .HasConversion<int>();

        entity.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(e => e.AllowMultipleSelection)
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(e => e.ShowResults)
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(e => e.TotalParticipants)
            .IsRequired()
            .HasDefaultValue(0);

        entity.Property(e => e.StartDate)
            .HasColumnType("datetime");

        entity.Property(e => e.EndDate)
            .HasColumnType("datetime");

        // Relationships
        entity.HasOne(d => d.Promotion)
            .WithMany()
            .HasForeignKey(d => d.PromotionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        entity.HasOne(d => d.Product)
            .WithMany()
            .HasForeignKey(d => d.ProductId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        entity.HasMany(d => d.Items)
            .WithOne(i => i.Survey)
            .HasForeignKey(i => i.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(d => d.Participations)
            .WithOne(p => p.Survey)
            .HasForeignKey(p => p.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        entity.HasIndex(e => e.PromotionId);
        entity.HasIndex(e => e.IsActive);
        entity.HasIndex(e => e.SurveyType);
        entity.HasIndex(e => new { e.StartDate, e.EndDate });
    }
}



