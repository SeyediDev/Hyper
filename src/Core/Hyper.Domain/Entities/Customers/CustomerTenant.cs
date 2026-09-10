namespace Hyper.Domain.Entities.Customers;

/// <summary>
/// رابطه مشتری با اکوسیستم‌ها
/// یک مشتری می‌تواند به چندین اکوسیستم تعلق داشته باشد
/// </summary>
[DisplayName("مشتری اکوسیستم")]
[SBVR(SBVRModality.Obligatory, "چند اکوسیستمی", "هر رابطه مشتری-اکوسیستم باید برای جداسازی داده‌ها و مدیریت دسترسی قابل شناسایی باشد")]
[EntityIndex(nameof(IsDeleted), nameof(CustomerId))]
[EntityIndex(nameof(IsDeleted), nameof(TenantId))]
[EntityIndex(nameof(IsDeleted), nameof(CustomerId), nameof(TenantId))]
public class CustomerTenant : HyperBaseCoreAuditableEntity<int>
{
    [DisplayName("مشتری")]
    public int CustomerId { get; set; }

    [DisplayName("مشتری")]
    public Customer Customer { get; set; } = null!;

    [DisplayName("اکوسیستم")]
    public int TenantId { get; set; }

    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

	[DisplayName("نام")]
	[InDisplayString]
	[MaxLength(61)]
	[SBVR(SBVRModality.Recommended, "شناسایی مشتری", "نام برای شخصی‌سازی پیام‌ها و تحلیل جمعیت‌شناختی ضروری است")]
	public string? FirstName { get; set; }

	[DisplayName("نام خانوادگی")]
	[InDisplayString]
	[MaxLength(61)]
	[SBVR(SBVRModality.Recommended, "شناسایی مشتری", "نام خانوادگی برای تکمیل پروفایل و تحلیل خانواده‌ها مهم است")]
	public string? LastName { get; set; }
	
    public int? CountryId { get; set; }
    [DisplayName("کشور")]
    public Country? Country { get; set; }

    public int? ProvinceId { get; set; }
    [DisplayName("استان")]
    public Province? Province { get; set; }

    public int? CityId { get; set; }
    [DisplayName("شهر")]
    public City? City { get; set; }

    /// <summary>
    /// تاریخ شروع عضویت مشتری در اکوسیستم
    /// </summary>
    [DisplayName("تاریخ عضویت")]
    [SBVR(SBVRModality.Recommended, "چند اکوسیستمی", "تاریخ عضویت برای تحلیل رشد اعضا و مدیریت چرخه حیات مشتری استفاده می‌شود")]
    public DateTime? JoinDate { get; set; }

    [DisplayName("تاریخ عضویت")]
    [SBVR(SBVRModality.Recommended, "چند اکوسیستمی", "تاریخ عضویت برای تحلیل رشد اعضا و مدیریت چرخه حیات مشتری استفاده می‌شود")]
    [Formula($"IsNull({nameof(JoinDate)},{nameof(CreateDate)})")]
    public DateTime? JoinToTenantDate { get; set; }

    /// <summary>
    /// تاریخ پایان عضویت مشتری در اکوسیستم (در صورت خاتمه)
    /// </summary>
    [DisplayName("تاریخ خاتمه عضویت")]
    [SBVR(SBVRModality.Optional, "چند اکوسیستمی", "تاریخ خاتمه عضویت برای کنترل روابط فعال و تحلیل ریزش استفاده می‌شود")]
    public DateTime? LeaveDate { get; set; }

    // =====================================================
    // RFM Analysis Fields
    // =====================================================

