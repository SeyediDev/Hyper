namespace Hyper.Domain.Entities.Tenants;

/// <summary>
/// ویژگی اکوسیستم- ذخیره مقادیر پیکربندی اکوسیستم برای استفاده در شرط‌ها و فرمول‌ها
/// 
/// این موجودیت برای ذخیره مقادیر پیکربندی اکوسیستم استفاده می‌شود که می‌توانند در شرط‌های پویش استفاده شوند.
/// 
/// مثال‌های استفاده:
/// - RegularAge: سن قانونی (برای شرط: Customer.Age > Tenant.RegularAge)
/// - AverageCost: میانگین هزینه (برای شرط: Event.Cost > Tenant.AverageCost)
/// - MinPurchaseAmount: حداقل مبلغ خرید (برای شرط: Event.Amount >= Tenant.MinPurchaseAmount)
/// 
/// این مقادیر در فرمول‌های شرط با پیشوند Tenant.* قابل دسترسی هستند.
/// </summary>
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area))]
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area), nameof(Key))]
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area), nameof(SegmentId))]
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area), nameof(ProductCategoryId))]
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area), nameof(ProductId))]
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area), nameof(EventTypeId))]
[EntityIndex(nameof(IsDeleted), nameof(IsActive), nameof(TenantId), nameof(Area), nameof(ChannelId))]
[SBVR(SBVRModality.Obligatory, "ماموریت ویژگی اکوسیستم",
    "ویژگی اکوسیستم برای ذخیره مقادیر پیکربندی اکوسیستم استفاده می‌شود. " +
    "این مقادیر می‌توانند در شرط‌های پویش و فرمول‌ها با پیشوند ناحیه کاربرد استفاده شوند. مانند Tenant.RegularAge" +
    "هر مقدار یک کلید یکتا دارد و می‌تواند از نوع عددی، متنی، تاریخ یا بولی باشد.")]
