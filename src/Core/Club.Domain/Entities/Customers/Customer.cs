namespace Hyper.Domain.Entities.Customers;

[DisplayName("مشتری")]
[SBVR(SBVRModality.Obligatory, "مدیریت مشتریان", "هر مشتری باید برای امتیازدهی، تراکنش‌ها، تحلیل RFM و کمپین‌های بازاریابی قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت مشتریان", "اطلاعات مشتری باید برای محاسبه Customer Lifetime Value، تحلیل رفتار و شخصی‌سازی خدمات کامل باشد")]
public partial class Customer : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// نام
    /// </summary>
    [DisplayName("نام")]
    [InDisplayString] 
    [MaxLength(61)]
    [SBVR(SBVRModality.Recommended, "شناسایی مشتری", "نام برای شخصی‌سازی پیام‌ها و تحلیل جمعیت‌شناختی ضروری است")]
    public string? FirstName { get; set; }

    /// <summary>
    /// نام خانوادگی
    /// </summary>
    [DisplayName("نام خانوادگی")]
    [InDisplayString] 
    [MaxLength(61)]
    [SBVR(SBVRModality.Recommended, "شناسایی مشتری", "نام خانوادگی برای تکمیل پروفایل و تحلیل خانواده‌ها مهم است")]
    public string? LastName { get; set; }

    /// <summary>
    /// کد ملی
    /// </summary>
    [DisplayName("کد ملی")]
    [SBVR(SBVRModality.Recommended, "شناسایی یکتا", "کد ملی برای جلوگیری از ثبت تکراری و تحلیل جمعیت‌شناختی استفاده می‌شود")]
    public long? NationalCode { get; set; }

    /// <summary>
    /// شماره موبایل
    /// </summary>
    [DisplayName("شماره موبایل")]
    [MaxLength(14)]
    [SBVR(SBVRModality.Obligatory, "ارتباط با مشتری", "شماره موبایل برای ارسال پیامک‌های کمپین، اطلاع‌رسانی و احراز هویت ضروری است")]
    public string? MobileNo { get; set; }

    /// <summary>
    /// تاریخ تولد
    /// </summary>
    [DisplayName("تاریخ تولد")]
    [SBVR(SBVRModality.Recommended, "تحلیل جمعیت‌شناختی", "تاریخ تولد برای کمپین‌های سنی، تحلیل رفتار و شخصی‌سازی خدمات استفاده می‌شود")]
    public DateTime? BirthDate { get; set; }
}