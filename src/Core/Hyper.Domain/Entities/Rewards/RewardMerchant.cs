namespace Hyper.Domain.Entities.Rewards;

/// <summary>
///  ارائه دهنده گان محصول را اینجا لیست می کنیم شناسه یک را برای سیموتک در نظر می گیریم
/// </summary>
[DisplayName("ارائه دهنده پاداش")]
[OldDbMap("ProductMerchants")]
public class RewardMerchant : HyperBaseCoreAuditableEntity<int>
{
    [DisplayName("عنوان")] [InDisplayString] [MaxLength(41)] 
    public string Title { get; set; } = null!;
}
