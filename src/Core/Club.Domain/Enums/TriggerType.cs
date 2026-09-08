namespace Hyper.Domain.Enums;

public enum TriggerType
{
    [Description("رویداد")]
    Event = 1,
    [Description("پویش/کمپین")]
    Promotion = 2,
    [Description("ارتقاء سطح‌امتیاز")]
    UpgradePointLevel = 3,
    [Description("خرید پاداش")]
    PurchaseAward = 4,
    [Description("مصرف پاداش")]
    ConsumeAward = 5,
    [Description("خرید محصول")]
    PurchaseProductOrService = 6,
    [Description("انتقال امتیاز")]
    PointTransfer = 7,
}
