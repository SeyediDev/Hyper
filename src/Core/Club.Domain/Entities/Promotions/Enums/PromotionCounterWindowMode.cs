namespace Hyper.Domain.Entities.Promotions.Enums;

public enum PromotionCounterWindowMode
{
    /// <summary>
    /// بدون پنجره تعدادی. هر رویداد بلافاصله فراخوانی می‌شود.
    /// </summary>
    [Description("بدون محدودیت")]
    None = 0,

    /// <summary>
    /// یک بار وقتی تعداد رویدادها به آستانه رسید.
    /// </summary>
    [Description("یکبار در آستانه")]
    OnceInThreshold = 1,

    /// <summary>
    /// بعد از رسیدن به آستانه، هر رویداد جدید هم باعث فراخوانی می‌شود.
    /// </summary>
    [Description("بعد از آستانه")]
    AfterThreshold = 2,

    /// <summary>
    /// وقتی تعداد رویدادها به مضارب آستانه رسید (مثلا 5، 10، 15).
    /// </summary>
    [Description("مضارب آستانه")]
    MultiplesOfThreshold = 3
}
