namespace Hyper.Domain.Entities.Analytics;

/// <summary>
/// تحلیل بازار - تحلیل جامع بازار، رقابت و فرصت‌های کسب‌وکار
/// این موجودیت شامل تمام شاخص‌های مهم بازاریابی پیشرفته می‌باشد
/// شامل تحلیل TAM/SAM/SOM، تحلیل رقابتی، روند بازار و فرصت‌ها
/// </summary>
[DisplayName("تحلیل بازار")]
public class MarketAnalysis : HyperBaseCoreConfigAuditableEntity<int>
{
    /// <summary>
    /// شناسه اکوسیستم - اکوسیستمی که تحلیل در آن انجام شده است
    /// </summary>
    public int TenantId { get; set; }

    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه محصول - محصول یا خدمت که این تحلیل برای آن انجام شده است (اختیاری)
    /// </summary>
    public int? ProductId { get; set; }

    [DisplayName("محصول")]
    [SBVR(SBVRModality.Permitted, "تحلیل محصول-بازار", "محصول برای تحلیل‌های اختصاصی یک محصول در بازار استفاده می‌شود")]
    public Product? Product { get; set; }

    /// <summary>
    /// عنوان تحلیل - عنوان مشخص کننده تحلیل بازار
    /// حداکثر 100 کاراکتر
    /// </summary>

