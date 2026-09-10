namespace Hyper.Domain.Entities.Events;

/// <summary>
/// تنوع کانال تولید کننده رویداد را اینجا لیست می کنیم
/// 1 - باجت
/// 2 - DigiPay
/// 3 - سوییچ
/// 4 - هوش مصنوعی
/// 4 - نوا
/// 10 - همراه بانک
/// </summary>
[DisplayName("کانال دریافت رویداد اکوسیستم")]
public class EventChannel : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("شناسه اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر کانال رویداد باید به یک اکوسیستم مشخص تعلق داشته باشد تا کنترل دسترسی و گزارش‌گیری صحیح انجام شود")]
    public int TenantId { get; set; }

    /// <summary>
    /// اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر کانال رویداد باید به یک اکوسیستم مشخص تعلق داشته باشد تا از اشتراک‌گذاری ناخواسته اطلاعات جلوگیری شود")]
    public Tenant Tenant { get; set; } = null!;

    [MaxLength(40)]
    [DisplayName("کلید")]
    public string Key { get; set; } = null!;

    //[Unique]
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;
}
