using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hyper.Infrastructure.Data.Configurations;
internal class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> entity)
    {
        entity.Property(e => e.CreateDate).HasColumnType("datetime");
        entity.Property(e => e.ExpireDate).HasColumnType("datetime");

        entity.HasOne(d => d.DocumentType).WithMany(p => p.Documents)
            .HasForeignKey(d => d.DocumentTypeId)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_Document_DocumentType");
    }
}
