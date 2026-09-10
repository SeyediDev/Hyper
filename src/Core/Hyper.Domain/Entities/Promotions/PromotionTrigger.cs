using Hyper.Domain.Entities.Lotteries;

namespace Hyper.Domain.Entities.Promotions;

/// <summary>
/// محرک عملیات پویش - تعریف‌کننده رویدادهایی که برای تحریک عملیات پویش باید دریافت شوند.
/// هر محرک در واقع تعریف دریافت یک رویداد ورودی است که باید به تعداد مشخص شده 
/// در بازه زمانی خاص (تاریخ و ساعت) دریافت شود.
/// محرک‌ها می‌توانند در گروه‌های مختلف قرار گیرند که محرک‌های هم‌گروه با هم AND می‌شوند 
/// (همه باید برقرار باشند) و گروه‌های مختلف با هم OR می‌شوند (یکی کافی است).
/// </summary>
[DisplayName("محرک عملیات پویش")]
[SBVR(SBVRModality.Obligatory, "ماموریت محرک عملیات پویش", 
    "محرک عملیات پویش تعریف‌کننده رویدادهایی است که برای تحریک عملیات پویش باید دریافت شوند. " +
    "هر محرک دریافت یک رویداد ورودی با تعداد مشخص در بازه زمانی خاص است. " +
    "محرک‌های هم‌گروه با AND و گروه‌های مختلف با OR ترکیب می‌شوند.")]
public class PromotionTrigger : HyperBaseCoreConfigAuditableEntity<int>, ISubOfPromotion
{
    [Formula("Promotion.TenantId")]
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    public Tenant Tenant { get; set; } = null!;