    [MaxLength(100)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// تاریخ تحلیل - تاریخ انجام تحلیل بازار
    /// </summary>

    public DateTime AnalysisDate { get; set; }

    /// <summary>
    /// دوره تحلیل - دوره زمانی که تحلیل بر اساس آن انجام شده است
    /// </summary>

    public AnalysisPeriod Period { get; set; }

    // ===== TAM/SAM/SOM ANALYSIS =====
    /// <summary>
    /// بازار کل قابل دسترس (TAM) - اندازه کل بازار قابل دسترس
    /// </summary>

    [SBVR(SBVRModality.Calculated, "بازار کل قابل دسترس (TAM)", "TAM = تعداد کل مشتریان بالقوه × میانگین ارزش هر مشتری")]
    [SBVR(SBVRModality.Recommended, "بازار کل قابل دسترس (TAM)", "TAM نشان‌دهنده پتانسیل کل بازار است")]
    public decimal TotalAddressableMarket { get; set; }

    /// <summary>
    /// بازار قابل دسترس خدمت (SAM) - اندازه بازار قابل دسترس برای خدمات
    /// </summary>

    [SBVR(SBVRModality.Calculated, "بازار قابل دسترس خدمت (SAM)", "SAM = TAM × درصد مشتریان قابل دسترس با خدمات فعلی")]
    [SBVR(SBVRModality.Recommended, "بازار قابل دسترس خدمت (SAM)", "SAM نشان‌دهنده بازار قابل دسترس با خدمات فعلی است")]
    public decimal ServiceableAddressableMarket { get; set; }

    /// <summary>
    /// بازار قابل دسترس کسب (SOM) - اندازه بازار قابل دسترس برای کسب
    /// </summary>

    [SBVR(SBVRModality.Calculated, "بازار قابل دسترس کسب (SOM)", "SOM = SAM × درصد سهم بازار قابل کسب")]
    [SBVR(SBVRModality.Recommended, "بازار قابل دسترس کسب (SOM)", "SOM نشان‌دهنده بازار قابل کسب با منابع فعلی است")]
    public decimal ServiceableObtainableMarket { get; set; }

    /// <summary>
    /// سهم بازار فعلی - درصد سهم فعلی در بازار (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "سهم بازار فعلی", "سهم بازار فعلی = (درآمد فعلی / درآمد کل بازار) × 100")]
    [SBVR(SBVRModality.Recommended, "سهم بازار فعلی", "سهم بازار بالای 10% نشان‌دهنده موقعیت قوی در بازار است")]
    public decimal CurrentMarketShare { get; set; }

    /// <summary>
    /// پتانسیل رشد - پتانسیل رشد در بازار (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "پتانسیل رشد", "پتانسیل رشد = (SOM - درآمد فعلی) / درآمد فعلی × 100")]
    [SBVR(SBVRModality.Recommended, "پتانسیل رشد", "پتانسیل رشد بالای 50% نشان‌دهنده فرصت‌های رشد زیاد است")]
    public decimal GrowthPotential { get; set; }

    // ===== COMPETITIVE ANALYSIS =====
    /// <summary>
    /// نقاط تشابه (POP) - نقاط تشابه با رقبا
    /// حداکثر 1000 کاراکتر
    /// </summary>

    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "نقاط تشابه (POP)", "POP شامل ویژگی‌هایی است که برای رقابت ضروری هستند")]
    public string? PointsOfParity { get; set; }

    /// <summary>
    /// نقاط تمایز (POD) - نقاط تمایز از رقبا
    /// حداکثر 1000 کاراکتر
    /// </summary>

    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "نقاط تمایز (POD)", "POD شامل ویژگی‌هایی است که ما را از رقبا متمایز می‌کند")]
    [SBVR(SBVRModality.Recommended, "نقاط تمایز (POD)", "POD قوی باعث مزیت رقابتی پایدار می‌شود")]
    public string? PointsOfDifference { get; set; }

    /// <summary>
    /// نمره رقابتی - نمره کلی قدرت رقابتی (0-100)
    /// </summary>

    [SBVR(SBVRModality.Calculated, "نمره رقابتی", "نمره رقابتی بر اساس مقایسه با رقبا در ابعاد مختلف محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "نمره رقابتی", "نمره رقابتی بالای 70 نشان‌دهنده موقعیت قوی در بازار است")]
    public decimal CompetitiveScore { get; set; }

    // ===== MARKET SIZE ANALYSIS =====
    /// <summary>
    /// اندازه بازار کل - اندازه کل بازار
    /// </summary>
    [DisplayName("اندازه بازار کل")]
    [SBVR(SBVRModality.Recommended, "اندازه بازار کل", "اندازه بازار کل برای ارزیابی پتانسیل کسب‌وکار و اولویت‌بندی بخش‌های بازار استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "اندازه بازار کل", "اندازه بازار کل بر اساس مطالعات بازار، آمار صنعت و تحلیل جمعیت هدف محاسبه می‌شود")]
    public decimal TotalMarketSize { get; set; }

    /// <summary>
    /// اندازه بازار هدف - اندازه بازار هدف
    /// </summary>
    [DisplayName("اندازه بازار هدف")]
    [SBVR(SBVRModality.Recommended, "اندازه بازار هدف", "اندازه بازار هدف برای شناسایی گروه‌های مشتری هدف و طراحی استراتژی بازاریابی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "اندازه بازار هدف", "اندازه بازار هدف بر اساس شخصیت مشتری ایده‌آل و تناسب محصول با نیازهای بازار محاسبه می‌شود")]
    public decimal TargetMarketSize { get; set; }

    /// <summary>
    /// اندازه بازار قابل دسترس - اندازه بازار قابل دسترس
    /// </summary>
    [DisplayName("اندازه بازار قابل دسترس")]
    [SBVR(SBVRModality.Recommended, "اندازه بازار قابل دسترس", "اندازه بازار قابل دسترس برای برنامه‌ریزی کانال‌های توزیع و استراتژی دسترسی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "اندازه بازار قابل دسترس", "اندازه بازار قابل دسترس = TotalMarketSize × درصد دسترسی جغرافیایی و توزیعی")]
    public decimal AccessibleMarketSize { get; set; }

    /// <summary>
    /// اندازه بازار قابل کسب - اندازه بازار قابل کسب
    /// </summary>
    [DisplayName("اندازه بازار قابل کسب")]
    [SBVR(SBVRModality.Recommended, "اندازه بازار قابل کسب", "اندازه بازار قابل کسب برای تعیین اهداف واقعی و منابع مورد نیاز استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "اندازه بازار قابل کسب", "اندازه بازار قابل کسب = AccessibleMarketSize × درصد قابلیت نفوذ با منابع و توانایی فعلی")]
    public decimal ObtainableMarketSize { get; set; }

    // ===== GROWTH ANALYSIS =====
    /// <summary>
    /// نرخ رشد بازار - نرخ رشد بازار (درصد)
    /// </summary>
    [DisplayName("نرخ رشد بازار")]
    [SBVR(SBVRModality.Recommended, "نرخ رشد بازار", "نرخ رشد بازار برای ارزیابی فرصت‌های آینده و برنامه‌ریزی بلندمدت استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ رشد بازار", "نرخ رشد بازار = ((ارزش بازار فعلی - ارزش بازار قبلی) / ارزش بازار قبلی) × 100")]
    public decimal MarketGrowthRate { get; set; }

    /// <summary>
    /// نرخ رشد سالانه - نرخ رشد سالانه بازار (درصد)
    /// </summary>
    [DisplayName("نرخ رشد سالانه")]
    [SBVR(SBVRModality.Recommended, "نرخ رشد سالانه", "نرخ رشد سالانه برای برآورد ظرفیت بازار در سال‌های آینده استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نرخ رشد سالانه", "نرخ رشد سالانه = میانگین هندسی نرخ رشد سه سال اخیر")]
    public decimal AnnualGrowthRate { get; set; }

    /// <summary>
    /// نرخ رشد پیش‌بینی شده - نرخ رشد پیش‌بینی شده بازار (درصد)
    /// </summary>
    [DisplayName("نرخ رشد پیش‌بینی شده")]
    [SBVR(SBVRModality.Recommended, "نرخ رشد پیش‌بینی شده", "نرخ رشد پیش‌بینی شده برای تصمیم‌گیری‌های استراتژیک و تخصیص منابع استفاده می‌شود")]
    [SBVR(SBVRModality.Predicted, "نرخ رشد پیش‌بینی شده", "نرخ رشد پیش‌بینی شده با استفاده از مدل‌های Time Series و روندهای تاریخی محاسبه می‌شود")]
    public decimal PredictedGrowthRate { get; set; }

    // ===== CUSTOMER ANALYSIS =====
    /// <summary>
    /// تعداد مشتریان کل - تعداد کل مشتریان در بازار
    /// </summary>
    [DisplayName("تعداد مشتریان کل")]
    [SBVR(SBVRModality.Recommended, "تعداد مشتریان کل", "تعداد مشتریان کل برای ارزیابی پتانسیل بازار و تخمین ظرفیت استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد مشتریان کل", "تعداد مشتریان کل = مجموع تمام مشتریان فعلی در بازار")]
    public int TotalCustomers { get; set; }

    /// <summary>
    /// تعداد مشتریان هدف - تعداد مشتریان هدف
    /// </summary>
    [DisplayName("تعداد مشتریان هدف")]
    [SBVR(SBVRModality.Recommended, "تعداد مشتریان هدف", "تعداد مشتریان هدف برای تعیین Segment هدف و طراحی کمپین‌های بازاریابی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد مشتریان هدف", "تعداد مشتریان هدف = تعداد مشتریان در Segment هدف بر اساس شخصیت مشتری ایده‌آل")]
    public int TargetCustomers { get; set; }

    /// <summary>
    /// تعداد مشتریان قابل دسترس - تعداد مشتریان قابل دسترس
    /// </summary>
    [DisplayName("تعداد مشتریان قابل دسترس")]
    [SBVR(SBVRModality.Recommended, "تعداد مشتریان قابل دسترس", "تعداد مشتریان قابل دسترس برای برنامه‌ریزی کانال‌های ارتباط و توزیع استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد مشتریان قابل دسترس", "تعداد مشتریان قابل دسترس = TargetCustomers × درصد دسترسی جغرافیایی و توزیعی")]
    public int AccessibleCustomers { get; set; }

    /// <summary>
    /// تعداد مشتریان قابل کسب - تعداد مشتریان قابل کسب
    /// </summary>
    [DisplayName("تعداد مشتریان قابل کسب")]
    [SBVR(SBVRModality.Recommended, "تعداد مشتریان قابل کسب", "تعداد مشتریان قابل کسب برای تعیین اهداف واقعی و ظرفیت تیم فروش استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد مشتریان قابل کسب", "تعداد مشتریان قابل کسب = AccessibleCustomers × نرخ تبدیل × ظرفیت منابع")]
    public int ObtainableCustomers { get; set; }

    // ===== REVENUE ANALYSIS =====
    /// <summary>
    /// درآمد کل بازار - درآمد کل بازار
    /// </summary>
    [DisplayName("درآمد کل بازار")]
    [SBVR(SBVRModality.Recommended, "درآمد کل بازار", "درآمد کل بازار برای ارزیابی فرصت‌های مالی و برنامه‌ریزی استراتژیک استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "درآمد کل بازار", "درآمد کل بازار = TotalMarketSize × میانگین درآمد هر مشتری")]
    public decimal TotalMarketRevenue { get; set; }

    /// <summary>
    /// درآمد بازار هدف - درآمد بازار هدف
    /// </summary>
    [DisplayName("درآمد بازار هدف")]
    [SBVR(SBVRModality.Recommended, "درآمد بازار هدف", "درآمد بازار هدف برای تعیین اهداف مالی و برنامه‌ریزی بودجه استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "درآمد بازار هدف", "درآمد بازار هدف = TargetMarketSize × میانگین درآمد هر مشتری هدف")]
    public decimal TargetMarketRevenue { get; set; }

    /// <summary>
    /// درآمد بازار قابل دسترس - درآمد بازار قابل دسترس
    /// </summary>
    [DisplayName("درآمد بازار قابل دسترس")]
    [SBVR(SBVRModality.Recommended, "درآمد بازار قابل دسترس", "درآمد بازار قابل دسترس برای برآورد درآمد قابل دستیابی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "درآمد بازار قابل دسترس", "درآمد بازار قابل دسترس = AccessibleMarketSize × میانگین درآمد هر مشتری قابل دسترس")]
    public decimal AccessibleMarketRevenue { get; set; }

    /// <summary>
    /// درآمد بازار قابل کسب - درآمد بازار قابل کسب
    /// </summary>
    [DisplayName("درآمد بازار قابل کسب")]
    [SBVR(SBVRModality.Recommended, "درآمد بازار قابل کسب", "درآمد بازار قابل کسب برای تعیین اهداف واقعی فروش استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "درآمد بازار قابل کسب", "درآمد بازار قابل کسب = ObtainableMarketSize × میانگین درآمد هر مشتری قابل کسب")]
    public decimal ObtainableMarketRevenue { get; set; }

    // ===== COMPETITIVE POSITION =====
    /// <summary>
    /// رتبه رقابتی - رتبه در بازار (1=اول، 2=دوم و...)
    /// </summary>
    [DisplayName("رتبه رقابتی")]
    [SBVR(SBVRModality.Recommended, "رتبه رقابتی", "رتبه رقابتی برای ارزیابی موقعیت در بازار و طراحی استراتژی رقابتی استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "رتبه رقابتی", "رتبه رقابتی بر اساس مقایسه سهم بازار با رقبا تعیین می‌شود")]
    public int CompetitiveRank { get; set; }

    /// <summary>
    /// تعداد رقبا - تعداد کل رقبا در بازار
    /// </summary>
    [DisplayName("تعداد رقبا")]
    [SBVR(SBVRModality.Recommended, "تعداد رقبا", "تعداد رقبا برای ارزیابی شدت رقابت و چالش‌های بازار استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تعداد رقبا", "تعداد رقبا بر اساس شناسایی رقبای مستقیم و غیر مستقیم محاسبه می‌شود")]
    public int CompetitorCount { get; set; }

    /// <summary>
    /// سهم رقبا - مجموع سهم رقبا در بازار (درصد)
    /// </summary>
    [DisplayName("سهم رقبا")]
    [SBVR(SBVRModality.Recommended, "سهم رقبا", "سهم رقبا برای ارزیابی شدت رقابت و فرصت‌های باقیمانده در بازار استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "سهم رقبا", "سهم رقبا = مجموع سهم بازار تمام رقبا × 100")]
    public decimal CompetitorShare { get; set; }

    /// <summary>
    /// نمره مزیت رقابتی - نمره مزیت رقابتی (0-100)
    /// </summary>
    [DisplayName("نمره مزیت رقابتی")]
    [SBVR(SBVRModality.Recommended, "نمره مزیت رقابتی", "نمره مزیت رقابتی برای ارزیابی قدرت رقابتی و نقاط تمایز استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "نمره مزیت رقابتی", "نمره مزیت رقابتی بر اساس مقایسه نقاط قوت با رقبا در ابعاد مختلف محاسبه می‌شود")]
    public decimal CompetitiveAdvantageScore { get; set; }

    // ===== MARKET TRENDS =====
    /// <summary>
    /// روند بازار - روند کلی بازار
    /// حداکثر 500 کاراکتر
    /// </summary>
    [DisplayName("روند بازار")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "روند بازار", "روند بازار برای برنامه‌ریزی استراتژیک و انطباق با تغییرات بازار استفاده می‌شود")]
    public string? MarketTrend { get; set; }

    /// <summary>
    /// فرصت‌های بازار - فرصت‌های موجود در بازار
    /// حداکثر 1000 کاراکتر
    /// </summary>
    [DisplayName("فرصت‌های بازار")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "فرصت‌های بازار", "شناسایی فرصت‌ها برای طراحی استراتژی رشد و ورود به بخش‌های جدید استفاده می‌شود")]
    public string? MarketOpportunities { get; set; }

    /// <summary>
    /// تهدیدهای بازار - تهدیدهای موجود در بازار
    /// حداکثر 1000 کاراکتر
    /// </summary>
    [DisplayName("تهدیدهای بازار")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "تهدیدهای بازار", "شناسایی تهدیدها برای آماده‌سازی و کاهش ریسک استفاده می‌شود")]
    public string? MarketThreats { get; set; }

    /// <summary>
    /// نقاط قوت - نقاط قوت در بازار
    /// حداکثر 1000 کاراکتر
    /// </summary>
    [DisplayName("نقاط قوت")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "نقاط قوت", "شناسایی نقاط قوت برای بهره‌برداری از مزیت‌ها و تقویت موقعیت استفاده می‌شود")]
    public string? Strengths { get; set; }

    /// <summary>
    /// نقاط ضعف - نقاط ضعف در بازار
    /// حداکثر 1000 کاراکتر
    /// </summary>
    [DisplayName("نقاط ضعف")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "نقاط ضعف", "شناسایی نقاط ضعف برای بهبود و تقویت موقعیت رقابتی استفاده می‌شود")]
    public string? Weaknesses { get; set; }

    // ===== ANALYSIS QUALITY =====
    /// <summary>
    /// دقت تحلیل - دقت تحلیل بازار (0-100)
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
    [SBVR(SBVRModality.Calculated, "منابع داده", "منابع داده شامل Survey، داده‌های داخلی، رصد بازار، گزارش‌های صنعت می‌باشد")]
    public string? DataSources { get; set; }

    /// <summary>
    /// روش تحلیل - روش تحلیل استفاده شده
    /// حداکثر 500 کاراکتر
    /// </summary>
    [DisplayName("روش تحلیل")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Recommended, "روش تحلیل", "روش تحلیل برای ارزیابی اعتبار علمی و تکرارپذیری تحلیل استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "روش تحلیل", "روش تحلیل شامل SWOT، TAM/SAM/SOM، تحلیل رقابتی، Time Series Analysis می‌باشد")]
    public string? AnalysisMethod { get; set; }

    /// <summary>
    /// تاریخ آخرین به‌روزرسانی - تاریخ آخرین به‌روزرسانی تحلیل
    /// </summary>
    [DisplayName("تاریخ آخرین به‌روزرسانی")]
    [SBVR(SBVRModality.Recommended, "تاریخ آخرین به‌روزرسانی", "تاریخ آخرین به‌روزرسانی برای مدیریت Freshness داده‌ها و تصمیم‌گیری در مورد نیاز به تحلیل مجدد استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تاریخ آخرین به‌روزرسانی", "تاریخ آخرین به‌روزرسانی = زمان اجرای آخرین محاسبه یا تحلیل")]
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

    // ===== MARKET POSITIONING MATRIX =====
    
    /// <summary>
    /// جایگاه در ماتریس قیمت-ارزش - جایگاه محصول در ماتریس قیمت در مقابل ارزش
    /// </summary>
    [DisplayName("جایگاه در ماتریس قیمت-ارزش")]
    [SBVR(SBVRModality.Recommended, "استراتژی قیمت‌گذاری", "جایگاه در ماتریس قیمت-ارزش برای طراحی استراتژی قیمت‌گذاری و تمایز محصول استفاده می‌شود")]
    public ValuePositionMatrix? PositionInValueMatrix { get; set; }

    /// <summary>
    /// جایگاه در ماتریس کیفیت-قیمت - جایگاه محصول در ماتریس کیفیت در مقابل قیمت
    /// </summary>
    [DisplayName("جایگاه در ماتریس کیفیت-قیمت")]
    [SBVR(SBVRModality.Recommended, "استراتژی کیفیت", "جایگاه در ماتریس کیفیت-قیمت برای ارزیابی ارزش محصول و طراحی استراتژی بهبود کیفیت استفاده می‌شود")]
    public QualityPositionMatrix? PositionInQualityMatrix { get; set; }

    /// <summary>
    /// جایگاه در ماتریس BCG - جایگاه محصول در ماتریس رشد-سهم بازار (Boston Consulting Group)
    /// </summary>
    [DisplayName("جایگاه در ماتریس BCG")]
    [SBVR(SBVRModality.Recommended, "استراتژی محصول", "جایگاه در ماتریس BCG برای تصمیم‌گیری در مورد سرمایه‌گذاری و اولویت‌بندی محصولات استفاده می‌شود")]
    public BCGMatrixPosition? PositionInBCGMatrix { get; set; }

    /// <summary>
    /// جایگاه در ماتریس Ansoff - جایگاه محصول در ماتریس رشد بازار-محصول (Ansoff)
    /// </summary>
    [DisplayName("جایگاه در ماتریس Ansoff")]
    [SBVR(SBVRModality.Recommended, "استراتژی رشد", "جایگاه در ماتریس Ansoff برای طراحی استراتژی‌های رشد و ورود به بازار استفاده می‌شود")]
    public AnsoffMatrixPosition? PositionInAnsoffMatrix { get; set; }
}

/// <summary>
/// جایگاه در ماتریس قیمت-ارزش
/// </summary>
public enum ValuePositionMatrix
{
    [Description("قیمت پایین - ارزش پایین")]
    LowPriceLowValue = 1,
    [Description("قیمت پایین - ارزش بالا")]
    LowPriceHighValue = 2,
    [Description("قیمت بالا - ارزش پایین")]
    HighPriceLowValue = 3,
    [Description("قیمت بالا - ارزش بالا")]
    HighPriceHighValue = 4
}

/// <summary>
/// جایگاه در ماتریس کیفیت-قیمت
/// </summary>
public enum QualityPositionMatrix
{
    [Description("کیفیت پایین - قیمت پایین")]
    LowQualityLowPrice = 1,
    [Description("کیفیت پایین - قیمت بالا")]
    LowQualityHighPrice = 2,
    [Description("کیفیت بالا - قیمت پایین")]
    HighQualityLowPrice = 3,
    [Description("کیفیت بالا - قیمت بالا")]
    HighQualityHighPrice = 4
}

/// <summary>
/// جایگاه در ماتریس BCG (Boston Consulting Group)
/// </summary>
public enum BCGMatrixPosition
{
    [Description("سگ (Low Share, Low Growth)")]
    Dog = 1,
    [Description("گاو شیرده (High Share, Low Growth)")]
    CashCow = 2,
    [Description("علامت سوال (Low Share, High Growth)")]
    QuestionMark = 3,
    [Description("ستاره (High Share, High Growth)")]
    Star = 4
}

/// <summary>
/// جایگاه در ماتریس Ansoff
/// </summary>
public enum AnsoffMatrixPosition
{
    [Description("نفوذ بازار - بازار فعلی و محصول فعلی")]
    MarketPenetration = 1,
    [Description("توسعه محصول - بازار فعلی و محصول جدید")]
    ProductDevelopment = 2,
    [Description("توسعه بازار - بازار جدید و محصول فعلی")]
    MarketDevelopment = 3,
    [Description("متنوع‌سازی - بازار جدید و محصول جدید")]
    Diversification = 4
}
