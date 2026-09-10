namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع عملیات پویش - تعیین می‌کند که عملیات چه زمانی اجرا شود
/// </summary>
public enum PromotionActionType
{
    [Description("فوری")]
    [SBVR(SBVRModality.Permitted, "اجرای فوری", "برای Scoring Rules (Category = PointPoint) - فوری اجرا می‌شود")]
    Immediate = 1,
    
    [Description("روی شرط خاص")]
    [SBVR(SBVRModality.Permitted, "اجرای روی شرط", "وقتی یک شرط خاص پاس شد - PromotionConditionId باید مشخص باشد")]
    OnCondition = 2,
    
    [Description("روی تکمیل")]
    [SBVR(SBVRModality.Permitted, "اجرای روی تکمیل", "وقتی همه شرط‌ها برقرار شدند - PromotionConditionId = null")]
    OnCompletion = 3
}

