using Neo.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;
internal class CultureTermConfiguration : IEntityTypeConfiguration<CultureTerm>
{
    public void Configure(EntityTypeBuilder<CultureTerm> entity)
    {
        entity.HasKey(e => e.Id).HasName("Id");
        entity.Property(e => e.SubjectTitle).HasColumnType("nvarchar(50)");
        entity.Property(e => e.SubjectField).HasColumnType("varchar(50)");
        entity.Property(e => e.Term).HasColumnType("nvarchar(50)");
        entity.Property(e => e.CreateDate).HasColumnType("datetime");
        entity.Property(e => e.ExpireDate).HasColumnType("datetime");

        entity.HasOne(d => d.Language).WithMany(p => p.CultureTerm)
      .HasForeignKey(d => d.LanguageId)
      .OnDelete(DeleteBehavior.NoAction);
    }
}
