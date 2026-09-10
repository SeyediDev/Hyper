namespace Hyper.Domain.Entities.Promotions.Enums;

public enum PromotionTriggerCheckTimeKind : int
{
    [Description("به‌محض دریافت رویداد")]
    OnRecieveEvent = 1,
    [Description("به‌محض دریافت رویداد به شرط کنترل مهلت مدت زمان خاص از رویداد قبلی")]
    OnReceivedEventControlDurationFromThePreviousEvent = 2,
    [Description("به‌محض دریافت رویداد به شرط کنترل دریافت در یک بازه زمانی در همان روز رویداد قبلی")]
    OnReceivedEventControlInATimeFrameOnTheSameDayAsThePreviousEvent = 3,

    [Description("زمان‌بندی شده در انتهای بازه زمانی")]
    ScheduledInEndOfATimeFrame = 10,
    [Description("زمان‌بندی شده در انتهای هر روز")]
    ScheduledInEndOfEachDay = 11,
    [Description("زمان‌بندی شده در انتهای هر هفته")]
    ScheduledInEndOfEachWeek = 12,
    [Description("زمان‌بندی شده در انتهای هر ماه")]
    ScheduledInEndOfEachMonth = 13,
    [Description("زمان‌بندی شده در انتهای هر سال")]
    ScheduledInEndOfEachYear = 14,
    [Description("زمان‌بندی شده در انتهای دوره پویش")]
    ScheduledInEndOfPeriodOfPromotion = 15,
    [Description("زمان‌بندی شده در بعد از سپری شدن یک زمان خاص از رویداد قبلی")]
    ScheduledInAfterASpecificTimeHasPassedSinceThePreviousEvent = 16,
    [Description("زمان‌بندی شده در یک بازه زمانی در همان روز از رویداد قبلی")]
    ScheduledInInATimeFrameOnTheSameDayAsThePreviousEvent = 17,
}