    public int PromotionId { get; set; }
    [DisplayName("پویش")]
    [SBVR(SBVRModality.Obligatory, "رابطه محرک-پویش", "هر محرک باید به یک پویش مشخص تعلق داشته باشد")]
    public Promotion Promotion { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

	[DisplayName("گروه محرک")]
	[SBVR(SBVRModality.Permitted, "گروه‌بندی محرک",
        @"گروه محرک - برای ترکیب چند محرک با AND
          محرک‌های هم‌گروه با هم AND می‌شوند (همه باید برقرار باشند)
          گروه‌های مختلف با هم OR می‌شوند (یکی کافی است)")]
	public ConditionGroup? TriggerGroup { get; set; }
	
    [DisplayName("نوع رویداد")]
	[SBVR(SBVRModality.Obligatory, "نوع رویداد", "نوع رویداد تعیین می‌کند که به ازای دریافت چه رویدادی محرک فعال می شود. این رویدادها می‌تواند پویا باشد یا به صورت اختصاصی باشد. که در هر مورد با فیلدهای تکمیلی دیتای تعریف رویداد تکمیل می شود")]
	public ReceiveEventType ReceiveEventType { get; set; }

	[DisplayName("منبع رویداد")]
	[Formula("Switch("+nameof(ReceiveEventType) +",'-'"
		+ ",{0},{1}"   // DynamicEvent
		+ ",{2},{3}"   // ReferCustomer
		+ ",{4},{5}"   // UpgradePointLevel
		+ ",{6},{7}"   // PurchaseReward
		+ ",{8},{9}"   // ConsumeReward
		+ ",{10},{11}" // ParticipateInSurvey
		+ ",{12},{13}" // ParticipateInLottery
		+ ",{14},{15}" // InteractInFeedback
		+ ",{16},{17}" // InteractInForum
		+ ",{18},{19}" // PurchasePlan
		+ ",{20},{21}" // CustomerCallCenterInteraction
		+ ",{22},{23}" // TransferPoint
        + ",{24},{25}" // ChangePoint
        + ")"
        // DynamicEvent = 1: Channel + EventType + Product + ProductCategory
        , (int)ReceiveEventType.DynamicEvent
		, $"Concat(" +
			$"IF(({nameof(ChannelId)}>0),({nameof(Channel)}.{nameof(Channel.Title)}),''),' '," +
			$"IF(({nameof(EventTypeId)}>0),({nameof(EventType)}.{nameof(EventType.Title)}),''),' '," +
            $"IF(({nameof(ProductId)}>0),({nameof(Product)}.{nameof(Product.Title)})," +
                $"IF(({nameof(ProductCategoryId)}>0),({nameof(ProductCategory)}.{nameof(ProductCategory.Title)}),''))" +
            $")"
        // ReferCustomer = 2: No supplementary field
        , (int)ReceiveEventType.ReferCustomer
		, "'-'"
		// UpgradePointLevel = 3: PointLevel
		, (int)ReceiveEventType.UpgradePointLevel
		, $"IF(({nameof(PointLevelId)}>0),({nameof(PointLevel)}.{nameof(PointLevel.Title)}),'-')"
		// PurchaseReward = 4: Reward + RewardCategory
		, (int)ReceiveEventType.PurchaseReward
		, $"Concat(" +
			$"IF(({nameof(RewardId)}>0),({nameof(Reward)}.{nameof(Reward.Title)})),' '," +
			$"IF(({nameof(RewardCategoryId)}>0),({nameof(RewardCategory)}.{nameof(RewardCategory.Title)}))" +
			$")"
		// ConsumeReward = 5: Reward + RewardCategory
		, (int)ReceiveEventType.ConsumeReward
		, $"Concat(" +
			$"IF(({nameof(RewardId)}>0),({nameof(Reward)}.{nameof(Reward.Title)})),' '," +
			$"IF(({nameof(RewardCategoryId)}>0),({nameof(RewardCategory)}.{nameof(RewardCategory.Title)}))" +
			$")"
		// ParticipateInSurvey = 7: Survey
		, (int)ReceiveEventType.ParticipateInSurvey
		, $"IF(({nameof(SurveyId)}>0),({nameof(Survey)}.{nameof(Survey.Title)}),'-')"
		// ParticipateInLottery = 8: Lottery
		, (int)ReceiveEventType.ParticipateInLottery
		, $"IF(({nameof(LotteryId)}>0),({nameof(Lottery)}.{nameof(Lottery.Title)}),'-')"
		// InteractInFeedback = 9: FeedbackType (enum - cast to string)
		, (int)ReceiveEventType.InteractInFeedback
		, $"IF(({nameof(FeedbackType)}!=null),String({nameof(FeedbackType)}),'-')"
		// InteractInForum = 10: ForumTopic
		, (int)ReceiveEventType.InteractInForum
		, $"IF(({nameof(ForumTopicId)}>0),({nameof(ForumTopic)}.{nameof(ForumTopic.Title)}),'-')"
		// PurchasePlan = 11: Plan
		, (int)ReceiveEventType.PurchasePlan
		, $"IF(({nameof(PlanId)}>0),({nameof(Plan)}.{nameof(Plan.Title)}),'-')"
		// CustomerCallCenterInteraction = 12: CallCenterInteractionType
		, (int)ReceiveEventType.CustomerCallCenterInteraction
		, $"IF(({nameof(CallCenterInteractionTypeId)}>0),({nameof(CallCenterInteractionType)}.{nameof(InteractionType.Title)}),'-')"
        , (int)ReceiveEventType.TransferPoint
        , $"Concat('از ',IF(({nameof(FromPointId)}>0),({nameof(FromPoint)}.{nameof(FromPoint.Title)}),'-'),' به ',IF(({nameof(ToPointId)}>0),({nameof(ToPoint)}.{nameof(ToPoint.Title)}),'-'))"
        , (int)ReceiveEventType.ChangePoint
        , $"Concat('از ',IF(({nameof(FromPointId)}>0),({nameof(FromPoint)}.{nameof(FromPoint.Title)}),'-'),' به ',IF(({nameof(ToPointId)}>0),({nameof(ToPoint)}.{nameof(ToPoint.Title)}),'-'))"
    )]
	public string? EventSource { get; set; }
	// ===== برای Event Triggers =====

	/// <summary>
	/// For PromotionTrigger.DynamicEvent
	/// </summary>
	public int? ChannelId { get; set; }
    
    [DisplayName("کانال اکوسیستم")]
    public EventChannel? Channel { get; set; }

	/// <summary>
	/// For PromotionTrigger.DynamicEvent
	/// </summary>
	public int? EventTypeId { get; set; }
    
    [DisplayName("رویداد")]
    [SBVR(SBVRModality.Permitted, "محرک رویداد", "رویداد برای تعریف محرکی که بر اساس دریافت رویداد فعال می‌شود")]
    public EventType? EventType { get; set; }

    /// <summary>
    /// For PromotionTrigger.DynamicEvent - دسته‌بندی محصول
    /// نال = همه دسته‌بندی های محصول
    /// </summary>
    public int? ProductCategoryId { get; set; }

    [DisplayName("دسته‌بندی محصول")]
    [SBVR(SBVRModality.Permitted, "محرک دسته‌بندی محصول", "دسته‌بندی محصول برای تعریف محرکی که بر اساس خرید از گروه خاصی از محصولات فعال می‌شود. نال = همه گروه‌ها")]
    public ProductCategory? ProductCategory { get; set; }

    /// <summary>
    /// For PromotionTrigger.DynamicEvent - محصولات 
    /// </summary>
    public int? ProductId { get; set; }

    [DisplayName("محصول")]
    [SBVR(SBVRModality.Permitted, "محرک محصول", "محصول اکوسیستم برای تعریف محرکی که بر اساس خرید محصولات اکوسیستمی فعال می‌شود")]
    public Product? Product { get; set; }


    /// <summary>
    /// For PromotionTrigger.UpgradePointLevel
    /// </summary>
    public int? PointLevelId { get; set; }

	[DisplayName("سطح‌امتیاز")]
	[SBVR(SBVRModality.Permitted, "محرک سطح امتیاز", "سطح امتیاز برای تعریف محرکی که بر اساس ارتقاء سطح فعال می‌شود")]
	public PointLevel? PointLevel { get; set; }

    /// <summary>
    /// For PromotionTrigger.TrasferPoint || PromotionTrigger.ChangePoint
    /// </summary>
    public int? FromPointId { get; set; }
    [DisplayName("از امتیاز")]
    public Point? FromPoint { get; set; }

    public int? ToPointId { get; set; }
    [DisplayName("به امتیاز")]
    public Point? ToPoint { get; set; }

    /// <summary>
    /// For PromotionTrigger.PurchaseReward or PromotionTrigger.ConsumeReward - طبقه‌بندی پاداش
    /// نال = همه طبقه‌بندی‌ها
    /// </summary>
    public int? RewardCategoryId { get; set; }

    [DisplayName("طبقه‌بندی پاداش")]
    [SBVR(SBVRModality.Permitted, "محرک طبقه‌بندی پاداش", "طبقه‌بندی پاداش برای تعریف محرکی که بر اساس خرید یا استفاده از پاداش‌های گروه خاص فعال می‌شود. نال = همه طبقه‌بندی‌ها")]
    public RewardCategory? RewardCategory { get; set; }

    /// <summary>
    /// For PromotionTrigger.PurchaseReward or PromotionTrigger.ConsumeReward
    /// </summary>
    public int? RewardId { get; set; }

	[DisplayName("پاداش")]
	[SBVR(SBVRModality.Permitted, "محرک پاداش", "پاداش برای تعریف محرکی که بر اساس خرید یا استفاده از پاداش فعال می‌شود")]
	[OldDbMap("Award")]
	public Reward? Reward { get; set; }

    /// <summary>
    /// For PromotionTrigger.ParticipateInSurvey - نظرسنجی/مسابقه
    /// نال = همه نظرسنجی‌ها
    /// </summary>
    public int? SurveyId { get; set; }
    [DisplayName("نظرسنجی")]
    [SBVR(SBVRModality.Permitted, "محرک نظرسنجی", "نظرسنجی برای تعریف محرکی که بر اساس شرکت در نظرسنجی خاص فعال می‌شود. نال = همه نظرسنجی‌ها")]
    public Survey? Survey { get; set; }

    /// <summary>
    /// For PromotionTrigger.ParticipateInLottery - قرعه‌کشی
    /// نال = همه قرعه‌کشی‌ها
    /// </summary>
    public int? LotteryId { get; set; }
    [DisplayName("قرعه‌کشی")]
    [SBVR(SBVRModality.Permitted, "محرک قرعه‌کشی", "قرعه‌کشی برای تعریف محرکی که بر اساس شرکت در قرعه‌کشی خاص فعال می‌شود. نال = همه قرعه‌کشی‌ها")]
    public Lottery? Lottery { get; set; }

    /// <summary>
    /// For PromotionTrigger.InteractInFeedback - نوع بازخورد
    /// نال = همه انواع بازخورد
    /// </summary>
    [DisplayName("نوع بازخورد")]
    [SBVR(SBVRModality.Permitted, "محرک نوع بازخورد", "نوع بازخورد برای تعریف محرکی که بر اساس ثبت نوع خاصی از بازخورد فعال می‌شود. نال = همه انواع")]
    public FeedbackType? FeedbackType { get; set; }

    /// <summary>
    /// For PromotionTrigger.InteractInForum - موضوع انجمن
    /// نال = همه موضوعات
    /// </summary>
    public int? ForumTopicId { get; set; }
    [DisplayName("موضوع انجمن")]
    [SBVR(SBVRModality.Permitted, "محرک موضوع انجمن", "موضوع انجمن برای تعریف محرکی که بر اساس تعامل در موضوع خاص فعال می‌شود. نال = همه موضوعات")]
    public ForumTopic? ForumTopic { get; set; }

    /// <summary>
    /// For PromotionTrigger.PurchasePlan - طرح اشتراک
    /// نال = همه طرح‌ها
    /// </summary>
    public int? PlanId { get; set; }
    [DisplayName("طرح اشتراک")]
    [SBVR(SBVRModality.Permitted, "محرک طرح اشتراک", "طرح اشتراک برای تعریف محرکی که بر اساس خرید طرح خاص فعال می‌شود. نال = همه طرح‌ها")]
    public Plan? Plan { get; set; }

    /// <summary>
    /// For PromotionTrigger.CustomerCallCenterInteraction - نوع تعامل
    /// نال = همه انواع تعامل
    /// </summary>
    public int? CallCenterInteractionTypeId { get; set; }
    [DisplayName("نوع تعامل مرکز تماس")]
    [SBVR(SBVRModality.Permitted, "محرک نوع تعامل", "نوع تعامل برای تعریف محرکی که بر اساس تعامل خاص با مرکز تماس فعال می‌شود. نال = همه انواع")]
    public InteractionType? CallCenterInteractionType { get; set; }

    // ===== پنجره تعدادی (Counter Window) =====

    /// <summary>
    /// نحوه شمارش پنجره تعدادی برای این محرک
    /// </summary>
    [DisplayName("نحوه شمارش پنجره تعدادی")]
    [SBVR(SBVRModality.Permitted, "پنجره تعدادی", "نحوه شمارش تعیین می‌کند که چگونه تعداد دفعات دریافت رویداد محاسبه شود")]
    public PromotionCounterWindowMode? CounterWindowMode { get; set; }

    /// <summary>
    /// آستانه تعداد دفعات دریافت رویداد برای این محرک
    /// </summary>
    [DisplayName("آستانه تعداد دفعات")]
    [SBVR(SBVRModality.Permitted, "پنجره تعدادی", "آستانه تعداد دفعات تعیین می‌کند که چند بار این رویداد باید دریافت شود")]
    public int? Threshold { get; set; }

    [DisplayName("حداکثر آستانه تعداد دفعات")]
    [SBVR(SBVRModality.Permitted, "پنجره تعدادی", "آستانه تعداد دفعات تعیین می‌کند که چند بار این رویداد باید دریافت شود")]
    public int? MaxThreshold { get; set; }

    // ===== پنجره زمانی (Time Window) =====
    /// <summary>
    /// آیا این محرک بازه زمانی مستقل دارد؟
    /// اگر true باشد، بازه زمانی محرک استفاده می‌شود
    /// اگر false باشد، از بازه زمانی Promotion استفاده می‌شود
    /// </summary>
    [DisplayName("استفاده از بازه زمانی مستقل")]
    [SBVR(SBVRModality.Permitted, "بازه زمانی محرک", "استفاده از بازه زمانی مستقل برای محرک. اگر تیک نباشد محرک در بازه زمانی پویش فعال می باشد")]
    public bool? HasIndependentTimeWindow { get; set; }

    /// <summary>
    /// تاریخ شروع بازه زمانی محرک
    /// </summary>
    [DisplayName("تاریخ شروع")]
    [SBVR(SBVRModality.Permitted, "بازه زمانی محرک", "تاریخ شروع بازه زمانی محرک. این تاریخ باید مساوی یا بعد از تاریخ شروع پویش باشد")]
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// تاریخ پایان بازه زمانی محرک
    /// </summary>
    [DisplayName("تاریخ پایان")]
    [SBVR(SBVRModality.Permitted, "بازه زمانی محرک", "تاریخ پایان بازه زمانی محرک. این تاریخ باید مساوی یا قبل از تاریخ انتهای پویش باشد.")]
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// ساعت شروع بازه زمانی محرک (0-23)
    /// </summary>
    [DisplayName("ساعت شروع")]
    [SBVR(SBVRModality.Permitted, "بازه زمانی محرک", "ساعت شروع بازه زمانی محرک")]
    public int? FromHour { get; set; }

    /// <summary>
    /// ساعت پایان بازه زمانی محرک (0-23)
    /// </summary>
    [DisplayName("ساعت پایان")]
    [SBVR(SBVRModality.Permitted, "بازه زمانی محرک", "ساعت پایان بازه زمانی محرک")]
    public int? ToHour { get; set; }

    [DisplayName("نوع زمان بررسی")]
    [SBVR(SBVRModality.Permitted, "نوع زمان بررسی", "نوع زمان بررسی دریافت رویداد لازمه")]
    [SBVR(SBVRModality.Permitted, "پنجره تعدادی", "آنهایی که به‌محض دریافت رویداد هستند باید شرط پنجره تعدادی را پاس کرده باشند و آنهایی که زمان بندی شده اند در بازه زمان بندی شده وقت دارند شرط پنجره تعدای را پاس کنند.")]
    public PromotionTriggerCheckTimeKind? CheckTimeKind { get; set; }

    [DisplayName("تعداد دقیقه مهلت انجام رویداد از رویداد قبلی")]
    public int? MinutesRemaining { get; set; }

    [DisplayName("ماه زمان‌بندی شده")]
    [SBVR(SBVRModality.Permitted, "زمان‌بندی محرک", "در نوع زمان‌بندی انتهای هر سال نیاز است ذکر شود که چه ماهی انتهای سال در نظر گرفته می شود. شماره میلادی ماه. اعداد 1 الی 12")]
    public int? ScheduledMonth { get; set; }

    /// <summary>
    /// روز هفته برای بررسی (برای CheckTimeKind = Weekly)
    /// </summary>
    [DisplayName("روز هفته زمان‌بندی شده")]
    [SBVR(SBVRModality.Permitted, "زمان‌بندی محرک", "در نوع زمان‌بندی انتهای هر هفته نیاز است ذکر شود که چه روزی از هفته انتهای هفته در نظر گرفته می شود.")]
    public DayOfWeek? ScheduledWeekDay { get; set; }

    /// <summary>
    /// روز ماه برای بررسی (برای CheckTimeKind = Monthly یا Yearly)
    /// </summary>
    [DisplayName("روز ماه زمان‌بندی شده")]
    [SBVR(SBVRModality.Permitted, "زمان‌بندی محرک", "در نوع زمان‌بندی انتهای هر سال و انتهای هر ماه نیاز است ذکر شود که چه روزی از ماه انتهای بازه در نظر گرفته می شود.")]
    public int? ScheduledMonthDay { get; set; }

    /// <summary>
    /// ساعت انجام محرک (0-23)
    /// </summary>
    [DisplayName("ساعت")]
    [SBVR(SBVRModality.Permitted, "زمان‌بندی محرک", "ساعت برای زمان‌بندی دقیق اجرای محرک")]
    public int? ScheduledHour { get; set; }

    /// <summary>
    /// دقیقه انجام محرک (0-59)
    /// </summary>
    [DisplayName("دقیقه")]
    [SBVR(SBVRModality.Permitted, "زمان‌بندی محرک", "دقیقه برای زمان‌بندی دقیق اجرای محرک")]
    public int? ScheduledMinute { get; set; }

    /// <summary>
    /// ترتیب در توالی (برای Campaign - مدیریت تقدم و تأخر)
    /// </summary>
    [DisplayName("ترتیب")]
    [SBVR(SBVRModality.Permitted, "ترتیب محرک", "ترتیب برای Campaign ها - تعیین می‌کند که این محرک در چه مرحله‌ای از توالی قرار دارد")]
    public int? SequenceOrder { get; set; }

    /// <summary>
    /// آیا با محرک دیگر موازی است (برای Campaign - parallel execution)
    /// </summary>
    [DisplayName("نوع جریان رویداد")]
    [SBVR(SBVRModality.Permitted, "نوع جریان رویداد", "به ازای نوع جریان رویداد موازی و بعد از یک رویداد، باید یک رویداد انتخاب شود")]
    public PromotionTriggerFlowType? FlowType { get; set; }

    [DisplayName("شناسه رویداد محرک وابسته")]
    public int? DependencyTriggerId { get; set; }
    [DisplayName("رویداد محرک وابسته")]
    public PromotionTrigger? DependencyTrigger { get; set; }

    // ===== شرط محرک =====
    /// <summary>
    /// شرط محرک - فرمول شرطی که باید برای فعال شدن محرک برقرار باشد.
    /// 
    /// شرط به صورت یک فرمول قابل خواندن نوشته می‌شود که می‌تواند شامل:
    /// - پارامترهای رویداد (Event.*)
    /// - فیلدهای مشتری (Customer.*)
    /// - مقادیر تنظیمات اکوسیستم (Tenant.*)
    /// - توابع ریاضی و منطقی
    /// 
    /// مثال شرط ساده:
    /// "Event.Cost > 1000000"
    /// 
    /// مثال شرط پیچیده:
    /// "((Customer.Age > Tenant.MaxRegularAge) && (Event.Cost > Tenant.AverageCost)) || (Event.Force == true)"
    /// 
    /// نحوه استفاده در فرمول:
    /// - Customer.*: دسترسی به فیلدهای مشتری (Age, FirstName, LastName, Mobile, Email, BirthDate, ...)
    /// - Tenant.*: دسترسی به مقادیر تنظیمات اکوسیستم (MaxRegularAge, AverageCost, MinPurchaseAmount, ...)
    /// - Event.*: دسترسی به پارامترهای رویداد (Cost, Amount, Force, ...)
    /// - توابع: Math.Max(), Math.Min(), String.Contains(), DateDiff(), ...
    /// - عملگرهای مقایسه: ==, !=, >, <, >=, <=
    /// - عملگرهای منطقی: && (AND), || (OR), ! (NOT)
    /// </summary>
    [DisplayName("شرط")]
    [MaxLength(2000)]
    [SBVR(SBVRModality.Permitted, "شرط محرک",
        "شرط محرک به صورت یک فرمول قابل خواندن نوشته می‌شود که شرط را تعریف می‌کند. " +
        "این فرمول باید یک عبارت بولی باشد که true یا false برمی‌گرداند. " +
        "فرمول می‌تواند از موارد زیر استفاده کند: " +
        "1. پارامترهای رویداد با پیشوند Event.* (مثال: Event.Cost, Event.Amount, Event.Force) " +
        "2. نوع رفتار مشتری با Event.BehaviorTypeValue (عدد) یا Event.BehaviorType (string) " +
        "   - Event.BehaviorTypeValue == 1 (خرید), 2 (ابطال خرید), 3 (مشاهده محصول), 4 (افزودن به سبد), 5 (حذف از سبد), " +
        "     6 (شروع تسویه), 7 (تکمیل تسویه), 8 (نصب برنامه), 9 (حذف برنامه), 10 (ورود), 11 (خروج), 12 (ثبت‌نام), " +
        "     13 (جستجو), 14 (اشتراک‌گذاری), 15 (نظر دادن), 16 (دانلود), 17 (اشتراک), 18 (لغو اشتراک), " +
        "     19 (مشاهده صفحه), 20 (پخش ویدیو), 21 (اتمام ویدیو), 22 (کلیک), 23 (اسکرول), 24 (زمان سپری شده), " +
        "     25 (افزودن به علاقه‌مندی‌ها), 26 (حذف از علاقه‌مندی‌ها), 27 (بازگشت), 28 (خرید مجدد), " +
        "     29 (ارجاع), 30 (تعامل با محتوا), 0 (سفارشی) " +
        "   - Event.BehaviorType == \"Purchase\" یا Event.BehaviorType == \"Login\" (نام enum به صورت string) " +
        "3. فیلدهای مشتری با پیشوند Customer.* (مثال: Customer.Age, Customer.FirstName, Customer.BirthDate) " +
        "4. مقادیر تنظیمات اکوسیستم با پیشوند Tenant.* (مثال: Tenant.MaxRegularAge, Tenant.AverageCost) " +
        "5. توابع ریاضی: Math.Max(), Math.Min(), Math.Abs(), Math.Round(), Math.Floor(), Math.Ceiling() " +
        "6. توابع رشته: String.Contains(), String.StartsWith(), String.EndsWith(), String.ToLower(), String.ToUpper() " +
        "7. توابع تاریخ: DateDiff(), Year(), Month(), Day(), Now(), Today() " +
        "8. عملگرهای مقایسه: == (برابر), != (نابرابر), > (بزرگتر), < (کوچکتر), >= (بزرگتر مساوی), <= (کوچکتر مساوی) " +
        "9. عملگرهای منطقی: && (AND), || (OR), ! (NOT) " +
        "10. پرانتز برای اولویت عملیات: () " +
        "مثال شرط ساده: 'Event.Cost > 1000000' " +
        "مثال شرط برای خرید: 'Event.BehaviorTypeValue == 1' یا 'Event.BehaviorType == \"Purchase\"' " +
        "مثال شرط برای ورود یا ثبت‌نام: '(Event.BehaviorTypeValue == 10) || (Event.BehaviorTypeValue == 12)' " +
        "مثال شرط برای رفتارهای مرتبط با محصول: '(Event.BehaviorTypeValue >= 1) && (Event.BehaviorTypeValue <= 7)' " +
        "مثال شرط پیچیده: '((Event.BehaviorTypeValue == 1) && (Event.Cost > 1000000)) || (Event.BehaviorType == \"Registration\")' " +
        "مثال شرط ترکیبی: '((Customer.Age > Tenant.MaxRegularAge) && (Event.BehaviorTypeValue == 1) && (Event.Cost > Tenant.AverageCost)) || (Event.Force == true)' " +
        "نکته: برای مقادیر متنی از کوتیشن استفاده کنید: 'Customer.FirstName == \"علی\"' یا 'Event.BehaviorType == \"Purchase\"' " +
        "نکته: برای مقادیر بولی از true یا false استفاده کنید: 'Event.Force == true' " +
        "نکته: استفاده از BehaviorTypeValue (عدد) برای مقایسه سریع‌تر است. " +
        "نکته: استفاده از BehaviorType (string) برای خوانایی بهتر است. " +
        "نکته مهم: اگر شرط خالی یا null باشد، به معنای بدون شرط اضافی است و همیشه true برمی‌گرداند (محرک بدون شرط اضافی فعال می‌شود).")]
    [SBVR(SBVRModality.Permitted, "توابع ریاضی", "راهنمای استفاده در فرمول نویسی ها" +
@"
### Math Patterns:

| Pattern               | Meaning                                | Example                      | Output                |
|-----------------------|----------------------------------------|------------------------------|-----------------------|
| Math.Max()            |                                        | Max(2,3)                     | 3                     |
| Math.Min()            |                                        | Min(2,3)                     | 2                     |
| Math.Abs()            |                                        | Abs(-2)                      | 2                     |
| Math.Round()          |                                        | Round(2.2)                   | 2                     |
| Math.Floor()          |                                        | Floor(2.3000)                | 2.3                   |
| Math.Ceiling()        |                                        | Ceiling(2.1)                 | 2                     |
")]
    [SBVR(SBVRModality.Permitted, "کلید خواندن ویژگی‌ها", "راهنمای استفاده در فرمول نویسی ها" +
@"
### Patterns:

| Pattern                     | Meaning                                | Example                      | Output                |
|-----------------------------|----------------------------------------|------------------------------|-----------------------|
| CustomerPoint.{Key}         | موجودی مشتری در امتیاز با این کلید    | CustomerPoint.Momtaz         | 125                   |
| CustomerPoint.{Key}.Level   | کلید سطح مشتری در امتیاز با این کلید  | CustomerPoint.Momtaz.Level   | Platinium             |
| CustomerPoint.{Key}.LevelId | سطح، سطح مشتری در امتیاز با این کلید  | CustomerPoint.Momtaz.LevelId | 3                     |")]
    public string? Condition { get; set; }
}