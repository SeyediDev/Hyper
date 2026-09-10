namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// روش اطلاع رسانی عملیات پویش
/// </summary>
public enum PromotionNotificationSendMethod
{
    [Description("بدون اطلاع رسانی")]
    None = 0,
    
    [Description("ارسال پیامک")]
    SendSms = 1,

    [Description("ارسال ایمیل")]
    SendEmail = 2,

    [Description("ارسال نوتیفیکیشن")]
    PushNotification = 4
}

public enum PromotionMessageSendMethod
{
    [Description("ارسال پیامک")]
    SendSms = 1,

    [Description("ارسال ایمیل")]
    SendEmail = 2,

    [Description("ارسال نوتیفیکیشن")]
    PushNotification = 4
}
