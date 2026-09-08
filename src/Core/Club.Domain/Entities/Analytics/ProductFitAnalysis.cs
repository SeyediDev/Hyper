namespace Hyper.Domain.Entities.Analytics;

/// <summary>
/// تحلیل تناسب محصول - تحلیل تناسب محصول با بازار و خدمات
/// این موجودیت شامل تمام شاخص‌های مهم تناسب محصول می‌باشد
/// شامل تحلیل PMF/PSF، UVP/USP، رضایت مشتری و پیش‌بینی موفقیت
/// </summary>
[DisplayName("تحلیل تناسب محصول")]
public class ProductFitAnalysis : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم - اکوسیستمی که تحلیل در آن انجام شده است
    /// </summary>
    public int TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه محصول - محصول یا خدمت که این تحلیل برای آن انجام شده است
    /// </summary>
    public int ProductId { get; set; }

    [DisplayName("محصول")]
    [SBVR(SBVRModality.Obligatory, "تحلیل محصول-بازار", "هر تحلیل تناسب محصول باید به یک محصول مشخص تعلق داشته باشد")]
    public Product Product { get; set; } = null!;

    /// <summary>
    /// عنوان تحلیل - عنوان مشخص کننده تحلیل
    /// حداکثر 100 کاراکتر
    /// </summary>

    [MaxLength(100)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// تاریخ تحلیل - تاریخ انجام تحلیل تناسب محصول
    /// </summary>

    public DateTime AnalysisDate { get; set; }

    // ===== PRODUCT MARKET FIT =====
    /// <summary>
    /// نمره تناسب محصول-بازار (PMF) - نمره تناسب محصول با نیازهای بازار (0-100)
    /// هرچه عدد بالاتر باشد، محصول بهتر با بازار تناسب دارد
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره تناسب محصول-بازار (PMF)", "PMF = (رضایت مشتری + نرخ نگهداری + نرخ رشد) / 3")]
    [SBVR(SBVRModality.Recommended, "نمره تناسب محصول-بازار (PMF)", "PMF بالای 70 نشان‌دهنده تناسب خوب محصول با بازار است")]
    public decimal ProductMarketFitScore { get; set; }

    /// <summary>
    /// نمره تناسب محصول-خدمت (PSF) - نمره تناسب محصول با خدمات ارائه شده (0-100)
    /// هرچه عدد بالاتر باشد، محصول بهتر با خدمات تناسب دارد
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره تناسب محصول-خدمت (PSF)", "PSF = (کیفیت خدمات + رضایت از خدمات + نرخ حل مشکل) / 3")]
    [SBVR(SBVRModality.Recommended, "نمره تناسب محصول-خدمت (PSF)", "PSF بالای 75 نشان‌دهنده تناسب خوب محصول با خدمات است")]
    public decimal ProductServiceFitScore { get; set; }

    // ===== VALUE PROPOSITION =====
    /// <summary>
    /// پیشنهاد ارزش منحصر (UVP) - پیشنهاد ارزش منحصر محصول
    /// حداکثر 500 کاراکتر
    /// </summary>

    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "پیشنهاد ارزش منحصر (UVP)", "UVP باید واضح، مختصر و متمایز از رقبا باشد")]
    [SBVR(SBVRModality.Recommended, "پیشنهاد ارزش منحصر (UVP)", "UVP باید شامل مزیت اصلی و ارزش منحصر محصول باشد")]
    public string? UniqueValueProposition { get; set; }

    /// <summary>
    /// نقطه فروش منحصر (USP) - نقطه فروش منحصر محصول
    /// حداکثر 500 کاراکتر
    /// </summary>

    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "نقطه فروش منحصر (USP)", "USP باید شامل ویژگی منحصر و مزیت رقابتی باشد")]
    [SBVR(SBVRModality.Recommended, "نقطه فروش منحصر (USP)", "USP باید قابل اثبات و متمایز از رقبا باشد")]
    public string? UniqueSellingProposition { get; set; }

    /// <summary>
    /// نمره ارزش - نمره کلی ارزش محصول (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره ارزش", "نمره ارزش = (کیفیت + عملکرد + قیمت + خدمات) / 4")]
    [SBVR(SBVRModality.Recommended, "نمره ارزش", "نمره ارزش بالای 80 نشان‌دهنده محصول با ارزش بالا است")]
    public decimal ValueScore { get; set; }

    // ===== CUSTOMER SATISFACTION =====
    /// <summary>
    /// نمره رضایت مشتری - نمره کلی رضایت مشتریان از محصول (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره رضایت مشتری", "نمره رضایت مشتری بر اساس نظرسنجی‌ها و بازخوردها محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره رضایت مشتری", "نمره رضایت بالای 85 نشان‌دهنده کیفیت عالی محصول است")]
    public decimal CustomerSatisfactionScore { get; set; }

    /// <summary>
    /// نمره NPS - نمره Net Promoter Score محصول (-100 تا +100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره NPS", "NPS = درصد Promoters - درصد Detractors")]
    [SBVR(SBVRModality.Recommended, "نمره NPS", "NPS بالای 50 نشان‌دهنده رضایت بالا و احتمال ارجاع زیاد است")]
    public decimal NetPromoterScore { get; set; }

    /// <summary>
    /// نمره CSAT - نمره Customer Satisfaction محصول (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره CSAT", "CSAT = میانگین نمرات رضایت مشتریان")]
    [SBVR(SBVRModality.Recommended, "نمره CSAT", "CSAT بالای 80 نشان‌دهنده رضایت خوب مشتریان است")]
    public decimal CustomerSatisfactionRating { get; set; }

    // ===== PRODUCT METRICS =====
    /// <summary>
    /// نمره کیفیت محصول - نمره کیفیت محصول (0-100)
    /// </summary>
    [DisplayName("نمره کیفیت محصول")]
    [SBVR(SBVRModality.Recommended, "نمره کیفیت محصول", "نمره کیفیت محصول برای ارزیابی برتری محصول و رضایت مشتری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره کیفیت محصول", "نمره کیفیت محصول بر اساس نرخ مشکل، رضایت مشتری و رضایت از کیفیت محاسبه می‌شود")]
    public decimal ProductQualityScore { get; set; }

    /// <summary>
    /// نمره عملکرد محصول - نمره عملکرد محصول (0-100)
    /// </summary>
    [DisplayName("نمره عملکرد محصول")]
    [SBVR(SBVRModality.Recommended, "نمره عملکرد محصول", "نمره عملکرد محصول برای ارزیابی کارایی و تأثیر محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره عملکرد محصول", "نمره عملکرد محصول بر اساس معیارهای عملکردی، سرعت، و کارایی محاسبه می‌شود")]
    public decimal ProductPerformanceScore { get; set; }

    /// <summary>
    /// نمره قابلیت استفاده - نمره قابلیت استفاده محصول (0-100)
    /// </summary>
    [DisplayName("نمره قابلیت استفاده")]
    [SBVR(SBVRModality.Recommended, "نمره قابلیت استفاده", "نمره قابلیت استفاده برای ارزیابی سهولت استفاده و UX محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره قابلیت استفاده", "نمره قابلیت استفاده بر اساس تست کاربر، زمان یادگیری و نرخ خطا محاسبه می‌شود")]
    public decimal UsabilityScore { get; set; }

    /// <summary>
    /// نمره طراحی - نمره طراحی محصول (0-100)
    /// </summary>
    [DisplayName("نمره طراحی")]
    [SBVR(SBVRModality.Recommended, "نمره طراحی", "نمره طراحی برای ارزیابی ظاهر، زیبایی و جذابیت محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره طراحی", "نمره طراحی بر اساس رضایت از ظاهر، زیبایی‌شناسی و جذابیت بصری محاسبه می‌شود")]
    public decimal DesignScore { get; set; }

    /// <summary>
    /// نمره قیمت - نمره مناسب بودن قیمت محصول (0-100)
    /// </summary>
    [DisplayName("نمره قیمت")]
    [SBVR(SBVRModality.Recommended, "نمره قیمت", "نمره قیمت برای ارزیابی رقابتی‌بودن و مناسب بودن قیمت استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره قیمت", "نمره قیمت بر اساس مقایسه با رقبا، ارزش دریافتی و رضایت از قیمت محاسبه می‌شود")]
    public decimal PriceScore { get; set; }

    // ===== MARKET METRICS =====
    /// <summary>
    /// نمره تقاضای بازار - نمره تقاضای بازار برای محصول (0-100)
    /// </summary>
    [DisplayName("نمره تقاضای بازار")]
    [SBVR(SBVRModality.Recommended, "نمره تقاضای بازار", "نمره تقاضای بازار برای ارزیابی علاقه مشتریان و پتانسیل فروش استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره تقاضای بازار", "نمره تقاضای بازار بر اساس جستجوها، سوالات، علاقمندی و Intent خرید محاسبه می‌شود")]
    public decimal MarketDemandScore { get; set; }

    /// <summary>
    /// نمره رقابت - نمره قدرت رقابتی محصول (0-100)
    /// </summary>
    [DisplayName("نمره رقابت")]
    [SBVR(SBVRModality.Recommended, "نمره رقابت", "نمره رقابت برای ارزیابی توانایی رقابت در بازار استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره رقابت", "نمره رقابت بر اساس مقایسه محصول با رقبا در ابعاد مختلف محاسبه می‌شود")]
    public decimal CompetitionScore { get; set; }

    /// <summary>
    /// نمره تمایز - نمره تمایز محصول از رقبا (0-100)
    /// </summary>
    [DisplayName("نمره تمایز")]
    [SBVR(SBVRModality.Recommended, "نمره تمایز", "نمره تمایز برای ارزیابی تفاوت محصول از رقبا و مزیت رقابتی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره تمایز", "نمره تمایز بر اساس تمایزات محصول، USP و تفاوت‌های کلیدی محاسبه می‌شود")]
    public decimal DifferentiationScore { get; set; }

    /// <summary>
    /// نمره نوآوری - نمره نوآوری محصول (0-100)
    /// </summary>
    [DisplayName("نمره نوآوری")]
    [SBVR(SBVRModality.Recommended, "نمره نوآوری", "نمره نوآوری برای ارزیابی خلاقیت و تمایز استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره نوآوری", "نمره نوآوری بر اساس تمایزات نوین، ویژگی‌های منحصر به فرد و تازگی محصول محاسبه می‌شود")]
    public decimal InnovationScore { get; set; }

    // ===== ADOPTION METRICS =====
    /// <summary>
    /// نرخ پذیرش - نرخ پذیرش محصول توسط مشتریان (0-100)
    /// </summary>
    [DisplayName("نرخ پذیرش")]
    [SBVR(SBVRModality.Recommended, "نرخ پذیرش", "نرخ پذیرش برای ارزیابی سرعت انتشار و محبوبیت محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ پذیرش", "نرخ پذیرش = (تعداد کاربران جدید / تعداد کاربران بالقوه) × 100")]
    public decimal AdoptionRate { get; set; }

    /// <summary>
    /// نرخ نگهداری - نرخ نگهداری مشتریان محصول (0-100)
    /// </summary>
    [DisplayName("نرخ نگهداری")]
    [SBVR(SBVRModality.Recommended, "نرخ نگهداری", "نرخ نگهداری برای ارزیابی وفاداری مشتریان و کیفیت محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ نگهداری", "نرخ نگهداری = (تعداد کاربران فعال بعد از دوره / تعداد کاربران اولیه) × 100")]
    public decimal RetentionRate { get; set; }

    /// <summary>
    /// نرخ رشد - نرخ رشد استفاده از محصول (0-100)
    /// </summary>
    [DisplayName("نرخ رشد")]
    [SBVR(SBVRModality.Recommended, "نرخ رشد", "نرخ رشد برای ارزیابی سرعت توسعه محصول و بازار استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ رشد", "نرخ رشد = ((کاربران دوره فعلی - کاربران دوره قبلی) / کاربران دوره قبلی) × 100")]
    public decimal GrowthRate { get; set; }

    /// <summary>
    /// نرخ ارجاع - نرخ ارجاع محصول توسط مشتریان (0-100)
    /// </summary>
    [DisplayName("نرخ ارجاع")]
    [SBVR(SBVRModality.Recommended, "نرخ ارجاع", "نرخ ارجاع برای ارزیابی رضایت مشتریان و ارزش محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ ارجاع", "نرخ ارجاع = (تعداد مشتریان ارجاع شده / تعداد کل مشتریان) × 100")]
    public decimal ReferralRate { get; set; }

    // ===== BUSINESS METRICS =====
    /// <summary>
    /// نرخ تبدیل - نرخ تبدیل مشتریان بالقوه به مشتریان واقعی (0-100)
    /// </summary>
    [DisplayName("نرخ تبدیل")]
    [SBVR(SBVRModality.Recommended, "نرخ تبدیل", "نرخ تبدیل برای ارزیابی اثربخشی بازاریابی و کیفیت محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ تبدیل", "نرخ تبدیل = (تعداد خریداران / تعداد بازدیدکنندگان) × 100")]
    public decimal ConversionRate { get; set; }

    /// <summary>
    /// نرخ بازگشت سرمایه - نرخ بازگشت سرمایه محصول (0-100)
    /// </summary>
    [DisplayName("نرخ بازگشت سرمایه")]
    [SBVR(SBVRModality.Recommended, "نرخ بازگشت سرمایه", "ROI برای ارزیابی سودآوری و موفقیت مالی محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ بازگشت سرمایه", "ROI = ((درآمد - هزینه) / هزینه) × 100")]
    public decimal ROIScore { get; set; }

    /// <summary>
    /// نرخ سودآوری - نرخ سودآوری محصول (0-100)
    /// </summary>
    [DisplayName("نرخ سودآوری")]
    [SBVR(SBVRModality.Recommended, "نرخ سودآوری", "نرخ سودآوری برای ارزیابی کارایی مالی و پایداری محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ سودآوری", "نرخ سودآوری = (سود خالص / درآمد) × 100")]
    public decimal ProfitabilityScore { get; set; }

    /// <summary>
    /// نرخ مقیاس‌پذیری - نرخ مقیاس‌پذیری محصول (0-100)
    /// </summary>
    [DisplayName("نرخ مقیاس‌پذیری")]
    [SBVR(SBVRModality.Recommended, "نرخ مقیاس‌پذیری", "نرخ مقیاس‌پذیری برای ارزیابی توانایی رشد محصول استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ مقیاس‌پذیری", "نرخ مقیاس‌پذیری بر اساس هزینه‌های ثابت، هزینه‌های متغیر و ظرفیت رشد محاسبه می‌شود")]
    public decimal ScalabilityScore { get; set; }

    // ===== PREDICTION METRICS =====
    /// <summary>
    /// احتمال موفقیت - احتمال موفقیت محصول در بازار (0-100)
    /// </summary>
    [DisplayName("احتمال موفقیت")]
    [SBVR(SBVRModality.Recommended, "احتمال موفقیت", "احتمال موفقیت برای تصمیم‌گیری‌های استراتژیک و تخصیص منابع استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال موفقیت", "احتمال موفقیت با استفاده از مدل‌های Machine Learning بر اساس PMF، رضایت و نرخ رشد محاسبه می‌شود")]
    public decimal SuccessProbability { get; set; }

    /// <summary>
    /// احتمال شکست - احتمال شکست محصول در بازار (0-100)
    /// </summary>
    [DisplayName("احتمال شکست")]
    [SBVR(SBVRModality.Recommended, "احتمال شکست", "احتمال شکست برای شناسایی ریسک‌ها و اقدامات پیشگیرانه استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال شکست", "احتمال شکست با استفاده از الگوهای محصولات ناموفق، نرخ ریزش و شکایات محاسبه می‌شود")]
    public decimal FailureProbability { get; set; }

    /// <summary>
    /// احتمال رشد - احتمال رشد محصول در آینده (0-100)
    /// </summary>
    [DisplayName("احتمال رشد")]
    [SBVR(SBVRModality.Recommended, "احتمال رشد", "احتمال رشد برای برنامه‌ریزی آینده و تخصیص منابع استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال رشد", "احتمال رشد با استفاده از روندهای تاریخی، الگوهای بازار و تقاضای پیش‌بینی شده محاسبه می‌شود")]
    public decimal GrowthProbability { get; set; }

    /// <summary>
    /// احتمال رقابت - احتمال موفقیت در رقابت (0-100)
    /// </summary>
    [DisplayName("احتمال رقابت")]
    [SBVR(SBVRModality.Recommended, "احتمال رقابت", "احتمال رقابت برای طراحی استراتژی‌های تمایز و نوآوری استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "احتمال رقابت", "احتمال رقابت با استفاده از تحلیل رقبا، SWOT و تمایزات محصول محاسبه می‌شود")]
    public decimal CompetitionProbability { get; set; }

    // ===== ANALYSIS QUALITY =====
    /// <summary>
    /// دقت تحلیل - دقت تحلیل تناسب محصول (0-100)
    /// </summary>
    [DisplayName("دقت تحلیل")]
    [SBVR(SBVRModality.Recommended, "دقت تحلیل", "دقت تحلیل برای ارزیابی کیفیت داده‌ها و اعتماد به نتایج استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "دقت تحلیل", "دقت تحلیل بر اساس مقایسه پیش‌بینی‌ها با واقعیت در گذشته محاسبه می‌شود")]
    public decimal AnalysisAccuracy { get; set; }

    /// <summary>
    /// اعتماد تحلیل - سطح اعتماد به تحلیل (0-100)
    /// </summary>
    [DisplayName("اعتماد تحلیل")]
    [SBVR(SBVRModality.Recommended, "اعتماد تحلیل", "اعتماد تحلیل برای ارزیابی کیفیت داده‌ها و دقت نتیجه‌گیری استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "اعتماد تحلیل", "اعتماد تحلیل بر اساس کیفیت داده‌ها، روش‌شناسی و نمونه‌گیری محاسبه می‌شود")]
    public decimal AnalysisConfidence { get; set; }

    /// <summary>
    /// منابع داده - منابع داده استفاده شده در تحلیل
    /// حداکثر 500 کاراکتر
    /// </summary>
    [DisplayName("منابع داده")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "منابع داده", "منابع داده برای ارزیابی کیفیت و اعتمادپذیری تحلیل استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "منابع داده", "منابع داده شامل Survey، بازخورد مشتریان، تست‌های A/B، داده‌های استفاده محصول می‌باشد")]
    public string? DataSources { get; set; }

    /// <summary>
    /// روش تحلیل - روش تحلیل استفاده شده
    /// حداکثر 500 کاراکتر
    /// </summary>
    [DisplayName("روش تحلیل")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "روش تحلیل", "روش تحلیل برای ارزیابی اعتبار علمی و تکرارپذیری تحلیل استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "روش تحلیل", "روش تحلیل شامل PMF/PSF، NPS، CSAT، تحلیل رضایت، یادگیری ماشین می‌باشد")]
    public string? AnalysisMethod { get; set; }

    /// <summary>
    /// تاریخ آخرین به‌روزرسانی - تاریخ آخرین به‌روزرسانی تحلیل
    /// </summary>
    [DisplayName("تاریخ آخرین به‌روزرسانی")]
    [SBVR(SBVRModality.Recommended, "تاریخ آخرین به‌روزرسانی", "تاریخ آخرین به‌روزرسانی برای مدیریت Freshness داده‌ها و تصمیم‌گیری در مورد نیاز به تحلیل مجدد استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تاریخ آخرین به‌روزرسانی", "تاریخ آخرین به‌روزرسانی = زمان اجرای آخرین محاسبه یا تحلیل برای این محصول")]
    public DateTime LastUpdatedDate { get; set; }

    /// <summary>
    /// نسخه مدل - نسخه مدل تحلیل استفاده شده
    /// حداکثر 50 کاراکتر
    /// </summary>
    [DisplayName("نسخه مدل")]
    [MaxLength(50)]
    [SBVR(SBVRModality.Recommended, "نسخه مدل", "نسخه مدل برای ردیابی نسخه‌های مختلف مدل و مدیریت Reproducibility تحلیل‌ها استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نسخه مدل", "نسخه مدل = شناسه نسخه مدل تحلیل که در زمان محاسبه استفاده شده است")]
    public string? ModelVersion { get; set; }

    /// <summary>
    /// داده‌های اضافی - داده‌های اضافی تحلیل در فرمت JSON
    /// حداکثر 2000 کاراکتر
    /// </summary>
    [DisplayName("داده‌های اضافی")]
    [MaxLength(2000)]
    [SBVR(SBVRModality.Recommended, "داده‌های اضافی", "داده‌های اضافی برای ذخیره پارامترهای خاص مدل، جزئیات محاسبات یا نتیجه‌های میانی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "داده‌های اضافی", "داده‌های اضافی = JSON حاوی اطلاعات تکمیلی تحلیل شامل اعداد اعشاری نمرات، فاکتورهای موثر و metadata محاسبه")]
    public string? AdditionalData { get; set; }
}
