namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// زیرنوع بودجه اطلاع‌رسانی - تعیین روش اطلاع‌رسانی
/// </summary>
public enum NotificationBudgetSubType
{
    [Description("همه روش‌ها")]
    All = 0,
    
    [Description("ایمیل")]
    Email = 1,
    
    [Description("پیامک")]
    Sms = 2,
    
    [Description("نوتیفیکیشن")]
    PushNotification = 3,
    
    [Description("درون‌برنامه‌ای")]
    InApp = 4,
    
    [Description("وب‌هوک")]
    Webhook = 5,
    
    [Description("داخلی")]
    Internal = 6,
    
    [Description("واتساپ")]
    WhatsApp = 7,
    
    [Description("تلگرام")]
    Telegram = 8,
}
