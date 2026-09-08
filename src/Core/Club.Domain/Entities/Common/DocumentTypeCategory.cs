namespace Hyper.Domain.Entities.Common;

[DisplayName("دسته بندی نوع مستند")]
public partial class DocumentTypeCategory : HyperBaseCoreConfigAuditableEntity<int>
{
    public int DocumentTypeId { get; set; }

    [DisplayName("نوع مستند")] [InDisplayString]
    public virtual DocumentType DocumentType { get; set; } = null!;
    
    [DisplayName("دسته بندی")] [InDisplayString] [MaxLength(41)]
    public string Category { get; set; } = null!;
}
