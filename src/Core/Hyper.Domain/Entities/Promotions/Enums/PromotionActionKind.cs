namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع اقدام پویش - چه عملیاتی انجام شود
/// </summary>
public enum PromotionActionKind
{
    [Description("افزایش امتیاز")]
    CreditPoint = 1,
    
    [Description("کاهش امتیاز")]
    DebitPoint = 2,
    
    [Description("مقداردهی امتیاز")]
    SetPointBalance = 3,

    [Description("تنظیم سطح امتیاز")]
    SetPointLevel = 4,

    [Description("مقداردهی پارامتر مشتری")]
    SetCustomerParameterValue = 11,
    
    [Description("عضویت در جامعه مشتریان")]
    JoinInCustomerSegment = 12,
    
    [Description("ثبت معرف")]
    ReferrerRegistration = 13,

    [Description("اعطای رایگان پاداش")]
    GrantReward = 21,
    
    [Description("شرکت در قرعه‌کشی")]
    JoinLottery = 22,
    
    [Description("فراخوانی API بیرونی")]
    CallExternalApi = 31,

    [Description("فقط اطلاع رسانی")]
    None = 101,
}