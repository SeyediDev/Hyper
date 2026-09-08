namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// مشخص می‌کند که عملیات پویش بر روی چه کسی اعمال می‌شود
/// </summary>
public enum PromotionActionOnWho
{
    [Description("مشتری اصلی")]
    Customer = 1,
    
    [Description("معرف")]
    Referrer = 2,
    
    [Description("مشتری و معرف")]
    Both = 3
}

