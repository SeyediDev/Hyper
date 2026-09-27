namespace Hyper.Domain.Entities.Common;

[DisplayName("سوال پرتکرار")]
public class Faq : HyperBaseCoreConfigAuditableEntity<int>
{
    [DisplayName("سوال")]
    public string Question { get; set; } = null!;
    [DisplayName("جواب")]
    public string Answer { get; set; } = null!;
    [DisplayName("ترتیب")]
    public int SortIndex { get; set; }

    public int FaqCategoryId { get; set; }
    [AssociationMap(nameof(FaqCategoryId), nameof(FaqCategory.Id))]
    [DisplayName("طبقه بندی")]
    public FaqCategory FaqCategory { get; set; } = null!;

    public LanguageId LanguageId { get; set; }
    [AssociationMap(nameof(LanguageId), nameof(Language.Id))]
    [DisplayName("زبان")]
    public Language Language { get; set; } = null!;
}
