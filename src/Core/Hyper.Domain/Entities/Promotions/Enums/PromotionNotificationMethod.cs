namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// روش اطلاع رسانی عملیات پویش
/// </summary>
public enum PromotionNotificationMethod
{
    [Description("بدون اطلاع رسانی")]
    None = 0,
    
    [Description("ارسال پیامک")]
    SendSms = 1,
    
    [Description("ارسال نوتیفیکیشن")]
    SendNotification = 2,
    
    [Description("هر دو")]
    Both = 3,
}

