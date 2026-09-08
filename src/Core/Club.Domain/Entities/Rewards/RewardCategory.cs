namespace Hyper.Domain.Entities.Rewards;

/// <summary>
/// طبقه‌بندی پاداش را اینجا لیست می کنیم
/// </summary>
[DisplayName("طبقه‌بندی پاداش")]
public class RewardCategory : HyperBaseCoreAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر طبقه‌بندی پاداش باید به یک اکوسیستم مشخص تعلق داشته باشد تا از تداخل داده‌ها جلوگیری شود")]
    public Tenant Tenant { get; set; } = null!;

    [DisplayName("عنوان")] [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

    [OldDbMap("CategoryId")]
    public int? ParentRewardCategoryId { get; set; }
    [DisplayName("طبقه‌بندی مافوق")]
    public RewardCategory? ParentRewardCategory { get; set; } = null!;
}