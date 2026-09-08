namespace Hyper.Domain.Entities.Common;

[DisplayName("گروه سوالات پر تکرار")]
public class FaqCategory : HyperBaseCoreConfigAuditableEntity<int>
{
    [DisplayName("عنوان")] [InDisplayString]
    [MaxLength(41)] public string Title { get; set; } = null!;
    public virtual ICollection<Faq> Faq { get; set; } = [];
}
