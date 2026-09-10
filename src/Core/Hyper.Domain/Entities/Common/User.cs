namespace Hyper.Domain.Entities.Common;

[DisplayName("کاربر")]
[SBVR(SBVRModality.Obligatory, "مدیریت کاربران", "هر کاربر باید برای دسترسی به سیستم، مدیریت مجوزها و ردیابی فعالیت‌ها قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت کاربران", "کاربران باید برای امنیت سیستم و مدیریت دسترسی‌ها طراحی شوند")]
public partial class User : BaseUser
{
    /// <summary>
    /// نام
    /// </summary>
    [DisplayName("نام")]
    [InDisplayString]
    [MaxLength(61)]
    [SBVR(SBVRModality.Recommended, "شناسایی کاربر", "نام برای شخصی‌سازی رابط کاربری و مدیریت کاربران ضروری است")]
    public string? FirstName { get; set; }

    /// <summary>
    /// نام خانوادگی
    /// </summary>
    [DisplayName("نام خانوادگی")]
    [InDisplayString]
    [MaxLength(61)]
    [SBVR(SBVRModality.Recommended, "شناسایی کاربر", "نام خانوادگی برای تکمیل پروفایل کاربر و مدیریت کاربران مهم است")]
    public string? LastName { get; set; }

    /// <summary>
    /// کد ملی
    /// </summary>
    [DisplayName("کد ملی")]
    [SBVR(SBVRModality.Recommended, "شناسایی یکتا", "کد ملی برای جلوگیری از ثبت تکراری و احراز هویت کاربران استفاده می‌شود")]
    public long? NationalCode { get; set; }

    /// <summary>
    /// تم رنگی
    /// </summary>
    [DisplayName("تم رنگی")]
    [SBVR(SBVRModality.Recommended, "شخصی‌سازی رابط کاربری", "تم رنگی برای بهبود تجربه کاربری و شخصی‌سازی رابط کاربری استفاده می‌شود")]
    public ThemePreference ThemePreference { get; set; } = ThemePreference.Simotek;

    public virtual ICollection<Notification> Notifications { get; set; } = [];
}