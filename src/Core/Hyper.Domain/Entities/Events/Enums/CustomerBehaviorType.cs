namespace Hyper.Domain.Entities.Events.Enums;

/// <summary>
/// نوع رفتار مشتری در چنل - بر اساس استانداردهای علمی رفتارشناسی کاربر در پرتال‌ها و اپ‌های کاربردی
/// </summary>
[Description("نوع رفتار مشتری")]
public enum CustomerBehaviorType
{
    /// <summary>
    /// رفتار سفارشی - برای رفتارهای خاص که در دسته‌بندی‌های استاندارد قرار نمی‌گیرند
    /// </summary>
    [Description("سفارشی")]
    Custom = 0,

    /// <summary>
    /// خرید محصول/خدمت - رفتار اصلی خرید که شامل تکمیل فرآیند خرید می‌شود
    /// </summary>
    [Description("خرید محصول")]
    Purchase = 1,

    /// <summary>
    /// ابطال خرید - لغو یا بازگشت خرید انجام شده
    /// </summary>
    [Description("ابطال خرید")]
    PurchaseCancellation = 2,

    /// <summary>
    /// مشاهده محصول - بازدید از صفحه محصول بدون خرید
    /// </summary>
    [Description("مشاهده محصول")]
    ProductView = 3,

    /// <summary>
    /// افزودن به سبد خرید - اضافه کردن محصول به سبد خرید
    /// </summary>
    [Description("افزودن به سبد")]
    AddToCart = 4,

    /// <summary>
    /// حذف از سبد خرید - حذف محصول از سبد خرید
    /// </summary>
    [Description("حذف از سبد")]
    RemoveFromCart = 5,

    /// <summary>
    /// شروع تسویه حساب - شروع فرآیند checkout
    /// </summary>
    [Description("شروع تسویه")]
    CheckoutStart = 6,

    /// <summary>
    /// تکمیل تسویه حساب - تکمیل موفق فرآیند checkout
    /// </summary>
    [Description("تکمیل تسویه")]
    CheckoutComplete = 7,

    /// <summary>
    /// نصب برنامه - نصب اپلیکیشن موبایل یا دسکتاپ
    /// </summary>
    [Description("نصب برنامه")]
    AppInstall = 8,

    /// <summary>
    /// حذف برنامه - حذف اپلیکیشن
    /// </summary>
    [Description("حذف برنامه")]
    AppUninstall = 9,

    /// <summary>
    /// ورود به سیستم - Login کاربر
    /// </summary>
    [Description("ورود")]
    Login = 10,

    /// <summary>
    /// خروج از سیستم - Logout کاربر
    /// </summary>
    [Description("خروج")]
    Logout = 11,

    /// <summary>
    /// ثبت‌نام - ثبت‌نام کاربر جدید
    /// </summary>
    [Description("ثبت‌نام")]
    Registration = 12,

    /// <summary>
    /// جستجو - جستجوی محصول، محتوا یا اطلاعات
    /// </summary>
    [Description("جستجو")]
    Search = 13,

    /// <summary>
    /// اشتراک‌گذاری - به اشتراک گذاشتن محتوا یا محصول
    /// </summary>
    [Description("اشتراک‌گذاری")]
    Share = 14,

    /// <summary>
    /// نظر دادن - ثبت نظر یا رتبه‌دهی
    /// </summary>
    [Description("نظر دادن")]
    Review = 15,

    /// <summary>
    /// دانلود - دانلود فایل، محتوا یا محصول
    /// </summary>
    [Description("دانلود")]
    Download = 16,

    /// <summary>
    /// اشتراک - خرید یا فعال‌سازی اشتراک
    /// </summary>
    [Description("اشتراک")]
    Subscription = 17,

    /// <summary>
    /// لغو اشتراک - لغو اشتراک فعال
    /// </summary>
    [Description("لغو اشتراک")]
    Unsubscription = 18,

    /// <summary>
    /// مشاهده صفحه - بازدید از یک صفحه خاص
    /// </summary>
    [Description("مشاهده صفحه")]
    PageView = 19,

    /// <summary>
    /// پخش ویدیو - شروع پخش ویدیو
    /// </summary>
    [Description("پخش ویدیو")]
    VideoPlay = 20,

