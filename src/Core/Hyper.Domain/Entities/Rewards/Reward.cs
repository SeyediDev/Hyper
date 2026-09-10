namespace Hyper.Domain.Entities.Rewards;

/// <summary>
/// پاداش - موجودیت اصلی برای مدیریت پاداش‌های سیستم امتیازدهی
/// پاداش‌ها محصولاتی هستند که مشتریان می‌توانند با استفاده از امتیازات خود خریداری کنند
/// </summary>
[DisplayName("پاداش")]
[SBVR(SBVRModality.Obligatory, "مدیریت پاداش‌ها", "هر پاداش باید برای فروش امتیازی، تولید دارایی و تحلیل پاداش‌ها قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "مدیریت پاداش‌ها", "پاداش‌ها باید برای تحلیل اثربخشی، محاسبه ROI و بهینه‌سازی کاتالوگ سازماندهی شوند")]
public class Reward : HyperBaseCoreAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر پاداش باید به یک اکوسیستم مشخص تعلق داشته باشد تا از تداخل داده‌ها جلوگیری شود")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// شناسه جامعه مشتریان
    /// </summary>
    [DisplayName("شناسه جامعه مشتریان")]
    [SBVR(SBVRModality.Permitted, "دسترسی محدود", "جامعه مشتریان برای محدود کردن دسترسی پاداش به گروه‌های خاص مشتریان استفاده می‌شود")]
    public int? CustomerSegmentId { get; set; }

    [DisplayName("جامعه مشتریان")]
    [SBVR(SBVRModality.Permitted, "دسترسی محدود", "جامعه مشتریان برای محدود کردن دسترسی پاداش به گروه‌های خاص مشتریان استفاده می‌شود")]
    public CustomerSegment? CustomerSegment { get; set; }

    [DisplayName("عنوان پاداش")]
    [InDisplayString]
    [MaxLength(41)]
    [SBVR(SBVRModality.Obligatory, "شناسایی پاداش", "عنوان پاداش باید برای نمایش در کاتالوگ، فاکتورها و گزارش‌ها واضح و قابل فهم باشد")]
    [SBVR(SBVRModality.Recommended, "بازاریابی پاداش‌ها", "عنوان پاداش باید جذاب و واضح باشد تا مشتریان را به خرید ترغیب کند")]
    public string Title { get; set; } = null!;

    [OldDbMap("CategoryId")]
    public int RewardCategoryId { get; set; }
    [DisplayName("دسته‌بندی پاداش")]
    [SBVR(SBVRModality.Obligatory, "دسته‌بندی پاداش‌ها", "هر پاداش باید در یک دسته‌بندی قرار گیرد تا تحلیل فروش و مدیریت موجودی امکان‌پذیر باشد")]
    public RewardCategory RewardCategory { get; set; } = null!;

    public int MerchantId { get; set; }
    [DisplayName("ارائه‌دهنده پاداش")]
    [SBVR(SBVRModality.Obligatory, "مدیریت ارائه‌دهندگان", "هر پاداش باید یک ارائه‌دهنده مشخص داشته باشد تا محاسبه کمیسیون و مدیریت روابط امکان‌پذیر باشد")]
    public RewardMerchant Merchant { get; set; } = null!;

    public int? PointLevelId { get; set; }
    [DisplayName("سطح امتیاز پاداش")]
    [SBVR(SBVRModality.Prohibited, "انعطاف‌پذیری امتیازدهی", "پاداش‌ها نباید مستقیماً به سطح امتیاز خاصی وابسته باشند تا قوانین امتیازدهی انعطاف‌پذیر باقی بمانند")]
    public PointLevel? PointLevel { get; set; } = null!;

    /// <summary>
    /// کنترل دارایی - تعیین می‌کند که آیا پاداش کنترل دارایی دارد یا خیر
    /// </summary>
    [DisplayName("کنترل دارایی")]
    [SBVR(SBVRModality.Permitted, "مدیریت دارایی‌ها", "کنترل دارایی تعیین می‌کند که آیا پاداش نیاز به تولید و تخصیص دارایی‌های فیزیکی یا دیجیتالی دارد")]
    [SBVR(SBVRModality.Obligatory, "مدیریت موجودی فیزیکی", "کنترل دارایی برای پاداش‌های فیزیکی باید فعال باشد تا از فروش بیش از حد موجودی جلوگیری شود")]
    [SBVR(SBVRModality.Recommended, "کارایی سیستم", "کنترل دارایی برای پاداش‌های دیجیتالی یا پاداش‌های نامحدود می‌تواند غیرفعال باشد")]
    public bool? ControlAsset { get; set; }

    /// <summary>
    /// فقط برای قرعه‌کشی - تعیین می‌کند که آیا این پاداش فقط برای قرعه‌کشی است
    /// اگر true باشد، این پاداش در کاتالوگ باشگاه (Hyper) نمایش داده نمی‌شود
    /// </summary>
    [DisplayName("فقط برای قرعه‌کشی")]
    [SBVR(SBVRModality.Permitted, "مدیریت پاداش‌های قرعه‌کشی", "فقط برای قرعه‌کشی برای تعریف پاداش‌های ویژه که فقط از طریق قرعه‌کشی قابل دسترسی هستند")]
    [SBVR(SBVRModality.Recommended, "مدیریت کاتالوگ", "پاداش‌های قرعه‌کشی باید از کاتالوگ باشگاه جدا نگه داشته شوند تا تجربه کاربری واضح باشد")]
    public bool IsLotteryOnly { get; set; } = false;

    /// <summary>
    /// ارزش پاداش - ارزش امتیازی پاداش
    /// </summary>
    [DisplayName("ارزش پاداش")]
    [SBVR(SBVRModality.Obligatory, "محاسبه قیمت", "ارزش پاداش برای تعیین قیمت امتیازی در درون یک نوع امتیاز ضروری است")]
    [SBVR(SBVRModality.Calculated, "تعادل عرضه و تقاضا", "ارزش پاداش بر اساس عرضه، تقاضا، ارزش واقعی و استراتژی قیمت‌گذاری محاسبه می‌شود")]
    [SBVR(SBVRModality.Recommended, "تحلیل سودآوری", "ارزش پاداش برای تحلیل ROI، محاسبه حاشیه سود و بهینه‌سازی کاتالوگ استفاده می‌شود")]
    public long Value { get; set; }

    /// <summary>
    /// تعداد موجودی
    /// </summary>
    [DisplayName("تعداد موجودی")]
    [SBVR(SBVRModality.Calculated, "مدیریت موجودی", "تعداد موجودی برای کنترل فروش، جلوگیری از فروش بیش از حد و مدیریت عرضه استفاده می‌شود")]
    public int Quantity { get; set; }

    /// <summary>
    //    مستندات کاربری: فرمت تمپلیت‌ نویسی سریال
    //قانون کلی:
    //هر متنی که inside brackets {}
    //نباشد، به صورت ثابت و عیناً نمایش داده می‌شود.
    //جداکننده‌ها(like -, _, etc.) به صورت عادی نمایش داده می‌شوند.
    //از کاراکترهای ویژه inside brackets { }
    //برای تولید بخش‌های پویا استفاده کنید.
    //کاراکترهای ویژه و معانی آنها:
    //Pattern
    //Meaning
    //مثال → خروجی
    //{#n}
    //n عدد تصادفی	
    //{#5} → 42981
    //{@n}
    //n حرف بزرگ لاتین تصادفی
    //{@3} → XKD
    //{&n}
    //n حرف کوچک لاتین تصادفی
    //{&4} → abcd
    //{*n}
    //n کاراکتر الفانومریک تصادفی
    //{*6} → aB3fG7
    //{!}
    //شمارنده - عدد منحصر به فرد و پشت سر هم
    //{!} → 5
    //مثال‌های ترکیبی:
    //هدف تمپلیت  خروجی مثال
    //کد تخفیف    SAVE20-{#4}-{@1}{#1}{@1}{#1}	SAVE20-4291-R2R5
    //شماره سریال SN-{*12}	SN-aB3fG7tY12Zq
    //کد عضویت MEM-{@2}-{!}	MEM - XK - 15
    //کوپن هدیه	GIFT-{&5}-{#2}	GIFT-abcde-42
    //لایسنس نرم‌افزار    {#4}-{#4}-{#4}-{#4}	1234-5678-9012-3456
    /// </summary>
    [DisplayName("قالب سریال")]
    [MaxLength(61)]
    [SBVR(SBVRModality.Permitted, "Serial Template Format",
@"### General Rules:

Any text **outside brackets** `{}` will be displayed as-is (literal text).

Separators (like `-`, `_`, etc.) are displayed normally.

Use special characters **inside brackets** `{ }` to generate dynamic parts.

### Special Characters:

| Pattern | Meaning                     | Example | Output |
|---------|-----------------------------|---------|--------|
| `{#n}`  | n random digits             | `{#5}`  | 42981  |
| `{@n}`  | n random uppercase letters  | `{@3}`  | XKD    |
| `{&n}`  | n random lowercase letters  | `{&4}`  | abcd   |
| `{*n}`  | n random alphanumeric chars | `{*6}`  | aB3fG7 |
| `{!}`   | Unique sequential counter   | `{!}`   | 5      |

### Usage Examples:

| Purpose          | Template                 | Sample Output           |
|------------------|--------------------------|-------------------------|
| Discount Code    | `SAVE20-{#4}-{@1}{#1}`   | SAVE20-4291-R2          |
| Serial Number    | `SN-{*12}`               | SN-aB3fG7tY12Zq         |
| Membership ID    | `MEM-{@2}-{!}`           | MEM-XK-15               |
| Gift Coupon      | `GIFT-{&5}-{#2}`         | GIFT-abcde-42           |
| Software License | `{#4}-{#4}-{#4}-{#4}`    | 1234-5678-9012-3456     |
")]
    [SBVR(SBVRModality.Recommended, "مدیریت یکتایی", "قالب سریال برای شناسایی یکتا هر پاداش فروخته شده و جلوگیری از تقلب استفاده می‌شود")]
    [SBVR(SBVRModality.Calculated, "تولید خودکار کد", "سیستم به صورت خودکار کدهای سریال را بر اساس قالب تعریف شده تولید می‌کند")]
    [SBVR(SBVRModality.Permitted, "تولید سریال", "قالب سریال برای تولید کدهای یکتا برای دارایی‌های دیجیتالی و فیزیکی استفاده می‌شود")]
    public string? SerialFormat { get; set; }

    /// <summary>
    /// قابل مشاهده
    /// </summary>
    [DisplayName("قابل مشاهده")]
    [SBVR(SBVRModality.Recommended, "نمایش پاداش‌ها", "قابل مشاهده تعیین می‌کند که آیا پاداش در کاتالوگ و فروشگاه آنلاین نمایش داده شود")]
    public bool? Visible { get; set; }

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    [DisplayName("ترتیب نمایش")]
    [SBVR(SBVRModality.Permitted, "مدیریت نمایش", "ترتیب نمایش برای مرتب‌سازی و اولویت‌بندی پاداش‌ها در سفارشات استفاده می‌شود")]
    public int? OrderId { get; set; }

    public int? PictureId { get; set; }

    /// <summary>
    /// تصویر پاداش
    /// </summary>
    [DisplayName("تصویر پاداش")]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "تصویر پاداش برای بهبود تجربه کاربری و افزایش نرخ تبدیل در فروشگاه آنلاین استفاده می‌شود")]
    public Document? Picture { get; set; }

    // =====================================================
    // Analytics & Performance Fields
    // =====================================================

    /// <summary>
    /// تعداد کل فروش - تعداد دفعات خرید این پاداش
    /// </summary>
    [DisplayName("تعداد کل فروش")]
    [SBVR(SBVRModality.Calculated, "تحلیل فروش", "تعداد کل فروش برای تحلیل محبوبیت پاداش و بهینه‌سازی کاتالوگ استفاده می‌شود")]
    public int? TotalSales { get; set; }

    /// <summary>
    /// تعداد بازدیدها - تعداد دفعات مشاهده این پاداش
    /// </summary>
    [DisplayName("تعداد بازدیدها")]
    [SBVR(SBVRModality.Calculated, "تحلیل تعامل", "تعداد بازدیدها برای محاسبه نرخ تبدیل و تحلیل علاقه مشتریان استفاده می‌شود")]
    public int? TotalViews { get; set; }

    /// <summary>
    /// نرخ تبدیل - درصد تبدیل بازدید به خرید
    /// </summary>
    [DisplayName("نرخ تبدیل (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل عملکرد", "نرخ تبدیل = (تعداد فروش / تعداد بازدید) × 100")]
    public decimal? ConversionRate { get; set; }

    /// <summary>
    /// میانگین امتیاز رضایت - میانگین امتیاز رضایت مشتریان از این پاداش
    /// </summary>
    [DisplayName("میانگین امتیاز رضایت")]
    [SBVR(SBVRModality.Calculated, "تحلیل کیفیت", "میانگین امتیاز رضایت برای ارزیابی کیفیت پاداش و بهبود مستمر استفاده می‌شود")]
    public decimal? AverageRating { get; set; }

    /// <summary>
    /// تعداد نظرات
    /// </summary>
    [DisplayName("تعداد نظرات")]
    [SBVR(SBVRModality.Calculated, "تحلیل بازخورد", "تعداد نظرات برای سنجش میزان تعامل و علاقه مشتریان استفاده می‌شود")]
    public int? ReviewCount { get; set; }

    /// <summary>
    /// تاریخ آخرین خرید
    /// </summary>
    [DisplayName("تاریخ آخرین خرید")]
    [SBVR(SBVRModality.Calculated, "تحلیل فروش", "تاریخ آخرین خرید برای شناسایی پاداش‌های فعال و غیرفعال استفاده می‌شود")]
    public DateTime? LastPurchaseDate { get; set; }

    /// <summary>
    /// محبوبیت - نمره محبوبیت بر اساس فروش، بازدید و رضایت (0-100)
    /// </summary>
    [DisplayName("نمره محبوبیت")]
    [SBVR(SBVRModality.Calculated, "رتبه‌بندی", "نمره محبوبیت برای رتبه‌بندی و اولویت‌بندی پاداش‌ها در کاتالوگ استفاده می‌شود")]
    public decimal? PopularityScore { get; set; }

    /// <summary>
    /// وضعیت موجودی - In Stock, Out of Stock, Low Stock
    /// </summary>
    [DisplayName("وضعیت موجودی")]
    [MaxLength(20)]
    [SBVR(SBVRModality.Calculated, "مدیریت موجودی", "وضعیت موجودی برای مدیریت عرضه و نمایش به مشتریان استفاده می‌شود")]
    public string? StockStatus { get; set; }

    /// <summary>
    /// آستانه کمبود موجودی
    /// </summary>
    [DisplayName("آستانه کمبود")]
    [SBVR(SBVRModality.Recommended, "هشدار موجودی", "آستانه کمبود برای ارسال هشدار و سفارش مجدد استفاده می‌شود")]
    public int? LowStockThreshold { get; set; }

    /// <summary>
    /// هزینه واقعی پاداش (برای محاسبه سود)
    /// </summary>
    [DisplayName("هزینه واقعی")]
    [SBVR(SBVRModality.Recommended, "تحلیل سودآوری", "هزینه واقعی برای محاسبه حاشیه سود و تحلیل سودآوری پاداش استفاده می‌شود")]
    public decimal? ActualCost { get; set; }

    /// <summary>
    /// حاشیه سود - درصد سود پاداش
    /// </summary>
    [DisplayName("حاشیه سود (%)")]
    [SBVR(SBVRModality.Calculated, "تحلیل مالی", "حاشیه سود = ((ارزش - هزینه) / ارزش) × 100")]
    public decimal? ProfitMargin { get; set; }

    // Navigation Properties
    public ICollection<RewardCost> Costs { get; set; } = [];
}
