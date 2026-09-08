namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// معیارهای مشتری-محصول: ذخیره تحلیل‌های مالی و رفتاری برای هر ترکیب مشتری/محصول
/// این موجودیت برای تحلیل سودآوری، رفتار خرید و پیش‌بینی رفتار مشتری در سطح محصول استفاده می‌شود
/// </summary>
[DisplayName("معیارهای مشتری-محصول")]
[SBVR(SBVRModality.Obligatory, "تحلیل سودآوری", "برای تصمیم‌گیری دقیق باید معیارهای مشتری-محصول ذخیره شود")]
[SBVR(SBVRModality.Recommended, "تحلیل رفتار مشتری", "معیارهای مشتری-محصول برای تحلیل عمیق رفتار خرید و بهینه‌سازی استراتژی‌های بازاریابی استفاده می‌شود")]
public class CustomerProductMetrics : HyperBaseCoreAuditableEntity<int>
{
    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Obligatory, "شناسایی مشتری", "هر معیار باید به یک مشتری مشخص در اکوسیستم تعلق داشته باشد")]
    public int CustomerTenantId { get; set; }
    public CustomerTenant CustomerTenant { get; set; } = null!;

    [DisplayName("محصول")]
    [SBVR(SBVRModality.Obligatory, "شناسایی محصول", "هر معیار باید به یک محصول مشخص تعلق داشته باشد")]
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // مالی
    [DisplayName("ارزش طول عمر (CLV)")]
    [SBVR(SBVRModality.Calculated, "تحلیل ارزش مشتری", "CLV برای ارزیابی سودآوری مشتری در طول عمر و تصمیم‌گیری تخصیص منابع استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل ارزش مشتری", "CLV = متوسط درآمد هر خرید × تعداد خریدهای پیش‌بینی شده × طول عمر متوسط")]
    public decimal? CustomerLifetimeValue { get; set; }

    [DisplayName("هزینه جذب (CAC)")]
    [SBVR(SBVRModality.Calculated, "تحلیل هزینه جذب", "CAC برای ارزیابی کارایی بازاریابی و محاسبه ROI استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل هزینه جذب", "CAC = هزینه‌های بازاریابی / تعداد مشتریان جدید جذب شده")]
    public decimal? CustomerAcquisitionCost { get; set; }

    [DisplayName("نسبت LTV:CAC")]
    [SBVR(SBVRModality.Calculated, "تحلیل سودآوری", "نسبت LTV:CAC برای ارزیابی بازگشت سرمایه جذب مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل سودآوری", "نسبت بالای 3:1 نشان‌دهنده سودآوری خوب است")]
    public decimal? LtvToCacRatio { get; set; }

    [DisplayName("دوره بازگشت سرمایه (روز)")]
    [SBVR(SBVRModality.Calculated, "تحلیل نقدینگی", "دوره بازگشت سرمایه برای ارزیابی سرعت بازگشت سرمایه جذب مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل نقدینگی", "دوره بازگشت = CAC / درآمد ماهانه حاصل از مشتری")]
    public int? PaybackPeriodDays { get; set; }

    [DisplayName("حاشیه سود (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل سودآوری", "حاشیه سود برای ارزیابی سودآوری هر مشتری-محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل سودآوری", "حاشیه سود = ((CLV - CAC) / CLV) × 100")]
    public decimal? CustomerProfitMargin { get; set; }

    [DisplayName("ارزش ارجاع")]
    [SBVR(SBVRModality.Calculated, "تحلیل ارجاع", "ارزش ارجاع برای ارزیابی تأثیر مشتری در جذب مشتریان جدید استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل ارجاع", "ارزش مالی مشتریان جدیدی که توسط این مشتری معرفی شده‌اند")]
    public decimal? ReferralValue { get; set; }

    // خرید
    [DisplayName("تعداد خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "تعداد خرید برای سنجش وفاداری و تکرار خرید استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل رفتار خرید", "تعداد خرید = جمع تمام تراکنش‌های خرید مشتری برای این محصول")]
    public int PurchaseCount { get; set; }

    [DisplayName("مجموع درآمد")]
    [SBVR(SBVRModality.Calculated, "تحلیل درآمد", "مجموع درآمد برای محاسبه ارزش مالی مشتری-محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل درآمد", "مجموع درآمد = مجموع مبلغ تمام خریدهای مشتری برای این محصول")]
    public decimal TotalRevenue { get; set; }

    [DisplayName("میانگین ارزش خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "میانگین ارزش خرید برای شناخت الگوی خرید و طراحی پیشنهادات مناسب استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل رفتار خرید", "میانگین ارزش خرید = مجموع درآمد / تعداد خرید")]
    public decimal? AverageOrderValue { get; set; }

    [DisplayName("فرکانس خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "فرکانس خرید برای پیش‌بینی رفتار آینده و برنامه‌ریزی کمپین‌ها استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل رفتار خرید", "فرکانس خرید = تعداد خریدها در واحد زمان (مثلاً در ماه)")]
    public decimal? PurchaseFrequency { get; set; }

    [DisplayName("میانگین زمان بین خریدها (روز)")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "میانگین زمان بین خریدها برای پیش‌بینی خرید بعدی و زمان‌بندی کمپین‌ها استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل رفتار خرید", "میانگین زمان بین خریدها = مجموع فاصله‌های زمانی بین خریدهای متوالی / تعداد فاصله‌ها")]
    public int? AverageTimeBetweenPurchasesDays { get; set; }

    [DisplayName("تاریخ اولین خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل چرخه حیات", "تاریخ اولین خرید برای محاسبه طول عمر مشتری-محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل چرخه حیات", "تاریخ اولین خرید = تاریخ اولین تراکنش خرید این محصول توسط مشتری")]
    public DateTime? FirstPurchaseDate { get; set; }

    [DisplayName("تاریخ آخرین خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل فعالیت", "تاریخ آخرین خرید برای سنجش Recency و تحلیل فعالیت مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل فعالیت", "تاریخ آخرین خرید = تاریخ آخرین تراکنش خرید این محصول توسط مشتری")]
    public DateTime? LastPurchaseDate { get; set; }

    [DisplayName("روز از آخرین خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل فعالیت", "روز از آخرین خرید برای شناسایی مشتریان غیرفعال و برنامه‌ریزی کمپین‌های فعال‌سازی مجدد استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل فعالیت", "روز از آخرین خرید = تاریخ امروز - تاریخ آخرین خرید")]
    public int? DaysSinceLastPurchase { get; set; }

    // تعامل و رضایت
    [DisplayName("نرخ خرید مجدد (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل وفاداری", "نرخ خرید مجدد برای ارزیابی وفاداری مشتری و رضایت از محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل وفاداری", "نرخ خرید مجدد = (تعداد مشتریان با بیش از یک خرید / تعداد کل مشتریان) × 100")]
    public decimal? RepeatPurchaseRate { get; set; }

    [DisplayName("نرخ حفظ (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل حفظ", "نرخ حفظ برای ارزیابی موفقیت استراتژی‌های نگهداشت مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل حفظ", "نرخ حفظ = (تعداد مشتریان نگه‌داشته شده / تعداد کل مشتریان) × 100")]
    public decimal? RetentionRate { get; set; }

    [DisplayName("نمره رضایت")]
    [SBVR(SBVRModality.Calculated, "تحلیل رضایت", "نمره رضایت برای سنجش میزان رضایت کلی مشتری از محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل رضایت", "نمره رضایت بر اساس نظرسنجی‌ها و بازخوردهای مشتری محاسبه می‌شود")]
    public decimal? SatisfactionScore { get; set; }

    [DisplayName("نمره تعامل")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "نمره تعامل برای سنجش میزان مشارکت مشتری در فعالیت‌های مرتبط با محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل تعامل", "نمره تعامل بر اساس تعداد و کیفیت تعاملات مشتری با محصول محاسبه می‌شود")]
    public decimal? EngagementScore { get; set; }

    // پیش‌بینی
    [DisplayName("احتمال خرید بعدی (%)")]
    [SBVR(SBVRModality.Predicted, "پیش‌بینی خرید", "احتمال خرید بعدی برای برنامه‌ریزی کمپین‌های retention و پیش‌بینی جریان نقدی استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "پیش‌بینی خرید", "احتمال خرید بعدی با استفاده از مدل‌های Machine Learning بر اساس الگوهای خرید قبلی محاسبه می‌شود")]
    public decimal? NextPurchaseProbability { get; set; }

    [DisplayName("تاریخ پیش‌بینی خرید بعدی")]
    [SBVR(SBVRModality.Predicted, "پیش‌بینی زمان خرید", "تاریخ پیش‌بینی خرید بعدی برای زمان‌بندی کمپین‌ها و برنامه‌ریزی موجودی استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "پیش‌بینی زمان خرید", "تاریخ پیش‌بینی بر اساس الگوهای تاریخی و میانگین زمان بین خریدها محاسبه می‌شود")]
    public DateTime? PredictedNextPurchaseDate { get; set; }

    [DisplayName("ارزش پیش‌بینی خرید بعدی")]
    [SBVR(SBVRModality.Predicted, "پیش‌بینی ارزش خرید", "ارزش پیش‌بینی خرید بعدی برای برآورد درآمد آینده و برنامه‌ریزی مالی استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "پیش‌بینی ارزش خرید", "ارزش پیش‌بینی بر اساس میانگین ارزش خریدهای قبلی و روند تغییرات محاسبه می‌شود")]
    public decimal? PredictedNextPurchaseValue { get; set; }

    [DisplayName("تاریخ آخرین به‌روزرسانی")]
    [SBVR(SBVRModality.Calculated, "مدیریت داده‌ها", "تاریخ آخرین به‌روزرسانی برای مدیریت freshness داده‌ها و تصمیم‌گیری در مورد نیاز به محاسبه مجدد استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "مدیریت داده‌ها", "تاریخ آخرین به‌روزرسانی = زمان اجرای آخرین محاسبه یا به‌روزرسانی معیارها")]
    public DateTime? LastMetricsUpdateDate { get; set; }
}