    /// <summary>
    /// اتمام ویدیو - تماشای کامل ویدیو
    /// </summary>
    [Description("اتمام ویدیو")]
    VideoComplete = 21,

    /// <summary>
    /// کلیک - کلیک روی لینک، دکمه یا عنصر رابط کاربری
    /// </summary>
    [Description("کلیک")]
    Click = 22,

    /// <summary>
    /// اسکرول - اسکرول کردن صفحه
    /// </summary>
    [Description("اسکرول")]
    Scroll = 23,

    /// <summary>
    /// زمان سپری شده - رویدادهای مبتنی بر زمان (مثل تماشای محتوا)
    /// </summary>
    [Description("زمان سپری شده")]
    TimeSpent = 24,

    /// <summary>
    /// افزودن به علاقه‌مندی‌ها - افزودن محصول یا محتوا به لیست علاقه‌مندی‌ها
    /// </summary>
    [Description("افزودن به علاقه‌مندی‌ها")]
    AddToWishlist = 25,

    /// <summary>
    /// حذف از علاقه‌مندی‌ها - حذف محصول یا محتوا از لیست علاقه‌مندی‌ها
    /// </summary>
    [Description("حذف از علاقه‌مندی‌ها")]
    RemoveFromWishlist = 26,

    /// <summary>
    /// بازگشت - بازگشت کاربر به سایت یا اپ
    /// </summary>
    [Description("بازگشت")]
    Return = 27,

    /// <summary>
    /// خرید مجدد - خرید مجدد محصول یا خدمت
    /// </summary>
    [Description("خرید مجدد")]
    RepeatPurchase = 28,

    /// <summary>
    /// ارجاع - ارجاع کاربر جدید توسط کاربر موجود
    /// </summary>
    [Description("ارجاع")]
    Referral = 29,

    /// <summary>
    /// تعامل با محتوا - لایک، کامنت، یا سایر تعاملات با محتوا
    /// </summary>
    [Description("تعامل با محتوا")]
    ContentInteraction = 30,

    /// <summary>
    /// شرکت در کمپین - شرکت در یک کمپین یا پویش
    /// </summary>
    [Description("شرکت در کمپین")]
    CampaignParticipation = 31,

    /// <summary>
    /// ارسال نظرسنجی - تکمیل و ارسال یک نظرسنجی
    /// </summary>
    [Description("ارسال نظرسنجی")]
    SurveySubmission = 32,

    /// <summary>
    /// شرکت در مسابقه - ثبت نام یا شرکت در یک مسابقه
    /// </summary>
    [Description("شرکت در مسابقه")]
    ContestEntry = 33,

    /// <summary>
    /// شرکت در قرعه‌کشی - ثبت نام یا شرکت در یک قرعه‌کشی
    /// </summary>
    [Description("شرکت در قرعه‌کشی")]
    LotteryParticipation = 34,

    /// <summary>
    /// چرخاندن چرخونه شانس - استفاده از چرخونه شانس
    /// </summary>
    [Description("چرخاندن چرخونه شانس")]
    WheelSpin = 35,

    /// <summary>
    /// ارسال بازخورد - ارسال بازخورد یا فیدبک
    /// </summary>
    [Description("ارسال بازخورد")]
    FeedbackSubmission = 36,

    /// <summary>
    /// کلیک روی لینک کمپین - کلیک روی لینک تبلیغاتی کمپین
    /// </summary>
    [Description("کلیک روی لینک کمپین")]
    CampaignLinkClick = 37,

    /// <summary>
    /// مشاهده صفحه فرود کمپین - بازدید از صفحه فرود کمپین
    /// </summary>
    [Description("مشاهده صفحه فرود کمپین")]
    CampaignLandingPageView = 38,

    /// <summary>
    /// ثبت‌نام در کمپین - ثبت‌نام در یک کمپین از طریق صفحه فرود
    /// </summary>
    [Description("ثبت‌نام در کمپین")]
    CampaignRegistration = 39,

    /// <summary>
    /// اشتراک‌گذاری کمپین - به اشتراک گذاشتن کمپین با دیگران
    /// </summary>
    [Description("اشتراک‌گذاری کمپین")]
    CampaignShare = 40,

