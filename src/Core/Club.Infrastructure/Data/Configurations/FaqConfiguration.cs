using Hyper.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;
internal class FaqConfiguration : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.CreateDate).HasColumnType("datetime");
        entity.Property(e => e.ExpireDate).HasColumnType("datetime");
        entity.Property(e => e.LastModified).HasColumnType("datetime");

        entity.HasOne(d => d.Language).WithMany()
            .HasForeignKey(d => d.LanguageId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