    /// <summary>
    /// Recency - تاریخ آخرین خرید/تعامل
    /// </summary>
    [DisplayName("آخرین تعامل")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "تاریخ آخرین تعامل برای محاسبه Recency Score و تحلیل رفتار مشتری استفاده می‌شود")]
    public DateTime? LastInteractionDate { get; set; }

    /// <summary>
    /// Recency Score - نمره تازگی (1-5)
    /// </summary>
    [DisplayName("نمره تازگی")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "نمره تازگی برای دسته‌بندی مشتریان بر اساس میزان اخیر بودن تعامل استفاده می‌شود")]
    public int? RecencyScore { get; set; }

    /// <summary>
    /// Frequency - تعداد تراکنش‌ها/تعاملات
    /// </summary>
    [DisplayName("تعداد تعاملات")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "تعداد تعاملات برای محاسبه Frequency Score و تحلیل میزان فعالیت مشتری استفاده می‌شود")]
    public int? TotalInteractions { get; set; }

    /// <summary>
    /// Frequency Score - نمره تکرار (1-5)
    /// </summary>
    [DisplayName("نمره تکرار")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "نمره تکرار برای دسته‌بندی مشتریان بر اساس میزان تکرار تعاملات استفاده می‌شود")]
    public int? FrequencyScore { get; set; }

    /// <summary>
    /// Monetary - ارزش کل تراکنش‌ها
    /// </summary>
    [DisplayName("ارزش کل تراکنش‌ها")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "ارزش کل تراکنش‌ها برای محاسبه Monetary Score و تحلیل ارزش مشتری استفاده می‌شود")]
    public decimal? TotalTransactionValue { get; set; }

    /// <summary>
    /// Monetary Score - نمره ارزش (1-5)
    /// </summary>
    [DisplayName("نمره ارزش")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "نمره ارزش برای دسته‌بندی مشتریان بر اساس میزان ارزش مالی استفاده می‌شود")]
    public int? MonetaryScore { get; set; }

    /// <summary>
    /// RFM Segment - دسته RFM (e.g., "Champions", "Loyal", "At Risk")
    /// </summary>
    [DisplayName("دسته RFM")]
    [SBVR(SBVRModality.Calculated, "تحلیل RFM", "دسته RFM برای دسته‌بندی مشتریان و هدف‌گذاری کمپین‌ها استفاده می‌شود")]
    public RFMSegment? RfmSegment { get; set; }

    // =====================================================
    // Customer Lifetime Value (CLV) Fields
    // =====================================================

    /// <summary>
    /// Customer Lifetime Value - ارزش طول عمر مشتری
    /// </summary>
    [DisplayName("ارزش طول عمر مشتری (CLV)")]
    [SBVR(SBVRModality.Calculated, "تحلیل CLV", "ارزش طول عمر مشتری برای تحلیل سودآوری و اولویت‌بندی مشتریان استفاده می‌شود")]
    public decimal? CustomerLifetimeValue { get; set; }

    /// <summary>
    /// Average Order Value - میانگین ارزش سفارش
    /// </summary>
    [DisplayName("میانگین ارزش سفارش")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "میانگین ارزش سفارش برای تحلیل رفتار خرید و پیش‌بینی درآمد استفاده می‌شود")]
    public decimal? AverageOrderValue { get; set; }

    /// <summary>
    /// Purchase Frequency - تعداد خریدها در واحد زمان
    /// </summary>
    [DisplayName("فرکانس خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "فرکانس خرید برای پیش‌بینی رفتار آینده و برنامه‌ریزی کمپین‌ها استفاده می‌شود")]
    public decimal? PurchaseFrequency { get; set; }

    // =====================================================
    // Advanced Customer Value Metrics
    // =====================================================

    /// <summary>
    /// Customer Acquisition Cost - هزینه جذب مشتری (CAC)
    /// </summary>
    [DisplayName("هزینه جذب مشتری (CAC)")]
    [SBVR(SBVRModality.Calculated, "تحلیل بهره‌وری", "CAC = هزینه‌های بازاریابی / تعداد مشتریان جدید جذب شده")]
    public decimal? CustomerAcquisitionCost { get; set; }

    /// <summary>
    /// LTV:CAC Ratio - نسبت ارزش طول عمر به هزینه جذب
    /// </summary>
    [DisplayName("نسبت LTV:CAC")]
    [SBVR(SBVRModality.Calculated, "تحلیل سودآوری", "نسبت LTV:CAC برای ارزیابی بازگشت سرمایه جذب مشتری استفاده می‌شود")]
    public decimal? LtvToCacRatio { get; set; }

    /// <summary>
    /// Payback Period - دوره بازگشت سرمایه (روز)
    /// </summary>
    [DisplayName("دوره بازگشت سرمایه (روز)")]
    [SBVR(SBVRModality.Calculated, "تحلیل نقدینگی", "دوره بازگشت = CAC / درآمد ماهانه حاصل از مشتری")]
    public int? PaybackPeriodDays { get; set; }

    /// <summary>
    /// Customer Profit Margin - حاشیه سود مشتری (%)
    /// </summary>
    [DisplayName("حاشیه سود مشتری (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل سودآوری", "حاشیه سود = ((CLV - CAC) / CLV) × 100")]
    public decimal? CustomerProfitMargin { get; set; }

    /// <summary>
    /// Referral Value - ارزش ارجاع
    /// </summary>
    [DisplayName("ارزش ارجاع")]
    [SBVR(SBVRModality.Calculated, "تحلیل ارجاع", "ارزش مالی مشتریان جدیدی که توسط این مشتری معرفی شده‌اند")]
    public decimal? ReferralValue { get; set; }

    /// <summary>
    /// Retention Rate - نرخ حفظ مشتری (%)
    /// </summary>
    [DisplayName("نرخ حفظ مشتری (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل حفظ", "نرخ حفظ برای ارزیابی موفقیت استراتژی‌های نگهداشت مشتری استفاده می‌شود")]
    public decimal? RetentionRate { get; set; }

    /// <summary>
    /// Repeat Purchase Rate - نرخ خرید مجدد (%)
    /// </summary>
    [DisplayName("نرخ خرید مجدد (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل وفاداری", "نرخ خرید مجدد نشان می‌دهد چند درصد مشتریان بیش از یک بار خرید کرده‌اند")]
    public decimal? RepeatPurchaseRate { get; set; }

    /// <summary>
    /// Average Time Between Purchases - میانگین زمان بین خریدها (روز)
    /// </summary>
    [DisplayName("میانگین زمان بین خریدها (روز)")]
    [SBVR(SBVRModality.Calculated, "تحلیل رفتار خرید", "میانگین زمان بین خریدها برای پیش‌بینی خرید بعدی استفاده می‌شود")]
    public int? AverageTimeBetweenPurchasesDays { get; set; }

    // =====================================================
    // Engagement & Loyalty Fields
    // =====================================================

    /// <summary>
    /// Engagement Score - نمره تعامل مشتری (0-100)
    /// </summary>
    [DisplayName("نمره تعامل")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "نمره تعامل برای سنجش میزان مشارکت مشتری در فعالیت‌های باشگاه استفاده می‌شود")]
    public decimal? EngagementScore { get; set; }

    /// <summary>
    /// Loyalty Score - نمره وفاداری (0-100)
    /// </summary>
    [DisplayName("نمره وفاداری")]
    [SBVR(SBVRModality.Calculated, "تحلیل وفاداری", "نمره وفاداری برای شناسایی مشتریان وفادار و برنامه‌ریزی استراتژی‌های حفظ مشتری استفاده می‌شود")]
    public decimal? LoyaltyScore { get; set; }

    /// <summary>
    /// Churn Risk Score - نمره احتمال ریزش (0-100)
    /// </summary>
    [DisplayName("نمره احتمال ریزش")]
    [SBVR(SBVRModality.Calculated, "تحلیل ریزش", "نمره احتمال ریزش برای شناسایی مشتریان در معرض خطر ریزش و اجرای کمپین‌های حفظ مشتری استفاده می‌شود")]
    public decimal? ChurnRiskScore { get; set; }

    /// <summary>
    /// NPS Score - Net Promoter Score (-100 to 100)
    /// </summary>
    [DisplayName("نمره NPS")]
    [SBVR(SBVRModality.Recommended, "تحلیل رضایت", "نمره NPS برای سنجش احتمال توصیه مشتری به دیگران استفاده می‌شود")]
    public int? NpsScore { get; set; }

    /// <summary>
    /// Satisfaction Score - نمره رضایت (0-100)
    /// </summary>
    [DisplayName("نمره رضایت")]
    [SBVR(SBVRModality.Recommended, "تحلیل رضایت", "نمره رضایت برای سنجش میزان رضایت کلی مشتری استفاده می‌شود")]
    public decimal? SatisfactionScore { get; set; }

    // =====================================================
    // Status & Activity Fields
    // =====================================================

    /// <summary>
    /// Is Active Customer - آیا مشتری فعال است
    /// </summary>
    [DisplayName("مشتری فعال")]
    [SBVR(SBVRModality.Calculated, "وضعیت فعالیت", "وضعیت فعالیت مشتری برای فیلتر کردن و تحلیل مشتریان فعال استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// تاریخ اولین خرید/تعامل
    /// </summary>
    [DisplayName("تاریخ اولین تعامل")]
    [SBVR(SBVRModality.Calculated, "تحلیل چرخه حیات", "تاریخ اولین تعامل برای محاسبه طول عمر مشتری و تحلیل روند رشد استفاده می‌شود")]
    public DateTime? FirstInteractionDate { get; set; }

    /// <summary>
    /// Days Since Last Interaction - تعداد روز از آخرین تعامل
    /// </summary>
    [DisplayName("روز از آخرین تعامل")]
    [SBVR(SBVRModality.Calculated, "تحلیل فعالیت", "تعداد روز از آخرین تعامل برای شناسایی مشتریان غیرفعال و برنامه‌ریزی کمپین‌های فعال‌سازی مجدد استفاده می‌شود")]
    public int? DaysSinceLastInteraction { get; set; }

    /// <summary>
    /// Total Points Earned - مجموع امتیازات کسب شده
    /// </summary>
    [DisplayName("مجموع امتیازات کسب شده")]
    [SBVR(SBVRModality.Calculated, "تحلیل امتیازات", "مجموع امتیازات کسب شده برای تحلیل میزان مشارکت در برنامه وفاداری استفاده می‌شود")]
    public long? TotalPointsEarned { get; set; }

    /// <summary>
    /// Total Points Redeemed - مجموع امتیازات استفاده شده
    /// </summary>
    [DisplayName("مجموع امتیازات استفاده شده")]
    [SBVR(SBVRModality.Calculated, "تحلیل امتیازات", "مجموع امتیازات استفاده شده برای تحلیل میزان استفاده از پاداش‌ها استفاده می‌شود")]
    public long? TotalPointsRedeemed { get; set; }

    /// <summary>
    /// Current Points Balance - موجودی فعلی امتیازات
    /// </summary>
    [DisplayName("موجودی فعلی امتیازات")]
    [SBVR(SBVRModality.Calculated, "تحلیل امتیازات", "موجودی فعلی امتیازات برای نمایش به مشتری و تحلیل ارزش باقیمانده استفاده می‌شود")]
    public long? CurrentPointsBalance { get; set; }

    // =====================================================
    // Navigation Properties
    // =====================================================

    /// <summary>
    /// گیرندگان پویش‌های مرتبط با این مشتری
    /// </summary>
    [DisplayName("گیرندگان پویش")]
    public ICollection<PromotionRecipient> PromotionRecipients { get; set; } = [];
}