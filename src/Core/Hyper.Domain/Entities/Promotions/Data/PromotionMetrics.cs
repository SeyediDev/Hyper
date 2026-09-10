namespace Hyper.Domain.Entities.Promotions.Data;

/// <summary>
/// معیارهای عملکرد پویش - فیلدهای محاسبه شده (Calculated) که نتیجه عملیات و داده‌های سامانه هستند
/// این موجودیت در اسکیمای جداگانه (مثلا Analytics) نگهداری می‌شود
/// رابطه یک به یک با Promotion دارد
/// </summary>
[DisplayName("معیارهای عملکرد پویش")]
[SBVR(SBVRModality.Calculated, "معیارهای عملکرد", "معیارهای عملکرد برای تحلیل اثربخشی و بهینه‌سازی پویش‌ها محاسبه می‌شوند")]
public class PromotionMetrics : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// شناسه پویش (کلید یکتا برای رابطه یک به یک)
    /// </summary>
    public int PromotionId { get; set; }
    
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه معیار-پویش", "هر معیار باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;
    
    // =====================================================
    // Campaign Performance Metrics
    // =====================================================
    
    /// <summary>
    /// تعداد کل دریافت‌کنندگان هدف
    /// </summary>
    [DisplayName("تعداد هدف")]
    [SBVR(SBVRModality.Calculated, "تحلیل عملکرد", "تعداد هدف برای محاسبه نرخ دستیابی و بودجه‌بندی استفاده می‌شود")]
    public int? TargetAudienceCount { get; set; }
    
    /// <summary>
    /// تعداد پیام‌های ارسال شده
    /// </summary>
    [DisplayName("تعداد پیام‌های ارسال شده")]
    [SBVR(SBVRModality.Calculated, "تحلیل ارسال", "تعداد پیام‌های ارسال شده برای محاسبه نرخ تحویل استفاده می‌شود")]
    public int? MessagesSent { get; set; }
    
    /// <summary>
    /// تعداد پیام‌های تحویل شده
    /// </summary>
    [DisplayName("تعداد پیام‌های تحویل شده")]
    [SBVR(SBVRModality.Calculated, "تحلیل ارسال", "تعداد پیام‌های تحویل شده برای محاسبه نرخ تحویل استفاده می‌شود")]
    public int? MessagesDelivered { get; set; }
    
    /// <summary>
    /// نرخ تحویل - Delivery Rate (%)
    /// </summary>
    [DisplayName("نرخ تحویل")]
    [SBVR(SBVRModality.Calculated, "تحلیل عملکرد", "نرخ تحویل = (تحویل شده / ارسال شده) × 100")]
    public decimal? DeliveryRate { get; set; }
    
    /// <summary>
    /// تعداد باز شدن پیام - Open Count
    /// </summary>
    [DisplayName("تعداد باز شدن")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "تعداد باز شدن برای محاسبه نرخ باز شدن استفاده می‌شود")]
    public int? OpenCount { get; set; }
    
    /// <summary>
    /// نرخ باز شدن - Open Rate (%)
    /// </summary>
    [DisplayName("نرخ باز شدن")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "نرخ باز شدن = (باز شده / تحویل شده) × 100")]
    public decimal? OpenRate { get; set; }
    
    /// <summary>
    /// تعداد کلیک - Click Count
    /// </summary>
    [DisplayName("تعداد کلیک")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "تعداد کلیک برای محاسبه نرخ کلیک استفاده می‌شود")]
    public int? ClickCount { get; set; }
    
    /// <summary>
    /// نرخ کلیک - Click-Through Rate (%)
    /// </summary>
    [DisplayName("نرخ کلیک")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "نرخ کلیک = (کلیک شده / باز شده) × 100")]
    public decimal? ClickThroughRate { get; set; }
    
    /// <summary>
    /// تعداد تبدیل‌ها - Conversion Count
    /// </summary>
    [DisplayName("تعداد تبدیل")]
    [SBVR(SBVRModality.Calculated, "تحلیل نتایج", "تعداد تبدیل برای محاسبه نرخ تبدیل و ROI استفاده می‌شود")]
    public int? ConversionCount { get; set; }
    
    /// <summary>
    /// نرخ تبدیل - Conversion Rate (%)
    /// </summary>
    [DisplayName("نرخ تبدیل")]
    [SBVR(SBVRModality.Calculated, "تحلیل نتایج", "نرخ تبدیل = (تبدیل شده / هدف) × 100")]
    public decimal? ConversionRate { get; set; }
    
    // =====================================================
    // Financial Metrics
    // =====================================================
    
    /// <summary>
    /// هزینه کل پویش - Campaign Cost
    /// این فیلد از PromotionCostAllocation محاسبه می‌شود یا مستقیماً وارد می‌شود
    /// </summary>
    [DisplayName("هزینه پویش")]
    [SBVR(SBVRModality.Calculated, "تحلیل مالی", "هزینه پویش برای محاسبه ROI و CAC استفاده می‌شود")]
    public decimal? CampaignCost { get; set; }
    
    /// <summary>
    /// درآمد حاصل از پویش - Campaign Revenue
    /// </summary>
    [DisplayName("درآمد")]
    [SBVR(SBVRModality.Calculated, "تحلیل مالی", "درآمد پویش برای محاسبه ROI و سودآوری استفاده می‌شود")]
    public decimal? CampaignRevenue { get; set; }
    
    /// <summary>
    /// بازگشت سرمایه - ROI (%)
    /// </summary>
    [DisplayName("بازگشت سرمایه (ROI)")]
    [SBVR(SBVRModality.Calculated, "تحلیل سودآوری", "ROI = ((درآمد - هزینه) / هزینه) × 100")]
    public decimal? ReturnOnInvestment { get; set; }
    
    /// <summary>
    /// هزینه جذب مشتری - Customer Acquisition Cost
    /// </summary>
    [DisplayName("هزینه جذب مشتری (CAC)")]
    [SBVR(SBVRModality.Calculated, "تحلیل بهره‌وری", "CAC = هزینه پویش / تعداد مشتریان جذب شده")]
    public decimal? CustomerAcquisitionCost { get; set; }
    
    /// <summary>
    /// هزینه هر تبدیل - Cost Per Conversion
    /// </summary>
    [DisplayName("هزینه هر تبدیل")]
    [SBVR(SBVRModality.Calculated, "تحلیل بهره‌وری", "هزینه هر تبدیل = هزینه پویش / تعداد تبدیل‌ها")]
    public decimal? CostPerConversion { get; set; }
    
    // =====================================================
    // Engagement Metrics
    // =====================================================
    
    /// <summary>
    /// تعداد مشتریان جدید جذب شده
    /// </summary>
    [DisplayName("تعداد مشتریان جدید")]
    [SBVR(SBVRModality.Calculated, "تحلیل جذب", "تعداد مشتریان جدید برای محاسبه CAC و تحلیل اثربخشی استفاده می‌شود")]
    public int? NewCustomersAcquired { get; set; }
    
    /// <summary>
    /// نمره اثربخشی - Effectiveness Score (0-100)
    /// </summary>
    [DisplayName("نمره اثربخشی")]
    [SBVR(SBVRModality.Calculated, "تحلیل کلی", "نمره اثربخشی برای مقایسه پویش‌ها و اولویت‌بندی استفاده می‌شود")]
    public decimal? EffectivenessScore { get; set; }
    
    /// <summary>
    /// تاریخ آخرین محاسبه معیارها
    /// </summary>
    [DisplayName("تاریخ آخرین محاسبه")]
    [SBVR(SBVRModality.Calculated, "مدیریت داده", "تاریخ آخرین محاسبه برای اطمینان از به‌روز بودن معیارها استفاده می‌شود")]
    public DateTime? LastMetricsCalculationDate { get; set; }
}

