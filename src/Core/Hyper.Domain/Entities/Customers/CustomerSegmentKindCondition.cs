namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// شرط نوع جامعه/بازار مشتریان - تعریف شرط‌های خاص برای انواع مختلف جامعه‌ها/بازارهای مشتریان
/// این موجودیت امکان تعریف شرط‌های دقیق بر اساس پارامترهای مشتری را فراهم می‌کند
/// </summary>
[DisplayName("شرط‌های جامعه‌سازی/بازار")]
public class CustomerSegmentKindCondition : HyperBaseCoreAuditableEntity<int>
{
    public int CustomerSegmentId { get; set; }

    [DisplayName("جامعه/بازار مشتریان")]
    [SBVR(SBVRModality.Obligatory, "جامعه/بازار مشتریان", "هر شرط باید به یک جامعه/بازار مشخص تعلق داشته باشد")]
    public CustomerSegment CustomerSegment { get; set; } = null!;

    [DisplayName("نوع جامعه/بازار")]
    [SBVR(SBVRModality.Obligatory, "نوع جامعه/بازار", "نوع جامعه/بازار تعیین می‌کند که جامعه/بازار چگونه تعریف شده است")]
    [SBVR(SBVRModality.Recommended, "نوع جامعه/بازار", "انتخاب نوع صحیح باعث مدیریت بهتر جامعه/بازار می‌شود")]
    public CustomerSegmentKind Kind { get; set; }

    [DisplayName("پارامتر مشتری هدف")]
    [SBVR(SBVRModality.Permitted, "شرط پارامتری", "در صورت بررسی یک پارامتر مشخص، شناسه آن باید ذخیره شود")]
    public int? CustomerParameterId { get; set; }

    [DisplayName("پارامتر مشتری")]
    public CustomerParameter? CustomerParameter { get; set; }

    [DisplayName("کلید پارامتر مشتری")]
    [MaxLength(40)]
    [SBVR(SBVRModality.Permitted, "شرط پارامتری", "برای پارامترهای رزرو شده یا محاسبه‌شده می‌توان از کلید استفاده کرد")]
    public string? CustomerParameterKey { get; set; }

    [DisplayName("عنوان شرط")]
    [MaxLength(100)]
    [SBVR(SBVRModality.Obligatory, "عنوان شرط", "عنوان شرط باید واضح و توصیفی باشد")]
    [SBVR(SBVRModality.Recommended, "عنوان شرط", "عنوان باید شامل نوع شرط و معیارهای کلیدی باشد")]
    public string Title { get; set; } = null!;


    [DisplayName("توضیحات شرط")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "توضیحات شرط", "توضیحات باید شامل منطق و نحوه ارزیابی شرط باشد")]
    public string? Description { get; set; }

    [DisplayName("اولویت")]
    [SBVR(SBVRModality.Recommended, "اولویت شرط", "اولویت تعیین می‌کند که کدام شرط زودتر بررسی شود")]
    [SBVR(SBVRModality.Permitted, "اولویت شرط", "اولویت 1 بالاترین و اولویت 10 پایین‌ترین است")]
    public int Priority { get; set; } = 1;

    [DisplayName("گروه شرط")]
    [SBVR(SBVRModality.Permitted, "گروه شرط", "گروه شرط برای دسته‌بندی شرط‌های مرتبط استفاده می‌شود. شرط‌های یک گروه با هم And می شوند. و شرط‌های گروه های مختلف با هم Or می شوند ")]
    public ConditionGroup? ConditionGroup { get; set; }

    [DisplayName("نوع شرط")]
    [SBVR(SBVRModality.Recommended, "نوع شرط", "نوع شرط تعیین می‌کند که چگونه شرط ارزیابی شود")]
    public CustomerSegmentConditionKind ConditionKind { get; set; }

    [DisplayName("فرمول شرط")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "فرمول شرط", "فرمول شرط برای شرط‌های پیچیده استفاده می‌شود")]
    public string? Constraint { get; set; }

    [DisplayName("مقایسه شود با")]
    [SBVR(SBVRModality.Permitted, "مقایسه شود با", "تعیین می‌کند که شرط با چه چیزی مقایسه شود")]
    public CustomerSegmentCompareWith? CompareWith { get; set; }

    [DisplayName("مقدار شرط")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "مقدار شرط", "مقدار مورد نظر برای مقایسه")]
    public string? Value { get; set; }

    [DisplayName("حداقل امتیاز")]
    [SBVR(SBVRModality.Permitted, "حداقل امتیاز", "حداقل امتیاز مورد نیاز برای برقراری شرط")]
    public decimal? MinScore { get; set; }

    [DisplayName("حداکثر امتیاز")]
    [SBVR(SBVRModality.Permitted, "حداکثر امتیاز", "حداکثر امتیاز مجاز برای برقراری شرط")]
    public decimal? MaxScore { get; set; }

    [DisplayName("پیام خطا")]
    [MaxLength(200)]
    [SBVR(SBVRModality.Permitted, "پیام خطا", "پیام خطا در صورت عدم برقراری شرط نمایش داده می‌شود")]
    public string? ErrorMessage { get; set; }
}
