using Hyper.Domain.Entities.CallCenter.Enums;

namespace Hyper.Domain.Entities.CallCenter;

/// <summary>
/// تعامل با مشتری - ثبت تمام تعاملات کال سنتر با مشتریان
/// شامل تماس‌های تلفنی، چت‌ها، ایمیل‌ها و سایر ارتباطات
/// </summary>
[DisplayName("تعامل با مشتری")]
[SBVR(SBVRModality.Obligatory, "مدیریت ارتباطات", "هر تعامل با مشتری باید برای تحلیل، پیگیری و بهبود خدمات ثبت شود")]
[SBVR(SBVRModality.Recommended, "تحلیل رفتار", "تعاملات برای تحلیل رفتار مشتری، رضایت و اثربخشی خدمات استفاده می‌شود")]
public class CustomerInteraction : HyperBaseCoreAuditableEntity<long>
{
    /// <summary>
    /// شناسه مشتری 
    /// </summary>
    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Obligatory, "ارتباط با مشتری", "هر تعامل باید به یک مشتری مشخص مرتبط باشد")]
    public int CustomerTenantId { get; set; }

    [DisplayName("مشتری")]
    [InDisplayString]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// شناسه اکوسیستم
    /// </summary>
    [DisplayName("اکوسیستم")]
    public int TenantId { get; set; }

    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه نوع تعامل
    /// </summary>
    [DisplayName("نوع تعامل")]
    [SBVR(SBVRModality.Obligatory, "دسته‌بندی تعامل", "نوع تعامل برای دسته‌بندی و تحلیل تعاملات ضروری است")]
    public int InteractionTypeId { get; set; }

    [DisplayName("نوع تعامل")]
    public InteractionType InteractionType { get; set; } = null!;

    /// <summary>
    /// شناسه کاربر کال سنتر (پشتیبان)
    /// </summary>
    [DisplayName("پشتیبان")]
    [SBVR(SBVRModality.Obligatory, "مسئولیت", "هر تعامل باید به یک پشتیبان مشخص اختصاص داده شود")]
    public UserId? AgentUserId { get; set; }

    [DisplayName("پشتیبان")]
    public User? AgentUser { get; set; }

    /// <summary>
    /// عنوان تعامل
    /// </summary>
    [DisplayName("عنوان")]
    [MaxLength(200)]
    [SBVR(SBVRModality.Recommended, "شناسایی سریع", "عنوان برای شناسایی سریع تعامل در لیست‌ها استفاده می‌شود")]
    public string? Title { get; set; }

    /// <summary>
    /// توضیحات تعامل
    /// </summary>
    [DisplayName("توضیحات")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Obligatory, "مستندسازی", "توضیحات برای ثبت جزئیات تعامل و مراجعه بعدی ضروری است")]
    public string? Description { get; set; }

    /// <summary>
    /// تاریخ و زمان شروع تعامل
    /// </summary>
    [DisplayName("زمان شروع")]
    [SBVR(SBVRModality.Obligatory, "زمان‌بندی", "زمان شروع برای محاسبه مدت زمان تعامل و تحلیل استفاده می‌شود")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// تاریخ و زمان پایان تعامل
    /// </summary>
    [DisplayName("زمان پایان")]
    [SBVR(SBVRModality.Recommended, "زمان‌بندی", "زمان پایان برای محاسبه مدت زمان تعامل استفاده می‌شود")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// مدت زمان تعامل (دقیقه)
    /// </summary>
    [DisplayName("مدت زمان (دقیقه)")]
    [SBVR(SBVRModality.Calculated, "تحلیل کارایی", "مدت زمان برای تحلیل کارایی پشتیبانی و برنامه‌ریزی منابع استفاده می‌شود")]
    public int? DurationMinutes { get; set; }

    /// <summary>
    /// شناسه نتیجه تعامل
    /// </summary>
    [DisplayName("نتیجه تعامل")]
    [SBVR(SBVRModality.Recommended, "پیگیری", "نتیجه برای تحلیل اثربخشی و نیاز به پیگیری استفاده می‌شود")]
    public int? InteractionOutcomeId { get; set; }

    [DisplayName("نتیجه تعامل")]
    public InteractionOutcome? InteractionOutcome { get; set; }

    /// <summary>
    /// توضیحات نتیجه
    /// </summary>
    [DisplayName("توضیحات نتیجه")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "مستندسازی", "توضیحات نتیجه برای ثبت جزئیات بیشتر استفاده می‌شود")]
    public string? OutcomeNotes { get; set; }

    /// <summary>
    /// نمره رضایت مشتری (1-5)
    /// </summary>
    [DisplayName("نمره رضایت")]
    [SBVR(SBVRModality.Recommended, "تحلیل رضایت", "نمره رضایت برای سنجش کیفیت خدمات و بهبود استفاده می‌شود")]
    public int? SatisfactionScore { get; set; }

    /// <summary>
    /// نظرات مشتری
    /// </summary>
    [DisplayName("نظرات مشتری")]
    [MaxLength(2000)]
    [SBVR(SBVRModality.Recommended, "بازخورد", "نظرات مشتری برای بهبود خدمات و تحلیل استفاده می‌شود")]
    public string? CustomerFeedback { get; set; }

    /// <summary>
    /// آیا نیاز به پیگیری دارد
    /// </summary>
    [DisplayName("نیاز به پیگیری")]
    [SBVR(SBVRModality.Recommended, "مدیریت پیگیری", "پیگیری برای تعاملاتی که نیاز به اقدام بعدی دارند استفاده می‌شود")]
    public bool RequiresFollowUp { get; set; }

    /// <summary>
    /// تاریخ پیگیری بعدی
    /// </summary>
    [DisplayName("تاریخ پیگیری بعدی")]
    [SBVR(SBVRModality.Recommended, "زمان‌بندی پیگیری", "تاریخ پیگیری برای برنامه‌ریزی اقدامات بعدی استفاده می‌شود")]
    public DateTime? FollowUpDate { get; set; }

    /// <summary>
    /// شناسه تعامل مرتبط (در صورت نیاز به ارتباط با تعاملات قبلی)
    /// </summary>
    [DisplayName("تعامل مرتبط")]
    [SBVR(SBVRModality.Permitted, "ارتباط تعاملات", "تعامل مرتبط برای ارتباط دادن تعاملات مرتبط استفاده می‌شود")]
    public long? RelatedInteractionId { get; set; }

    [DisplayName("تعامل مرتبط")]
    public CustomerInteraction? RelatedInteraction { get; set; }

    /// <summary>
    /// شناسه تیکت یا درخواست مرتبط
    /// </summary>
    [DisplayName("شناسه تیکت")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Permitted, "یکپارچه‌سازی", "شناسه تیکت برای ارتباط با سیستم‌های تیکتینگ استفاده می‌شود")]
    public string? TicketId { get; set; }

    /// <summary>
    /// شناسه تماس (برای سیستم‌های تلفنی)
    /// </summary>
    [DisplayName("شناسه تماس")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Permitted, "یکپارچه‌سازی", "شناسه تماس برای ارتباط با سیستم‌های تلفنی استفاده می‌شود")]
    public string? CallId { get; set; }

    /// <summary>
    /// شماره تماس مشتری
    /// </summary>
    [DisplayName("شماره تماس")]
    [MaxLength(20)]
    [SBVR(SBVRModality.Recommended, "ارتباط", "شماره تماس برای تماس مجدد و ارتباط استفاده می‌شود")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// ایمیل مشتری
    /// </summary>
    [DisplayName("ایمیل")]
    [MaxLength(200)]
    [SBVR(SBVRModality.Permitted, "ارتباط", "ایمیل برای ارتباط و ارسال اطلاعات استفاده می‌شود")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// وضعیت تعامل
    /// </summary>
    [DisplayName("وضعیت")]
    [SBVR(SBVRModality.Obligatory, "مدیریت وضعیت", "وضعیت برای مدیریت چرخه حیات تعامل ضروری است")]
    public InteractionStatus Status { get; set; } = InteractionStatus.InProgress;

    /// <summary>
    /// اولویت تعامل
    /// </summary>
    [DisplayName("اولویت")]
    [SBVR(SBVRModality.Recommended, "مدیریت اولویت", "اولویت برای تخصیص منابع و مدیریت زمان استفاده می‌شود")]
    public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

    /// <summary>
    /// برچسب‌ها (JSON)
    /// </summary>
    [DisplayName("برچسب‌ها")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "دسته‌بندی", "برچسب‌ها برای دسته‌بندی و فیلتر کردن تعاملات استفاده می‌شود")]
    public string? TagsJson { get; set; }

    /// <summary>
    /// داده‌های اضافی (JSON)
    /// </summary>
    [DisplayName("داده‌های اضافی")]
    [Column(TypeName = "nvarchar(max)")]
    [SBVR(SBVRModality.Permitted, "انعطاف‌پذیری", "داده‌های اضافی برای ذخیره اطلاعات خاص استفاده می‌شود")]
    public string? AdditionalDataJson { get; set; }

    /// <summary>
    /// پیگیری‌ها
    /// </summary>
    [DisplayName("پیگیری‌ها")]
    public ICollection<InteractionFollowUp> FollowUps { get; set; } = [];

    /// <summary>
    /// ضمیمه‌ها
    /// </summary>
    [DisplayName("ضمیمه‌ها")]
    public ICollection<InteractionAttachment> Attachments { get; set; } = [];
}