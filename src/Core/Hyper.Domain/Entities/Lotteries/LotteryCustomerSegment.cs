namespace Hyper.Domain.Entities.Lotteries;

/// <summary>
/// جامعه/بازار مشتریان قرعه‌کشی - برای پشتیبانی از چند Segment در هر قرعه‌کشی با شانس/وزن
/// </summary>
[DisplayName("جامعه/بازار مشتریان قرعه‌کشی")]
[SBVR(SBVRModality.Obligatory, "هدف‌گذاری مشتریان", "هر قرعه‌کشی باید بتواند برای یک یا چند جامعه/بازار مشتریان تعریف شود تا اثربخشی افزایش یابد")]
[SBVR(SBVRModality.Permitted, "شانس شرکت", "برای هر جامعه می‌توان شانس شرکت در قرعه‌کشی را تعیین کرد")]
public class LotteryCustomerSegment : HyperBaseCoreConfigAuditableEntity<int>
{
    public int LotteryId { get; set; }
    
    [DisplayName("قرعه‌کشی")]
    [SBVR(SBVRModality.Obligatory, "رابطه قرعه‌کشی-جامعه", "هر رابطه باید به یک قرعه‌کشی مشخص تعلق داشته باشد")]
    public Lottery Lottery { get; set; } = null!;
    
    public int CustomerSegmentId { get; set; }
    
    [DisplayName("جامعه/بازار مشتریان")]
    [SBVR(SBVRModality.Obligatory, "رابطه قرعه‌کشی-جامعه", "هر رابطه باید به یک جامعه/بازار مشتریان مشخص تعلق داشته باشد")]
    public CustomerSegment CustomerSegment { get; set; } = null!;
    
    /// <summary>
    /// شانس/وزن این جامعه در قرعه‌کشی
    /// این مقدار برای محاسبه احتمال برنده شدن استفاده می‌شود
    /// </summary>
    [DisplayName("شانس")]
    [SBVR(SBVRModality.Obligatory, "شانس شرکت", "شانس برای تعیین احتمال شرکت این جامعه در قرعه‌کشی ضروری است")]
    [Range(1, int.MaxValue, ErrorMessage = "شانس باید بیشتر از صفر باشد")]
    public int Chance { get; set; } = 1;
    
    /// <summary>
    /// ترتیب - برای اولویت‌بندی در صورت نیاز
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب جامعه", "ترتیب برای اولویت‌بندی جوامع در صورت نیاز")]
    public int Order { get; set; }
}

