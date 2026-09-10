namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع شرط پویش
/// </summary>
public enum PromotionConditionType
{
    [Description("رویداد")]
    Event = 1,
    
    [Description("ارتقاء سطح امتیاز")]
    UpgradePointLevel = 2,
    
    [Description("خرید پاداش")]
    PurchaseAward = 3,
    
    [Description("مصرف پاداش")]
    ConsumeAward = 4,
    
    [Description("خرید محصول")]
    PurchaseProduct = 5
}

