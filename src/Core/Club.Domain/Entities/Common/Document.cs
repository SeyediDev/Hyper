namespace Hyper.Domain.Entities.Common;

[DisplayName("سند")]
[SBVR(SBVRModality.Obligatory, "مدیریت اسناد", "هر سند باید برای ذخیره‌سازی، بازیابی و مدیریت محتوا قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت اسناد", "اسناد باید برای مستندسازی فرآیندها و حفظ تاریخچه تغییرات طراحی شوند")]
public partial class Document : HyperBaseCoreConfigAuditableEntity<int>, IDocument
{
    public int? DocumentTypeId { get; set; }

    public virtual DocumentType? DocumentType { get; set; } = null!;

    /// <summary>
    /// چکسام فایل
    /// </summary>
    [DisplayName("چک‌سام سند")]
    [MaxLength(50)]
    [SBVR(SBVRModality.Obligatory, "یکپارچگی داده‌ها", "چک‌سام برای اطمینان از یکپارچگی و صحت محتوای سند ضروری است")]
    public string? Checksum { get; set; }
    public string ObjectStorageName => $"doc-{Id}";

    /// <summary>
    /// شناسه موضوع
    /// </summary>
    [DisplayName("شناسه موضوع سند")]
    [SBVR(SBVRModality.Recommended, "دسته‌بندی اسناد", "شناسه موضوع برای گروه‌بندی و جستجوی اسناد استفاده می‌شود")]
    public long? SubjectId { get; set; }

    /// <summary>
    /// عنوان موضوع
    /// </summary>
    [DisplayName("عنوان موضوع سند")]
    [MaxLength(61)]
    [SBVR(SBVRModality.Recommended, "شناسایی موضوع", "عنوان موضوع برای نمایش و جستجوی اسناد استفاده می‌شود")]
    public string? SubjectTitle { get; set; }

    /// <summary>
    /// فیلد موضوع
    /// </summary>
    [DisplayName("فیلد موضوع سند")]
    [MaxLength(61)]
    [SBVR(SBVRModality.Permitted, "جزئیات موضوع", "فیلد موضوع برای مشخص کردن جزئیات بیشتر موضوع استفاده می‌شود")]
    public string? SubjectField { get; set; }

    /// <summary>
    /// محتوای فایل
    /// </summary>
    [DisplayName("محتوای سند")]
    [MaxLength(4000)]
    [SBVR(SBVRModality.Obligatory, "ذخیره محتوا", "محتوای سند باید برای حفظ و بازیابی اطلاعات کامل باشد")]
    public byte[]? Content { get; set; }
}