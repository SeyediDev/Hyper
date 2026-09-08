namespace Hyper.Domain.Entities.Common;

public class Help : HyperBaseCoreConfigAuditableEntity<int>
{
    [MaxLength(512)]
    public string Content { get; set; } = null!;
    public LanguageId LanguageId { get; set; }
    public Language Language { get; set; } = null!;
}