[DisplayName("ویژگی")]
public class TenantAttribute : HyperBaseCoreConfigAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "رابطه ویژگی-اکوسیستم", "اکوسیستمی که این ویژگی به آن تعلق دارد")]
    public Tenant Tenant { get; set; } = null!;

    [DisplayName("ناحیه کاربرد")]
    [SBVR(SBVRModality.Obligatory, "ناحیه کاربرد", "ناحیه کاربرد تعیین می کند این ویژگی در کدام ناحیه به عنوان ویژگی ثابت برای همه رکوردهای آن ناحیه در نظر گرفته می شود")]
    [SBVR(SBVRModality.Obligatory, "ناحیه کاربرد", "اگر اکوسیستم باشد این ویژگی برای این اکوسیستم تعریف می شود")]
    [SBVR(SBVRModality.Obligatory, "ناحیه کاربرد", "اگر بقیه ناحیه‌ها باشد این ویژگی برای همه آن ناحیه در صورت نال بودن مقدار رکورد مربوط به آن در نظر گرفته می شود")]
    public AttributeArea Area { get; set; } = AttributeArea.Tenant;

    [DisplayName("کاربرد ویژگی برای مشتری")]
    public AttributeCustomerUsage CustomerUsage { get; set; }

    /// <summary>
    /// کلید ویژگی - یکتا در هر اکوسیستم
    /// مثال: "RegularAge", "AverageCost", "MinPurchaseAmount"
    /// </summary>
    [DisplayName("کلید")]
    [InDisplayString]
    [Required]
    [MaxLength(100)]
    [SBVR(SBVRModality.Obligatory, "شناسایی ویژگی",
        "کلید ویژگی یک شناسه یکتا است که برای دسترسی به مقدار استفاده می‌شود. " +
        "این کلید در فرمول‌ها با پیشوند Area.* قابل دسترسی است. " +
        "مثال: اگر کلید 'RegularAge' باشد، در فرمول به صورت Tenant.RegularAge استفاده می‌شود. " +
        "کلید باید یکتا باشد و نمی‌تواند در یک اکوسیستم تکراری باشد.")]
    public string Key { get; set; } = null!;

    [DisplayName("عبارت فرمول‌نویسی")]
    [Formula($"Concat(" +
        $"Switch(Area,'-'" +
        ",{0},'{1}',{2},'{3}'" +
        ",{4},'{5}',{6},'{7}'" +
        ",{8},'{9}'" +
        ")," +
        "'.',Key)"
        , (int)AttributeArea.Segment, AttributeArea.Segment
        , (int)AttributeArea.Product, AttributeArea.Product
        , (int)AttributeArea.Channel, AttributeArea.Channel
        , (int)AttributeArea.Event, AttributeArea.Event
        , (int)AttributeArea.Tenant, AttributeArea.Tenant
        )]
    public string? FormulaTerm { get; set; }
    [DisplayName("عبارت فرمول‌نویسی مشتری")]
    [Formula($"Concat('Customer'," +
        $"Switch(Area,''" +
        ",{0},'{1}',{2},'{3}'" +
        ",{4},'{5}',{6},'{7}'" +
        ")," +
        "'.',Key)"
        , (int)AttributeArea.Segment, AttributeArea.Segment
        , (int)AttributeArea.Product, AttributeArea.Product
        , (int)AttributeArea.Channel, AttributeArea.Channel
        , (int)AttributeArea.Event, AttributeArea.Event
        )]
    public string? CustomerFormulaTerm { get; set; }

    [DisplayName("نوع داده")]
    [SBVR(SBVRModality.Obligatory, "نوع داده ویژگی",
        "نوع داده مقدار ویژگی تعیین می‌کند که مقدار چگونه تفسیر و تبدیل شود. " +
        "String: مقدار به صورت متن استفاده می‌شود. " +
        "Integer: مقدار به عدد صحیح تبدیل می‌شود. " +
        "Decimal: مقدار به عدد اعشاری تبدیل می‌شود. " +
        "Boolean: مقدار به بولین تبدیل می‌شود (true/false). " +
        "DateTime: مقدار به تاریخ و زمان تبدیل می‌شود. " +
        "DateOnly: مقدار به تاریخ تبدیل می‌شود (بدون زمان).")]
    public TenantAttributeType ValueType { get; set; } = TenantAttributeType.String;

    [DisplayName("نوع ذخیره‌سازی")]
    [OldDbMap("Aggrigation")]
    public AttributeValueStorageType ValueStorageType { get; set; } = AttributeValueStorageType.Last;

    [DisplayName("عنوان")]
    [MaxLength(200)]
    [SBVR(SBVRModality.Recommended, "شناسایی ویژگی",
        "عنوان یا توضیحات ویژگی برای شناسایی و درک بهتر استفاده می‌شود. " +
        "این فیلد برای مستندسازی و نمایش در UI استفاده می‌شود.")]
    public string? Title { get; set; }

    [DisplayName("توضیحات")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Permitted, "مستندسازی ویژگی",
        "توضیحات تکمیلی برای مستندسازی و درک بهتر ویژگی استفاده می‌شود. " +
        "می‌تواند شامل مثال‌های استفاده، محدودیت‌ها و نکات مهم باشد.")]
    public string? Description { get; set; }

    [DisplayName("فعال")]
    [SBVR(SBVRModality.Permitted, "وضعیت ویژگی",
        "وضعیت فعال/غیرفعال ویژگی تعیین می‌کند که آیا این مقدار در فرمول‌ها قابل استفاده است یا خیر. " +
        "اگر غیرفعال باشد، در فرمول‌ها مقدار null یا مقدار پیش‌فرض استفاده می‌شود.")]
    public bool IsActive { get; set; } = true;

    [DisplayName("مقدار پیش‌فرض")]
    [MaxLength(500)]
    [SBVR(SBVRModality.Permitted, "مقدار پیش‌فرض ویژگی",
        "مقدار پیش‌فرض در صورت عدم وجود یا غیرفعال بودن ویژگی استفاده می‌شود. " +
        "این مقدار باید از همان نوع ValueType باشد.")]
    public string? DefaultValue { get; set; }

    /// <summary>
    /// آیا کنترل مقادیر مجاز بر اساس نوع پارامتر انجام شود؟
    /// مثلاً برای Long باید عدد باشد، برای DateOnly باید تاریخ معتبر باشد
    /// </summary>
    [DisplayName("کنترل بر اساس نوع")]
    public bool ValidateByType { get; set; } = true;

    /// <summary>
    /// آیا کنترل مقادیر مجاز بر اساس لیست انجام شود؟
    /// اگر true باشد، باید مقادیر مجاز در جدول CustomerParameterAllowedValue تعریف شوند
    /// </summary>
    [DisplayName("کنترل بر اساس لیست")]
    public bool ValidateByList { get; set; } = false;

    [DisplayName("اختیاری است ؟")]
    [SBVR(SBVRModality.Permitted, "ویژگی اختیاری", "ویژگی اختیاری می‌توانند مقداردهی نشوند")]
    //TODO کاربردش کامل شرح داده شود
    public bool? IsOptional { get; set; }

    [DisplayName("ایجاد شده توسط سیستم")]
    [SBVR(SBVRModality.Permitted, "ویژگی سیستمی", "ویژگی ایجاد شده توسط سیستم به صورت خودکار هنگام اولین استفاده ایجاد می‌شوند")]
    public bool? CreatedBySystem { get; set; }

    [DisplayName("پارامتر ناحیه")]
    [Formula("Switch(Area,'-'" +
        ",{0},{1},{2},{3}" +
        ",{4},{5},{6},{7}" +
        ")"
        , (int)AttributeArea.Segment
        , $"IF(({nameof(SegmentId)}>0),({nameof(Segment)}.{nameof(Segment.Title)}),'-')"
        , (int)AttributeArea.Product
        , $"IF(({nameof(ProductId)}>0),({nameof(Product)}.{nameof(Product.Title)})," +
            $"IF(({nameof(ProductCategoryId)}>0),({nameof(ProductCategory)}.{nameof(ProductCategory.Title)}),'-')"+
            $")"
        , (int)AttributeArea.Channel
        , $"IF(({nameof(ChannelId)}>0),({nameof(Channel)}.{nameof(Channel.Title)}),'-')"
        , (int)AttributeArea.Event
        , $"IF(({nameof(EventTypeId)}>0),({nameof(EventType)}.{nameof(EventType.Title)}),'-')"
        )]
    public string AreaParameter { get; set; } = null!;

    public int? SegmentId { get; set; }
    [DisplayName("جامعه/بازار")]
    [SBVR(SBVRModality.Permitted, "محدودیت", "با تعیین جامعه/بازار، این ویژگی فقط برای این جامعه/بازار تعریف می شود")]
    public CustomerSegment? Segment { get; set; }

    public int? ProductCategoryId { get; set; }
    [DisplayName("دسته‌بندی محصول")]
    [SBVR(SBVRModality.Permitted, "محدودیت", "با تعیین دسته‌بندی محصول، این ویژگی فقط برای این دسته‌بندی محصول تعریف می شود")]
    public ProductCategory? ProductCategory { get; set; } = null!;

    public int? ProductId { get; set; }
    [DisplayName("محصول")]
    [SBVR(SBVRModality.Permitted, "محدودیت", "با تعیین محصول، این ویژگی فقط برای این محصول تعریف می شود")]
    public Product? Product { get; set; } = null!;

    public int? ChannelId { get; set; }
    [DisplayName("کانال")]
    public EventChannel? Channel { get; set; } = null!;

    public int? EventTypeId { get; set; }
    [DisplayName("رویداد")]
    public EventType? EventType { get; set; } = null!;

    /// <summary>
    /// لیست مقادیر مجاز (Navigation Property)
    /// </summary>
    public ICollection<TenantAttributeAllowedValue> AllowedValues { get; set; } = [];
}