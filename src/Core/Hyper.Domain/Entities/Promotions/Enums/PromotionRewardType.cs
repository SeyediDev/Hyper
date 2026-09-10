namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع جوایز/مزایای کمپین - برای اطلاع‌رسانی و دسته‌بندی کمپین‌ها
/// </summary>
[EnumDescription("نوع جایزه", "PromotionRewardType")]
public enum PromotionRewardType
{
    /// <summary>
    /// امتیاز یا پوئن
    /// </summary>
    [Description("امتیاز/پوئن")]
    Points = 1,

    /// <summary>
    /// تخفیف یا کوپن
    /// </summary>
    [Description("تخفیف/کوپن")]
    Discount = 2,

    /// <summary>
    /// جایزه فیزیکی
    /// </summary>
    [Description("جایزه فیزیکی")]
    GiftItem = 3,

    /// <summary>
    /// دسترسی یا membership
    /// </summary>
    [Description("دسترسی و عضویت")]
    Access = 4,

    /// <summary>
    /// تجربه (رویداد، جلسه آموزشی و...)
    /// </summary>
    [Description("تجربه و فعالیت")]
    Experience = 5,

    /// <summary>
    /// سایر (نمایش در متن مزایا)
    /// </summary>
    [Description("سایر")]
    Other = 6
}
