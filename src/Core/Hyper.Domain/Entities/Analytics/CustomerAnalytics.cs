namespace Hyper.Domain.Entities.Analytics;

/// <summary>
/// تحلیل مشتری - تحلیل جامع رفتار، ارزش و پتانسیل مشتریان
/// این موجودیت شامل تمام شاخص‌های مهم CRM و بازاریابی پیشرفته می‌باشد
/// شامل تحلیل RFM، CLV، NPS، رضایت، وفاداری و پیش‌بینی رفتار مشتری
/// </summary>
[DisplayName("تحلیل مشتری")]
public class CustomerAnalytics : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه مشتری - رابطه مشتری در tenant مشخص
    /// </summary>
    [DisplayName("شناسه مشتری")]
    public int CustomerTenantId { get; set; }

    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// شناسه اکوسیستم - اکوسیستمی که تحلیل در آن انجام شده است
    /// </summary>
    [DisplayName("اکوسیستم")]
    public int TenantId { get; set; }

    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// تاریخ تحلیل - تاریخ انجام تحلیل مشتری
    /// </summary>

    [DisplayName("تاریخ تحلیل")]
    public DateTime AnalysisDate { get; set; }

    /// <summary>
    /// دوره تحلیل - دوره زمانی که تحلیل بر اساس آن انجام شده است
    /// </summary>

    [DisplayName("دوره تحلیل")]
    public AnalysisPeriod Period { get; set; }

    // ===== RFM ANALYSIS =====
    /// <summary>
    /// نمره Recency - نمره تازگی آخرین فعالیت مشتری (1-5)
    /// هرچه عدد بالاتر باشد، مشتری فعال‌تر است
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره Recency", "نمره Recency بر اساس فاصله آخرین فعالیت مشتری محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره Recency", "نمره 5 برای فعالیت‌های اخیر، نمره 1 برای فعالیت‌های قدیمی")]
    [DisplayName("نمره تازگی (Recency)")]
    public int RecencyScore { get; set; }

    /// <summary>
    /// نمره Frequency - نمره تکرار فعالیت‌های مشتری (1-5)
    /// هرچه عدد بالاتر باشد، مشتری بیشتر فعالیت کرده است
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره Frequency", "نمره Frequency بر اساس تعداد فعالیت‌های مشتری در دوره مشخص محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره Frequency", "نمره 5 برای مشتریان بسیار فعال، نمره 1 برای مشتریان کم‌فعال")]
    [DisplayName("نمره تکرار (Frequency)")]
    public int FrequencyScore { get; set; }

    /// <summary>
    /// نمره Monetary - نمره ارزش مالی مشتری (1-5)
    /// هرچه عدد بالاتر باشد، مشتری ارزشمندتر است
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره Monetary", "نمره Monetary بر اساس مبلغ کل خریدهای مشتری محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره Monetary", "نمره 5 برای مشتریان با ارزش بالا، نمره 1 برای مشتریان با ارزش پایین")]
    [DisplayName("نمره ارزش مالی (Monetary)")]
    public int MonetaryScore { get; set; }

    /// <summary>
    /// نمره ترکیبی RFM - ترکیب سه نمره RFM (مثل "555" یا "321")
    /// حداکثر 10 کاراکتر
    /// </summary>

    [MaxLength(10)]
    [SBVR(SBVRModality.Calculated, "نمره ترکیبی RFM", "نمره ترکیبی RFM = ترکیب Recency + Frequency + Monetary")]
    [SBVR(SBVRModality.Recommended, "نمره ترکیبی RFM", "مثال: 555 = مشتری چمپیون، 111 = مشتری خوابیده")]
    [DisplayName("نمره ترکیبی RFM")]
    public string RFMScore { get; set; } = null!;

    /// <summary>
    /// بخش RFM - بخش‌بندی مشتری بر اساس نمره RFM
    /// </summary>

    [SBVR(SBVRModality.Calculated, "بخش RFM", "بخش RFM بر اساس نمره ترکیبی RFM تعیین می‌شود")]
    [SBVR(SBVRModality.Recommended, "بخش RFM", "چمپیون، وفاداران، پتانسیل بالا، جدید، در خطر، نمی‌توان نگه داشت، خوابیده، از دست رفته")]
    [DisplayName("بخش RFM")]
    public RFMSegment RFMSegment { get; set; }

    // ===== CUSTOMER LIFETIME VALUE =====
    /// <summary>
    /// ارزش طول عمر مشتری - ارزش کل مشتری در طول عمرش
    /// </summary>

    [SBVR(SBVRModality.Calculated, "ارزش طول عمر مشتری", "CLV = مجموع تمام درآمدهای حاصل از مشتری منهای هزینه‌های کسب و نگهداری")]
    [SBVR(SBVRModality.Recommended, "ارزش طول عمر مشتری", "CLV بالا نشان‌دهنده مشتری ارزشمند و سودآور است")]
    [DisplayName("ارزش طول عمر مشتری (CLV)")]
    public decimal CustomerLifetimeValue { get; set; }

    /// <summary>
    /// ارزش پیش‌بینی شده - ارزش پیش‌بینی شده مشتری در آینده
    /// </summary>

    [SBVR(SBVRModality.Predicted, "ارزش پیش‌بینی شده", "ارزش پیش‌بینی شده بر اساس الگوهای رفتاری گذشته و مدل‌های پیش‌بینی محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "ارزش پیش‌بینی شده", "این شاخص برای برنامه‌ریزی بازاریابی و تخصیص منابع استفاده می‌شود")]
    [DisplayName("ارزش پیش‌بینی شده")]
    public decimal PredictedValue { get; set; }

    // ===== RETENTION METRICS =====
    /// <summary>
    /// نرخ نگهداری - درصد احتمال نگهداری مشتری (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نرخ نگهداری", "نرخ نگهداری = (تعداد مشتریان نگه‌داشته شده / تعداد کل مشتریان) × 100")]
    [SBVR(SBVRModality.Recommended, "نرخ نگهداری", "نرخ نگهداری بالای 80% نشان‌دهنده کیفیت خوب خدمات است")]
    [DisplayName("نرخ نگهداری")]
    public decimal RetentionRate { get; set; }

    /// <summary>
    /// احتمال ترک - درصد احتمال ترک مشتری (0-100)
    /// </summary>

    [SBVR(SBVRModality.Predicted, "احتمال ترک", "احتمال ترک بر اساس الگوهای رفتاری و مدل‌های پیش‌بینی محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "احتمال ترک", "احتمال ترک بالای 70% نیاز به اقدام فوری برای نگهداری مشتری دارد")]
    [DisplayName("احتمال ترک")]
    public decimal ChurnProbability { get; set; }

    // ===== ENGAGEMENT METRICS =====
    /// <summary>
    /// نمره تعامل - نمره کلی تعامل مشتری با سیستم (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره تعامل", "نمره تعامل بر اساس تعداد و کیفیت تعاملات مشتری محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره تعامل", "نمره تعامل بالا نشان‌دهنده مشتری فعال و درگیر است")]
    [DisplayName("نمره تعامل")]
    public decimal EngagementScore { get; set; }

    /// <summary>
    /// تعداد تعاملات - تعداد کل تعاملات مشتری با سیستم
    /// </summary>

    [SBVR(SBVRModality.Calculated, "تعداد تعاملات", "تعداد تعاملات شامل کلیه فعالیت‌های مشتری در سیستم است")]
    [DisplayName("تعداد تعاملات")]
    public int InteractionCount { get; set; }

    /// <summary>
    /// آخرین فعالیت - تاریخ آخرین فعالیت مشتری
    /// </summary>

    [SBVR(SBVRModality.Calculated, "آخرین فعالیت", "آخرین فعالیت بر اساس آخرین رکورد تعامل مشتری ثبت می‌شود")]
    [DisplayName("آخرین فعالیت")]
    public DateTime LastActivityDate { get; set; }

    // ===== NPS METRICS =====
    /// <summary>
    /// نمره NPS - نمره Net Promoter Score مشتری (-100 تا +100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره NPS", "NPS = درصد Promoters - درصد Detractors")]
    [SBVR(SBVRModality.Recommended, "نمره NPS", "NPS بالای 50 نشان‌دهنده رضایت بالا و احتمال ارجاع زیاد است")]
    [DisplayName("نمره NPS")]
    public decimal NetPromoterScore { get; set; }

    /// <summary>
    /// نمره رضایت - نمره کلی رضایت مشتری (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره رضایت", "نمره رضایت بر اساس نظرسنجی‌ها و بازخوردهای مشتری محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره رضایت", "نمره رضایت بالای 80 نشان‌دهنده کیفیت خوب خدمات است")]
    [DisplayName("نمره رضایت")]
    public decimal SatisfactionScore { get; set; }

    // ===== ADDITIONAL CRM METRICS =====
    /// <summary>
    /// نمره وفاداری - نمره وفاداری مشتری به برند (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره وفاداری", "نمره وفاداری برای شناسایی مشتریان کلیدی و برنامه‌ریزی استراتژی نگهداری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره وفاداری", "نمره وفاداری بر اساس تکرار خرید، دفعات بازگشت، نرخ تعامل و رفتار خرید محاسبه می‌شود")]
    [DisplayName("نمره وفاداری")]
    public decimal LoyaltyScore { get; set; }

    /// <summary>
    /// نمره ارزش - نمره ارزش کلی مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره ارزش", "نمره ارزش برای تعیین اولویت مشتریان و تخصیص منابع بازاریابی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره ارزش", "نمره ارزش = (نمره RFM × 0.4) + (CLV × 0.3) + (نرخ نگهداری × 0.3)")]
    [DisplayName("نمره ارزش")]
    public decimal ValueScore { get; set; }

    /// <summary>
    /// نمره ریسک - نمره ریسک از دست دادن مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره ریسک", "نمره ریسک بالا نیاز به اقدام فوری برای نگهداری مشتری دارد")]
    [SBVR(SBVRModality.Predicted, "نمره ریسک", "نمره ریسک بر اساس کاهش فعالیت، افزایش فاصله خرید، و الگوهای رفتاری نگران‌کننده محاسبه می‌شود")]
    [DisplayName("نمره ریسک")]
    public decimal RiskScore { get; set; }

    /// <summary>
    /// نمره پتانسیل - نمره پتانسیل رشد مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره پتانسیل", "نمره پتانسیل برای شناسایی مشتریان با ظرفیت رشد و طراحی کمپین‌های هدفمند استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "نمره پتانسیل", "نمره پتانسیل بر اساس الگوهای خرید، رفتار مرور، تعامل با محصولات و پیش‌بینی CLV محاسبه می‌شود")]
    [DisplayName("نمره پتانسیل")]
    public decimal PotentialScore { get; set; }

    // ===== PURCHASE METRICS =====
    /// <summary>
    /// تعداد خرید - تعداد کل خریدهای مشتری
    /// </summary>
    [SBVR(SBVRModality.Recommended, "تعداد خرید", "تعداد خرید برای سنجش وفاداری و پایگاه داده بازاریابی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد خرید", "تعداد خرید = جمع تمام تراکنش‌های خرید مشتری در دوره مشخص")]
    [DisplayName("تعداد خرید")]
    public int PurchaseCount { get; set; }

    /// <summary>
    /// مبلغ کل خرید - مجموع مبلغ تمام خریدهای مشتری
    /// </summary>
    [SBVR(SBVRModality.Recommended, "مبلغ کل خرید", "مبلغ کل خرید برای محاسبه ارزش مشتری و سنجش سودآوری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "مبلغ کل خرید", "مبلغ کل خرید = مجموع (Credit) تمام تراکنش‌های مشتری در دوره مشخص")]
    [DisplayName("مبلغ کل خرید")]
    public decimal TotalPurchaseAmount { get; set; }

    /// <summary>
    /// میانگین خرید - میانگین مبلغ هر خرید مشتری
    /// </summary>
    [SBVR(SBVRModality.Recommended, "میانگین خرید", "میانگین خرید برای شناخت الگوی خرید و طراحی پیشنهادات مناسب استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "میانگین خرید", "میانگین خرید = مبلغ کل خرید / تعداد خرید")]
    [DisplayName("میانگین مبلغ خرید")]
    public decimal AveragePurchaseAmount { get; set; }

    /// <summary>
    /// آخرین خرید - تاریخ آخرین خرید مشتری (اختیاری)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "آخرین خرید", "آخرین خرید برای سنجش Recency در تحلیل RFM استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "آخرین خرید", "آخرین خرید = تاریخ آخرین تراکنش Credit مشتری")]
    [DisplayName("آخرین خرید")]
    public DateTime? LastPurchaseDate { get; set; }

    /// <summary>
    /// فاصله خرید - فاصله زمانی بین خریدها (روز)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "فاصله خرید", "فاصله خرید برای درک الگوی خرید و برنامه‌ریزی کمپین‌های زمان‌بندی شده استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "فاصله خرید", "فاصله خرید = میانگین فاصله زمانی بین تاریخ‌های خرید متوالی")]
    [DisplayName("فاصله خرید")]
    public int PurchaseInterval { get; set; }

    // ===== VISIT METRICS =====
    /// <summary>
    /// تعداد بازدید - تعداد کل بازدیدهای مشتری
    /// </summary>
    [SBVR(SBVRModality.Recommended, "تعداد بازدید", "تعداد بازدید برای سنجش تعامل و علاقه مشتری به محصولات استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد بازدید", "تعداد بازدید = جمع تمام EventLog هایی که نوع رویداد آن 'Visit' است")]
    [DisplayName("تعداد بازدید")]
    public int VisitCount { get; set; }

    /// <summary>
    /// آخرین بازدید - تاریخ آخرین بازدید مشتری (اختیاری)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "آخرین بازدید", "آخرین بازدید برای سنجش Recency و برنامه‌ریزی engagement strategies استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "آخرین بازدید", "آخرین بازدید = تاریخ آخرین EventLog با نوع 'Visit'")]
    [DisplayName("آخرین بازدید")]
    public DateTime? LastVisitDate { get; set; }

    /// <summary>
    /// زمان حضور - میانگین زمان حضور در هر جلسه (دقیقه)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "زمان حضور", "زمان حضور برای سنجش کیفیت تعامل و علاقه مشتری به محصولات استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "زمان حضور", "زمان حضور = میانگین (EndTime - StartTime) برای تمام Session های مشتری")]
    [DisplayName("میانگین زمان حضور")]
    public int SessionDuration { get; set; }

    // ===== CONVERSION METRICS =====
    /// <summary>
    /// نرخ تبدیل - درصد تبدیل بازدید به خرید (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نرخ تبدیل", "نرخ تبدیل برای ارزیابی اثربخشی بازاریابی و UX استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ تبدیل", "نرخ تبدیل = (تعداد خرید / تعداد بازدید) × 100")]
    [DisplayName("نرخ تبدیل")]
    public decimal ConversionRate { get; set; }

    /// <summary>
    /// نرخ بازگشت - درصد بازگشت مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نرخ بازگشت", "نرخ بازگشت برای سنجش وفاداری و رضایت مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ بازگشت", "نرخ بازگشت = (تعداد بازدیدهای بیشتر از 1 / تعداد کل بازدیدها) × 100")]
    [DisplayName("نرخ بازگشت")]
    public decimal ReturnRate { get; set; }

    /// <summary>
    /// نرخ ارجاع - درصد ارجاع مشتری به دیگران (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نرخ ارجاع", "نرخ ارجاع برای ارزیابی اثربخشی برنامه‌های referral و رضایت مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ ارجاع", "نرخ ارجاع = (تعداد ارجاع های موفق / تعداد کل مشتریان ارجاع شده) × 100")]
    [DisplayName("نرخ ارجاع")]
    public decimal ReferralRate { get; set; }

    /// <summary>
    /// تعداد ارجاع - تعداد کل ارجاعات مشتری
    /// </summary>
    [SBVR(SBVRModality.Recommended, "تعداد ارجاع", "تعداد ارجاع برای ارزیابی ارزش مشتری و طراحی incentive programs استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد ارجاع", "تعداد ارجاع = شمارش مشتریانی که توسط این مشتری معرفی شده‌اند")]
    [DisplayName("تعداد ارجاع")]
    public int ReferralCount { get; set; }

    // ===== QUALITY METRICS =====
    /// <summary>
    /// نمره کیفیت - نمره کلی کیفیت تعامل مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره کیفیت", "نمره کیفیت برای شناسایی نقاط قوت و ضعف تجربه مشتری و بهبود خدمات استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره کیفیت", "نمره کیفیت = میانگین وزن‌دار تمام نمرات رضایت از خدمات، محصول، قیمت و تجربه")]
    [DisplayName("نمره کیفیت")]
    public decimal QualityScore { get; set; }

    /// <summary>
    /// نمره رضایت از خدمات - نمره رضایت از خدمات ارائه شده (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره رضایت از خدمات", "نمره رضایت از خدمات برای ارزیابی کیفیت خدمات و شناسایی نیاز به بهبود استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره رضایت از خدمات", "نمره رضایت از خدمات = میانگین امتیازات نظرسنجی‌های مرتبط با کیفیت خدمات")]
    [DisplayName("رضایت از خدمات")]
    public decimal ServiceSatisfactionScore { get; set; }

    /// <summary>
    /// نمره رضایت از محصول - نمره رضایت از محصولات ارائه شده (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره رضایت از محصول", "نمره رضایت از محصول برای ارزیابی مطابقت محصول با نیازهای مشتری و طراحی محصولات بهتر استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره رضایت از محصول", "نمره رضایت از محصول = میانگین امتیازات نظرسنجی‌های مرتبط با کیفیت و عملکرد محصولات")]
    [DisplayName("رضایت از محصول")]
    public decimal ProductSatisfactionScore { get; set; }

    /// <summary>
    /// نمره رضایت از قیمت - نمره رضایت از قیمت‌گذاری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره رضایت از قیمت", "نمره رضایت از قیمت برای ارزیابی استراتژی قیمت‌گذاری و تعادل قیمت-ارزش استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره رضایت از قیمت", "نمره رضایت از قیمت = میانگین امتیازات نظرسنجی‌های مرتبط با قیمت‌گذاری و ارزش پول")]
    [DisplayName("رضایت از قیمت")]
    public decimal PriceSatisfactionScore { get; set; }

    /// <summary>
    /// نمره رضایت از تجربه - نمره رضایت از تجربه کلی مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره رضایت از تجربه", "نمره رضایت از تجربه برای ارزیابی کلی CX و شناسایی فرصت‌های بهبود کلی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره رضایت از تجربه", "نمره رضایت از تجربه = میانگین وزن‌دار تمام جنبه‌های تعامل مشتری شامل touchpoints مختلف")]
    [DisplayName("رضایت از تجربه")]
    public decimal ExperienceSatisfactionScore { get; set; }

    /// <summary>
    /// نمره رضایت کلی - نمره رضایت کلی مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره رضایت کلی", "نمره رضایت کلی به عنوان یک شاخص کلیدی سلامت رابطه با مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره رضایت کلی", "نمره رضایت کلی = (رضایت خدمات × 0.25) + (رضایت محصول × 0.30) + (رضایت قیمت × 0.20) + (رضایت تجربه × 0.25)")]
    [DisplayName("رضایت کلی")]
    public decimal OverallSatisfactionScore { get; set; }

    // ===== PREDICTION METRICS =====
    /// <summary>
    /// احتمال خرید مجدد - درصد احتمال خرید مجدد مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "احتمال خرید مجدد", "احتمال خرید مجدد برای برنامه‌ریزی کمپین‌های retention و پیش‌بینی جریان نقدی استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال خرید مجدد", "احتمال خرید مجدد با استفاده از مدل‌های Machine Learning بر اساس الگوهای خرید قبلی، فاصله زمانی، و رفتار تعامل محاسبه می‌شود")]
    [DisplayName("احتمال خرید مجدد")]
    public decimal RepurchaseProbability { get; set; }

    /// <summary>
    /// احتمال ارجاع - درصد احتمال ارجاع مشتری به دیگران (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "احتمال ارجاع", "احتمال ارجاع برای شناسایی مشتریان بالقوه خوشنود و طراحی برنامه‌های referral استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال ارجاع", "احتمال ارجاع با استفاده از مدل‌های پیش‌بینی بر اساس رضایت، مدت زمان عضویت، دفعات خرید و تعامل با محصولات محاسبه می‌شود")]
    [DisplayName("احتمال ارجاع")]
    public decimal ReferralProbability { get; set; }


    /// <summary>
    /// احتمال ارتقا - درصد احتمال ارتقای سطح مشتری (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "احتمال ارتقا", "احتمال ارتقا برای شناسایی مشتریان با پتانسیل رشد و طراحی استراتژی‌های upselling استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال ارتقا", "احتمال ارتقا با استفاده از تحلیل الگوهای مصرف، ظرفیت پرداخت، و تعامل با محصولات premium محاسبه می‌شود")]
    [DisplayName("احتمال ارتقا")]
    public decimal UpgradeProbability { get; set; }

    /// <summary>
    /// احتمال خرید اضافی - درصد احتمال خرید محصولات اضافی (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "احتمال خرید اضافی", "احتمال خرید اضافی برای شناسایی فرصت‌های cross-selling و افزایش سودآوری مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال خرید اضافی", "احتمال خرید اضافی با استفاده از تحلیل همبستگی محصولات و الگوهای خرید مشابه محاسبه می‌شود")]
    [DisplayName("احتمال خرید اضافی (Cross-Sell)")]
    public decimal CrossSellProbability { get; set; }

    /// <summary>
    /// احتمال خرید مکمل - درصد احتمال خرید محصولات مکمل (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "احتمال خرید مکمل", "احتمال خرید مکمل برای طراحی استراتژی‌های complementary product bundling و افزایش ارزش سبد خرید استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال خرید مکمل", "احتمال خرید مکمل با استفاده از تحلیل الگوهای خرید محصولات مکمل و رفتار مشتریان مشابه محاسبه می‌شود")]
    [DisplayName("احتمال خرید مکمل (Up-Sell)")]
    public decimal UpsellProbability { get; set; }

    // ===== MODEL METRICS =====
    /// <summary>
    /// نمره پیش‌بینی - نمره کلی پیش‌بینی مدل (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "نمره پیش‌بینی", "نمره پیش‌بینی برای سنجش قدرت پیش‌بینی‌های مدل و تصمیم‌گیری مبتنی بر داده استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره پیش‌بینی", "نمره پیش‌بینی = ترکیب وزن‌دار تمام احتمال‌های پیش‌بینی شده شامل خرید مجدد، ارجاع، ارتقا و upsell")]
    [DisplayName("نمره پیش‌بینی")]
    public decimal PredictionScore { get; set; }

    /// <summary>
    /// دقت پیش‌بینی - دقت مدل پیش‌بینی (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "دقت پیش‌بینی", "دقت پیش‌بینی برای ارزیابی کیفیت مدل و اعتماد به پیش‌بینی‌ها استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "دقت پیش‌بینی", "دقت پیش‌بینی = (تعداد پیش‌بینی‌های صحیح / تعداد کل پیش‌بینی‌ها) × 100، بر اساس validation set")]
    [DisplayName("دقت پیش‌بینی")]
    public decimal PredictionAccuracy { get; set; }

    /// <summary>
    /// اعتماد پیش‌بینی - سطح اعتماد به پیش‌بینی (0-100)
    /// </summary>
    [SBVR(SBVRModality.Recommended, "اعتماد پیش‌بینی", "اعتماد پیش‌بینی برای ارزیابی اطمینان به نتایج و تصمیم‌گیری‌های استراتژیک استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "اعتماد پیش‌بینی", "اعتماد پیش‌بینی بر اساس تقارب توزیع احتمال و انحراف معیار نمونه‌های bootstrap محاسبه می‌شود")]
    [DisplayName("اعتماد پیش‌بینی")]
    public decimal PredictionConfidence { get; set; }

    /// <summary>
    /// تاریخ آخرین به‌روزرسانی - تاریخ آخرین به‌روزرسانی تحلیل
    /// </summary>
    [SBVR(SBVRModality.Recommended, "تاریخ آخرین به‌روزرسانی", "تاریخ آخرین به‌روزرسانی برای مدیریت freshness داده‌ها و تصمیم‌گیری در مورد نیاز به تحلیل مجدد استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تاریخ آخرین به‌روزرسانی", "تاریخ آخرین به‌روزرسانی = زمان اجرای آخرین محاسبه یا تحلیل برای این مشتری")]
    [DisplayName("آخرین به‌روزرسانی")]
    public DateTime LastUpdatedDate { get; set; }

    /// <summary>
    /// نسخه مدل - نسخه مدل تحلیل استفاده شده (اختیاری)
    /// حداکثر 50 کاراکتر
    /// </summary>
    [MaxLength(50)]
    [SBVR(SBVRModality.Recommended, "نسخه مدل", "نسخه مدل برای ردیابی نسخه‌های مختلف مدل و مدیریت reproducibility تحلیل‌ها استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نسخه مدل", "نسخه مدل = شناسه نسخه مدل تحلیل که در زمان محاسبه استفاده شده است")]
    [DisplayName("نسخه مدل تحلیل")]
    public string? ModelVersion { get; set; }

    /// <summary>
    /// داده‌های اضافی - داده‌های اضافی تحلیل در فرمت JSON (اختیاری)
    /// حداکثر 2000 کاراکتر
    /// </summary>
    [MaxLength(2000)]
    [SBVR(SBVRModality.Recommended, "داده‌های اضافی", "داده‌های اضافی برای ذخیره پارامترهای خاص مدل، جزئیات محاسبات یا نتیجه‌های میانی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "داده‌های اضافی", "داده‌های اضافی = JSON حاوی اطلاعات تکمیلی تحلیل شامل اعداد اعشاری نمرات، فاکتورهای موثر و metadata محاسبه")]
    [DisplayName("داده‌های اضافی تحلیل")]
    public string? AdditionalData { get; set; }
}
