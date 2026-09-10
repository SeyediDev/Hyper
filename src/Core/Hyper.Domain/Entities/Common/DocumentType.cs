namespace Hyper.Domain.Entities.Common;

[DisplayName("نوع مستند")]
public partial class DocumentType : HyperBaseCoreConfigAuditableEntity<int>
{
    [DisplayName("عنوان")] [InDisplayString] [MaxLength(41)]
    public string Title { get; set; } = null!;
    public virtual ICollection<Document> Documents { get; set; } = [];
    public virtual ICollection<DocumentTypeCategory> DocumentTypeCategories { get; set; } = [];
}
