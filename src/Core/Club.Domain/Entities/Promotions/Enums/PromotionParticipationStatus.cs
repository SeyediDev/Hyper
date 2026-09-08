namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// وضعیت شرکت مشتری در پویش
/// </summary>
public enum PromotionParticipationStatus
{
    /// <summary>
    /// در حال انجام - مشتری در حال شرکت در پویش است
    /// </summary>
    [Description("در حال انجام")]
    InProgress = 1,
    
    /// <summary>
    /// تکمیل شده - همه شرط‌های پویش برقرار شده‌اند
    /// </summary>
    [Description("تکمیل شده")]
    Completed = 2,
    
    /// <summary>
    /// لغو شده - شرکت مشتری لغو شده است
    /// </summary>
    [Description("لغو شده")]
    Cancelled = 3,
    
    /// <summary>
    /// منقضی شده - بازه زمانی پویش به پایان رسیده است
    /// </summary>
    [Description("منقضی شده")]
    Expired = 4
}

