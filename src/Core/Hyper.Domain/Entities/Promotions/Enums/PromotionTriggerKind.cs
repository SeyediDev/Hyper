namespace Hyper.Domain.Entities.Promotions.Enums;

public enum PromotionTriggerKind : int
{
    [Description("زمان‌بندی شده")]
    TimeBased = 1,

    /// <summary>
    /// در این نوع شرایط دریافت رویداد در سمت شرایط فراخوانی قانون امتیازدهی تنظیم می شود
    /// </summary>
    [Description("دریافت رویداد")]
    EventBased = 2
}
