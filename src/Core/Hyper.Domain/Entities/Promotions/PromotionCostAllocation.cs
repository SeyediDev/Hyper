namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// تسهیم هزینه پویش - توزیع هزینه پویش در بازه‌های زمانی مختلف
/// برای محاسبه دقیق‌تر CAC بر اساس بازه زمانی جذب مشتری
/// </summary>
[DisplayName("تسهیم زمانی هزینه پویش")]
[SBVR(SBVRModality.Obligatory, "تسهیم زمانی هزینه", "هر تسهیم هزینه باید برای توزیع هزینه پویش در بازه‌های زمانی مختلف قابل شناسایی باشد")]
public class PromotionCostAllocation : HyperBaseCoreConfigAuditableEntity<int>, ISubOfPromotion
{
    public int PromotionId { get; set; }
    
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه تسهیم-پویش", "هر تسهیم هزینه باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;
    
    /// <summary>
    /// عنوان بازه زمانی
    /// </summary>
    [DisplayName("عنوان بازه")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی بازه", "عنوان بازه باید برای مدیریت و ردیابی بازه‌ها واضح و قابل فهم باشد")]
    [SBVR(SBVRModality.Permitted, "مثال‌های عنوان بازه", "مثال‌های عنوان بازه: 'دوره تبلیغ'، '6 ماه بعد'، 'دوره اول'، 'دوره دوم'")]
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// تاریخ شروع بازه زمانی
    /// </summary>
    [DisplayName("تاریخ شروع")]
    [SBVR(SBVRModality.Obligatory, "بازه زمانی", "تاریخ شروع بازه باید مشخص باشد")]
    public DateTime FromDate { get; set; }
    
    /// <summary>
    /// تاریخ پایان بازه زمانی
    /// </summary>
    [DisplayName("تاریخ پایان")]
    [SBVR(SBVRModality.Obligatory, "بازه زمانی", "تاریخ پایان بازه باید مشخص باشد")]
    public DateTime ToDate { get; set; }
    
    /// <summary>
    /// درصد هزینه این بازه (0-100)
    /// مجموع درصدهای همه بازه‌ها باید 100 باشد
    /// </summary>
    [DisplayName("درصد هزینه")]
    [SBVR(SBVRModality.Obligatory, "تسهیم هزینه", "درصد هزینه باید برای توزیع هزینه در بازه‌های زمانی مشخص باشد")]
    public decimal CostPercentage { get; set; }
    
    /// <summary>
    /// هزینه این بازه (محاسبه شده از هزینه کل پویش × CostPercentage)
    /// </summary>
    [DisplayName("هزینه بازه")]
    [SBVR(SBVRModality.Calculated, "هزینه بازه", "هزینه بازه = هزینه کل پویش × درصد هزینه")]
    public decimal? AllocatedCost { get; set; }
    
    /// <summary>
    /// ترتیب بازه (برای نمایش و محاسبه)
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب بازه", "ترتیب برای اولویت‌بندی بازه‌ها")]
    public int Order { get; set; }
    
    /// <summary>
    /// توضیحات بازه
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(512)]
    [SBVR(SBVRModality.Permitted, "توضیحات بازه", "توضیحات برای مستندسازی بازه")]
    public string? Description { get; set; }
}

