namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// جامعه/بازار مشتریان پویش - برای پشتیبانی از چند Segment در هر پویش
/// اگر لیست خالی باشد، پویش برای همه مشتریان قابل دسترسی است
/// </summary>
[DisplayName("جامعه/بازار مشتریان پویش")]
[SBVR(SBVRModality.Obligatory, "هدف‌گذاری مشتریان", "هر پویش باید بتواند برای یک یا چند جامعه/بازار مشتریان تعریف شود تا اثربخشی افزایش یابد")]
[SBVR(SBVRModality.Permitted, "هدف‌گذاری مشتریان", "اگر لیست خالی باشد، پویش برای همه مشتریان قابل دسترسی است")]
public class PromotionCustomerSegment : HyperBaseCoreConfigAuditableEntity<int>, ISubOfPromotion
{
    public int PromotionId { get; set; }
    
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه پویش-جامعه", "هر رابطه باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;
    
    public int CustomerSegmentId { get; set; }
    
    [DisplayName("جامعه/بازار مشتریان")]
    [SBVR(SBVRModality.Obligatory, "رابطه پویش-جامعه", "هر رابطه باید به یک جامعه/بازار مشتریان مشخص تعلق داشته باشد")]
    public CustomerSegment CustomerSegment { get; set; } = null!;
    
    /// <summary>
    /// ترتیب - برای اولویت‌بندی در صورت نیاز
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب جامعه", "ترتیب برای اولویت‌بندی جوامع در صورت نیاز")]
    public int Order { get; set; }

    [DisplayName("شانس")]
    [SBVR(SBVRModality.Permitted, "شانس شرکت", "شانس برای تعیین احتمال برنده شدن این شرکت‌کننده استفاده می‌شود")]
    [Range(1, int.MaxValue, ErrorMessage = "شانس باید بیشتر از صفر باشد")]
    public int Chance { get; set; } = 1;
}