    /// <summary>
    /// ورود کد OTP - ورود کد OTP برای احراز هویت در صفحه فرود
    /// </summary>
    [Description("ورود کد OTP")]
    OtpVerification = 41,

    /// <summary>
    /// تکمیل فرم کمپین - تکمیل فرم در صفحه فرود کمپین
    /// </summary>
    [Description("تکمیل فرم کمپین")]
    CampaignFormSubmission = 42,

    /// <summary>
    /// مشاهده جزئیات کمپین - مشاهده جزئیات یک کمپین
    /// </summary>
    [Description("مشاهده جزئیات کمپین")]
    CampaignDetailsView = 43,

    /// <summary>
    /// لغو شرکت در کمپین - لغو شرکت در یک کمپین
    /// </summary>
    [Description("لغو شرکت در کمپین")]
    CampaignCancellation = 44,

    /// <summary>
    /// دریافت جایزه از کمپین - دریافت جایزه یا پاداش از کمپین
    /// </summary>
    [Description("دریافت جایزه از کمپین")]
    CampaignRewardClaim = 45,

    /// <summary>
    /// دعوت به کمپین - دعوت دیگران به شرکت در کمپین
    /// </summary>
    [Description("دعوت به کمپین")]
    CampaignInvitation = 46,

    /// <summary>
    /// مشاهده نتایج کمپین - مشاهده نتایج یا برندگان کمپین
    /// </summary>
    [Description("مشاهده نتایج کمپین")]
    CampaignResultsView = 47,

    /// <summary>
    /// تکمیل مراحل کمپین - تکمیل یک مرحله از کمپین چندمرحله‌ای
    /// </summary>
    [Description("تکمیل مراحل کمپین")]
    CampaignStepCompletion = 48,

    /// <summary>
    /// مشاهده پیشرفت در کمپین - مشاهده پیشرفت خود در کمپین
    /// </summary>
    [Description("مشاهده پیشرفت در کمپین")]
    CampaignProgressView = 49,

    /// <summary>
    /// دریافت اعلان کمپین - دریافت اعلان درباره کمپین
    /// </summary>
    [Description("دریافت اعلان کمپین")]
    CampaignNotificationReceived = 50,

    /// <summary>
    /// بازگشت به کمپین - بازگشت به یک کمپین که قبلاً مشاهده شده
    /// </summary>
    [Description("بازگشت به کمپین")]
    CampaignReturn = 51,

    /// <summary>
    /// دانلود محتوای کمپین - دانلود فایل یا محتوای مرتبط با کمپین
    /// </summary>
    [Description("دانلود محتوای کمپین")]
    CampaignContentDownload = 52,

    /// <summary>
    /// مشاهده ویدیوی کمپین - مشاهده ویدیوی تبلیغاتی کمپین
    /// </summary>
    [Description("مشاهده ویدیوی کمپین")]
    CampaignVideoView = 53,

    /// <summary>
    /// تعامل با محتوای کمپین - لایک، کامنت یا تعامل با محتوای کمپین
    /// </summary>
    [Description("تعامل با محتوای کمپین")]
    CampaignContentInteraction = 54,

    /// <summary>
    /// ثبت نظر درباره کمپین - ثبت نظر یا بازخورد درباره کمپین
    /// </summary>
    [Description("ثبت نظر درباره کمپین")]
    CampaignReview = 55,

    /// <summary>
    /// ذخیره کمپین - ذخیره یا افزودن کمپین به لیست علاقه‌مندی‌ها
    /// </summary>
    [Description("ذخیره کمپین")]
    CampaignSave = 56,

    /// <summary>
    /// مشاهده تاریخچه کمپین - مشاهده تاریخچه شرکت در کمپین‌ها
    /// </summary>
    [Description("مشاهده تاریخچه کمپین")]
    CampaignHistoryView = 57,

    /// <summary>
    /// دریافت کد تخفیف از کمپین - دریافت کد تخفیف یا کوپن از کمپین
    /// </summary>
    [Description("دریافت کد تخفیف از کمپین")]
    CampaignCouponReceived = 58,

    /// <summary>
    /// استفاده از کد تخفیف کمپین - استفاده از کد تخفیف یا کوپن کمپین
    /// </summary>
    [Description("استفاده از کد تخفیف کمپین")]
    CampaignCouponUsed = 59
}

