namespace Hyper.Domain.Entities.Forum;

/// <summary>
/// پست انجمن - پاسخ‌ها و نظرات در موضوعات انجمن
/// </summary>
[DisplayName("پست انجمن")]
[SBVR(SBVRModality.Obligatory, "تعامل", "پست‌ها برای گفتگو و تعامل در موضوعات ضروری هستند")]
public class ForumPost : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه موضوع
    /// </summary>
    [DisplayName("شناسه موضوع")]
    [SBVR(SBVRModality.Obligatory, "ارتباط", "هر پست باید به یک موضوع تعلق داشته باشد")]
    public int TopicId { get; set; }

    /// <summary>
    /// موضوع
    /// </summary>
    [DisplayName("موضوع")]
    public ForumTopic Topic { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری
    /// </summary>
    [DisplayName("شناسه مشتری")]
    [SBVR(SBVRModality.Obligatory, "شناسایی", "هر پست باید توسط یک مشتری ایجاد شود")]
    public int CustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// محتوای پست
    /// </summary>
    [DisplayName("محتوا")]
    [MaxLength(4000)]
    [SBVR(SBVRModality.Obligatory, "محتوا", "محتوای پست الزامی است")]
    public string Content { get; set; } = null!;

    /// <summary>
    /// شناسه پست والد (برای پاسخ به پست)
    /// </summary>
    [DisplayName("شناسه پست والد")]
    public int? ParentPostId { get; set; }

    /// <summary>
    /// پست والد
    /// </summary>
    [DisplayName("پست والد")]
    public ForumPost? ParentPost { get; set; }

    /// <summary>
    /// پاسخ‌ها
    /// </summary>
    [DisplayName("پاسخ‌ها")]
    public ICollection<ForumPost> Replies { get; set; } = [];

    /// <summary>
    /// آیا بهترین پاسخ است
    /// </summary>
    [DisplayName("بهترین پاسخ")]
    [SBVR(SBVRModality.Permitted, "کیفیت", "صاحب موضوع می‌تواند یکی از پاسخ‌ها را به عنوان بهترین پاسخ انتخاب کند")]
    public bool IsBestAnswer { get; set; } = false;

    /// <summary>
    /// آیا توسط مدیر تایید شده است
    /// </summary>
    [DisplayName("تایید شده")]
    public bool IsApproved { get; set; } = true;

    /// <summary>
    /// تعداد لایک‌ها
    /// </summary>
    [DisplayName("تعداد لایک")]
    public int LikesCount { get; set; } = 0;

    /// <summary>
    /// امتیاز کسب شده از پست (برای نویسنده)
    /// </summary>
    [DisplayName("امتیاز کسب شده")]
    public long? PointsEarned { get; set; }

    /// <summary>
    /// لایک‌ها
    /// </summary>
    [DisplayName("لایک‌ها")]
    public ICollection<ForumPostLike> Likes { get; set; } = [];
}



