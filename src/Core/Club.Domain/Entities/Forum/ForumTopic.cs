namespace Hyper.Domain.Entities.Forum;

/// <summary>
/// موضوع انجمن - موضوعات بحث در انجمن مشتریان
/// </summary>
[DisplayName("موضوع انجمن")]
[SBVR(SBVRModality.Obligatory, "مدیریت انجمن", "هر موضوع بحث باید برای تعامل مشتریان قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت انجمن", "موضوعات باید برای افزایش تعامل و ایجاد جامعه مشتریان استفاده شوند")]
public class ForumTopic : HyperBaseCoreAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری ایجادکننده
    /// </summary>
    [DisplayName("شناسه مشتری")]
    [SBVR(SBVRModality.Obligatory, "شناسایی", "هر موضوع باید توسط یک مشتری ایجاد شود")]
    [OldDbMap("CustomerTenantId")]
    public int CreatorCustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public CustomerTenant CreatorCustomerTenant { get; set; } = null!;

    /// <summary>
    /// عنوان موضوع
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(200)]
    [SBVR(SBVRModality.Obligatory, "شناسایی", "عنوان برای شناسایی سریع موضوع ضروری است")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// محتوای موضوع
    /// </summary>
    [DisplayName("محتوا")]
    [MaxLength(4000)]
    [SBVR(SBVRModality.Obligatory, "محتوا", "محتوای موضوع الزامی است")]
    public string Content { get; set; } = null!;

    /// <summary>
    /// دسته‌بندی
    /// </summary>
    [DisplayName("دسته‌بندی")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Recommended, "سازماندهی", "دسته‌بندی برای مدیریت بهتر موضوعات استفاده می‌شود")]
    public string? Category { get; set; }

    /// <summary>
    /// تگ‌ها
    /// </summary>
    [DisplayName("تگ‌ها")]
    [MaxLength(500)]
    public string? Tags { get; set; }

    /// <summary>
    /// آیا بسته شده است
    /// </summary>
    [DisplayName("بسته شده")]
    public bool IsClosed { get; set; } = false;

    /// <summary>
    /// آیا پین شده است (در بالا نمایش داده می‌شود)
    /// </summary>
    [DisplayName("پین شده")]
    public bool IsPinned { get; set; } = false;

    /// <summary>
    /// آیا قفل شده است (امکان پاسخ ندارد)
    /// </summary>
    [DisplayName("قفل شده")]
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// تعداد بازدید
    /// </summary>
    [DisplayName("تعداد بازدید")]
    [SBVR(SBVRModality.Calculated, "محبوبیت", "تعداد بازدید برای سنجش محبوبیت موضوع استفاده می‌شود")]
    public int ViewsCount { get; set; } = 0;

    /// <summary>
    /// تعداد پست‌ها
    /// </summary>
    [DisplayName("تعداد پست")]
    [SBVR(SBVRModality.Calculated, "تعامل", "تعداد پست‌ها برای سنجش میزان تعامل استفاده می‌شود")]
    public int PostsCount { get; set; } = 0;

    /// <summary>
    /// تعداد لایک‌ها
    /// </summary>
    [DisplayName("تعداد لایک")]
    public int LikesCount { get; set; } = 0;

    /// <summary>
    /// شناسه محصول مرتبط (اختیاری)
    /// </summary>
    [DisplayName("شناسه محصول")]
    public int? ProductId { get; set; }

    /// <summary>
    /// محصول مرتبط
    /// </summary>
    [DisplayName("محصول")]
    public Product? Product { get; set; }

    /// <summary>
    /// پست‌های موضوع
    /// </summary>
    [DisplayName("پست‌ها")]
    public ICollection<ForumPost> Posts { get; set; } = [];

    /// <summary>
    /// لایک‌ها
    /// </summary>
    [DisplayName("لایک‌ها")]
    public ICollection<ForumTopicLike> Likes { get; set; } = [];
}