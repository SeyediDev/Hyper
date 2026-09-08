namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع بودجه پویش - تعیین می‌کند که بودجه‌بندی بر اساس چه معیاری انجام می‌شود
/// این نوع‌ها با PromotionActionKind مطابقت دارند ولی برای بودجه‌بندی استفاده می‌شوند
/// </summary>
public enum PromotionBudgetKind
{
    [Description("بودجه امتیاز")]
    Point = 1,
    
    [Description("بودجه اعطای پاداش")]
    GrantReward = 21,
    
    [Description("بودجه عضویت در جامعه مشتریان")]
    JoinInCustomerSegment = 12,
    
    [Description("بودجه شرکت در قرعه‌کشی")]
    JoinLottery = 22,
    
    [Description("بودجه فراخوانی API بیرونی")]
    CallExternalApi = 31,
    
    [Description("بودجه اطلاع‌رسانی")]
    Notification = 101,
}
