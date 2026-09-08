namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع عملیات پویش - تعیین می‌کند که عملیات چه زمانی اجرا شود
/// </summary>
public enum PromotionActionRunTimeType
{
    [Description("تکمیل کل")]
    [SBVR(SBVRModality.Permitted, "اجرای روی تکمیل", "وقتی همه محرک‌ها برقرار شدند - PromotionTriggerId = null")]
    OnCompletion = 1,
    [Description("دریافت یک رویداد")]
    [SBVR(SBVRModality.Permitted, "اجرای روی محرک", "با این گزینه یک محرک معرفی می شود که در ازای تکمیل دریافت آن رویداد این عملیات انجام شود")]
    OnOneEventCompletion = 2,
}