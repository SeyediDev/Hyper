namespace Hyper.Domain.Entities.CallCenter;

/// <summary>
/// ضمیمه تعامل - فایل‌های مرتبط با تعاملات (صوت، تصویر، فایل)
/// </summary>
[DisplayName("ضمیمه تعامل")]
[SBVR(SBVRModality.Permitted, "مستندسازی", "ضمیمه‌ها برای ثبت مدارک و مستندات مرتبط با تعاملات استفاده می‌شود")]
public class InteractionAttachment : HyperBaseCoreAuditableEntity<long>
{
    /// <summary>
    /// شناسه تعامل
    /// </summary>
    [DisplayName("تعامل")]
    [SBVR(SBVRModality.Obligatory, "ارتباط", "هر ضمیمه باید به یک تعامل مشخص مرتبط باشد")]
    public long CustomerInteractionId { get; set; }

    [DisplayName("تعامل")]
    public CustomerInteraction CustomerInteraction { get; set; } = null!;

    /// <summary>
    /// شناسه سند
    /// </summary>
    [DisplayName("سند")]
    [SBVR(SBVRModality.Obligatory, "ذخیره‌سازی", "هر ضمیمه باید به یک سند در سیستم اسناد مرتبط باشد")]
    public int DocumentId { get; set; }

    [DisplayName("سند")]
    public Document Document { get; set; } = null!;

    /// <summary>
    /// عنوان ضمیمه
    /// </summary>
    [DisplayName("عنوان")]
    [MaxLength(200)]
    [SBVR(SBVRModality.Recommended, "شناسایی", "عنوان برای شناسایی ضمیمه استفاده می‌شود")]
    public string? Title { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "مستندسازی", "توضیحات برای توضیح محتوای ضمیمه استفاده می‌شود")]
    public string? Description { get; set; }

    /// <summary>
    /// نوع ضمیمه (صوت، تصویر، فایل)
    /// </summary>
    [DisplayName("نوع")]
    [MaxLength(50)]
    [SBVR(SBVRModality.Recommended, "دسته‌بندی", "نوع برای دسته‌بندی ضمیمه‌ها استفاده می‌شود")]
    public string? AttachmentType { get; set; }
}