namespace Hyper.Domain.Entities.Feedback;

/// <summary>
/// بازخورد مشتری - موجودیت برای ثبت انتقادات، پیشنهادات و شکایات مشتریان
/// </summary>
[DisplayName("بازخورد مشتری")]
[SBVR(SBVRModality.Obligatory, "مدیریت بازخورد", "هر بازخورد مشتری باید برای بهبود خدمات و محصولات ثبت و پیگیری شود")]
[SBVR(SBVRModality.Recommended, "مدیریت بازخورد", "بازخوردها باید برای افزایش رضایت مشتری و بهبود مستمر استفاده شوند")]
public class CustomerFeedback : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("شناسه اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر بازخورد باید به یک اکوسیستم مشخص تعلق داشته باشد")]
    public int TenantId { get; set; }

    /// <summary>
    /// اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری
    /// </summary>
    [DisplayName("شناسه مشتری")]
    [SBVR(SBVRModality.Obligatory, "شناسایی مشتری", "هر بازخورد باید به یک مشتری مشخص نسبت داده شود")]
    public int CustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// نوع بازخورد
    /// </summary>
    [DisplayName("نوع بازخورد")]
    [SBVR(SBVRModality.Obligatory, "طبقه‌بندی", "نوع بازخورد برای دسته‌بندی و اولویت‌بندی ضروری است")]
    public FeedbackType FeedbackType { get; set; }

    /// <summary>
    /// عنوان بازخورد
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(200)]
    [SBVR(SBVRModality.Obligatory, "شناسایی", "عنوان برای شناسایی سریع بازخورد ضروری است")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// متن کامل بازخورد
    /// </summary>
    [DisplayName("متن بازخورد")]
    [MaxLength(4000)]
    [SBVR(SBVRModality.Obligatory, "محتوا", "متن کامل بازخورد برای درک موضوع ضروری است")]
    public string Content { get; set; } = null!;

    /// <summary>
    /// شناسه محصول مرتبط (اختیاری)
    /// </summary>
    [DisplayName("شناسه محصول")]
    [SBVR(SBVRModality.Permitted, "ارتباط", "بازخورد می‌تواند به یک محصول خاص مرتبط باشد")]
    public int? ProductId { get; set; }

    /// <summary>
    /// محصول مرتبط
    /// </summary>
    [DisplayName("محصول")]
    public Product? Product { get; set; }

    /// <summary>
    /// دسته‌بندی بازخورد
    /// </summary>
    [DisplayName("دسته‌بندی")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Recommended, "سازماندهی", "دسته‌بندی برای مدیریت بهتر بازخوردها استفاده می‌شود")]
    public string? Category { get; set; }

    /// <summary>
    /// وضعیت بازخورد
    /// </summary>
    [DisplayName("وضعیت")]
    [SBVR(SBVRModality.Obligatory, "پیگیری", "وضعیت برای پیگیری و مدیریت بازخورد ضروری است")]
    public FeedbackStatus Status { get; set; } = FeedbackStatus.New;

    /// <summary>
    /// اولویت
    /// </summary>
    [DisplayName("اولویت")]
    [SBVR(SBVRModality.Recommended, "اولویت‌بندی", "اولویت برای مدیریت بهتر زمان و منابع استفاده می‌شود")]
    public FeedbackPriority Priority { get; set; } = FeedbackPriority.Medium;

    /// <summary>
    /// امتیاز رضایت (1-5)
    /// </summary>
    [DisplayName("امتیاز رضایت")]
    [SBVR(SBVRModality.Recommended, "سنجش رضایت", "امتیاز برای سنجش میزان رضایت مشتری استفاده می‌شود")]
    public int? SatisfactionScore { get; set; }

    /// <summary>
    /// آیا عمومی است (نمایش در انجمن)
    /// </summary>
    [DisplayName("عمومی")]
    [SBVR(SBVRModality.Permitted, "انتشار", "بازخورد می‌تواند عمومی باشد و در انجمن نمایش داده شود")]
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// شناسه کاربر پاسخ‌دهنده
    /// </summary>
    [DisplayName("شناسه پاسخ‌دهنده")]
    public UserId? AssignedToUserId { get; set; }

    /// <summary>
    /// کاربر پاسخ‌دهنده
    /// </summary>
    [DisplayName("پاسخ‌دهنده")]
    public User? AssignedToUser { get; set; }

    /// <summary>
    /// تاریخ پاسخ
    /// </summary>
    [DisplayName("تاریخ پاسخ")]
    [SBVR(SBVRModality.Calculated, "زمان‌بندی", "تاریخ پاسخ برای سنجش کیفیت خدمات استفاده می‌شود")]
    public DateTime? ResponseDate { get; set; }

    /// <summary>
    /// متن پاسخ
    /// </summary>
    [DisplayName("پاسخ")]
    [MaxLength(4000)]
    public string? Response { get; set; }

    /// <summary>
    /// تاریخ حل شده
    /// </summary>
    [DisplayName("تاریخ حل")]
    public DateTime? ResolvedDate { get; set; }

    /// <summary>
    /// یادداشت‌های داخلی (فقط برای کارکنان)
    /// </summary>
    [DisplayName("یادداشت داخلی")]
    [MaxLength(2000)]
    public string? InternalNotes { get; set; }

    /// <summary>
    /// تعداد لایک‌ها (برای بازخوردهای عمومی)
    /// </summary>
    [DisplayName("تعداد لایک")]
    [SBVR(SBVRModality.Calculated, "محبوبیت", "تعداد لایک‌ها برای شناسایی بازخوردهای مهم استفاده می‌شود")]
    public int LikesCount { get; set; } = 0;

    /// <summary>
    /// تعداد نظرات
    /// </summary>
    [DisplayName("تعداد نظرات")]
    [SBVR(SBVRModality.Calculated, "تعامل", "تعداد نظرات برای سنجش میزان تعامل استفاده می‌شود")]
    public int CommentsCount { get; set; } = 0;

    /// <summary>
    /// امتیاز قابل کسب از ثبت بازخورد
    /// </summary>
    [DisplayName("امتیاز کسب شده")]
    [SBVR(SBVRModality.Recommended, "تشویق", "امتیاز برای تشویق مشتریان به ارائه بازخورد استفاده می‌شود")]
    public long? PointsEarned { get; set; }

    /// <summary>
    /// تگ‌ها (برای جستجو و دسته‌بندی)
    /// </summary>
    [DisplayName("تگ‌ها")]
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// نظرات بازخورد
    /// </summary>
    [DisplayName("نظرات")]
    public ICollection<FeedbackComment> Comments { get; set; } = [];

    /// <summary>
    /// مستندات پیوست
    /// </summary>
    [DisplayName("پیوست‌ها")]
    public ICollection<FeedbackAttachment> Attachments { get; set; } = [];

    /// <summary>
    /// لایک‌ها
    /// </summary>
    [DisplayName("لایک‌ها")]
    public ICollection<FeedbackLike> Likes { get; set; } = [];
}



