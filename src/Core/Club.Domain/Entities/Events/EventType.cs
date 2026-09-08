namespace Hyper.Domain.Entities.Events;

/// <summary>
/// رویداد را اینجا لیست می کنیم
/// </summary>
[DisplayName("رویداد")]
public class EventType : HyperBaseCoreConfigAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر نوع رویداد باید به یک اکوسیستم مشخص تعلق داشته باشد تا از تداخل داده‌ها جلوگیری شود")]
    public Tenant Tenant { get; set; } = null!;

    [MaxLength(40)]
    //[Unique]
    [DisplayName("کلید")]
    public string Key { get; set; } = null!;
    //[Unique]
    [DisplayName("عنوان")] [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

    [DisplayName("مجوز افزودن ویژگی جدید")]
    public bool AddNewAttributePermission { get; set; }

    /// <summary>
    /// نوع رفتار مشتری در چنل - بر اساس استانداردهای علمی رفتارشناسی کاربر
    /// این فیلد نوع رفتار مشتری را مشخص می‌کند (خرید، مشاهده محصول، نصب برنامه، ورود، و غیره)
    /// </summary>
    [DisplayName("نوع رفتار مشتری")]
    [SBVR(SBVRModality.Obligatory, "نوع رفتار مشتری",
        "هر نوع رویداد باید نوع رفتار مشتری را مشخص کند تا سیستم بتواند رفتار مشتری را تحلیل و پردازش کند. " +
        "این فیلد بر اساس استانداردهای علمی رفتارشناسی کاربر در پرتال‌ها و اپ‌های کاربردی تعریف شده است. " +
        "برای رویدادهای مربوط به محصول (مثل خرید، مشاهده محصول)، هنگام ثبت رویداد باید اطلاعات محصول و attribute های آن ارسال شوند. " +
        "این فیلد در فرمول‌نویسی شرط و فرمول مقدار در محرک عملیات و عملیات پویش قابل استفاده است.")]
    [SBVR(SBVRModality.Recommended, "استفاده در فرمول‌نویسی",
        "نوع رفتار مشتری در فرمول‌نویسی شرط و فرمول مقدار قابل استفاده است. " +
        "برای استفاده در فرمول‌ها از یکی از روش‌های زیر استفاده کنید: " +
        "1. Event.BehaviorTypeValue == 1 (مقدار عددی: 0=Custom, 1=Purchase, 2=PurchaseCancellation, 3=ProductView, 4=AddToCart, 5=RemoveFromCart, 6=CheckoutStart, 7=CheckoutComplete, 8=AppInstall, 9=AppUninstall, 10=Login, 11=Logout, 12=Registration, 13=Search, 14=Share, 15=Review, 16=Download, 17=Subscription, 18=Unsubscription, 19=PageView, 20=VideoPlay, 21=VideoComplete, 22=Click, 23=Scroll, 24=TimeSpent, 25=AddToWishlist, 26=RemoveFromWishlist, 27=Return, 28=RepeatPurchase, 29=Referral, 30=ContentInteraction, 31=CampaignParticipation, 32=SurveySubmission, 33=ContestEntry, 34=LotteryParticipation, 35=WheelSpin, 36=FeedbackSubmission, 37=CampaignLinkClick, 38=CampaignLandingPageView, 39=CampaignRegistration, 40=CampaignShare, 41=OtpVerification, 42=CampaignFormSubmission, 43=CampaignDetailsView, 44=CampaignCancellation, 45=CampaignRewardClaim, 46=CampaignInvitation, 47=CampaignResultsView, 48=CampaignStepCompletion, 49=CampaignProgressView, 50=CampaignNotificationReceived, 51=CampaignReturn, 52=CampaignContentDownload, 53=CampaignVideoView, 54=CampaignContentInteraction, 55=CampaignReview, 56=CampaignSave, 57=CampaignHistoryView, 58=CampaignCouponReceived, 59=CampaignCouponUsed) " +
        "2. Event.BehaviorType == \"Purchase\" (نام enum به صورت string) " +
        "3. Event.BehaviorType == \"Login\" || Event.BehaviorType == \"Registration\" (چند رفتار) " +
        "مثال شرط برای خرید: 'Event.BehaviorTypeValue == 1' یا 'Event.BehaviorType == \"Purchase\"' " +
        "مثال شرط برای ورود یا ثبت‌نام: '(Event.BehaviorTypeValue == 10) || (Event.BehaviorTypeValue == 12)' یا 'Event.BehaviorType == \"Login\" || Event.BehaviorType == \"Registration\"' " +
        "مثال شرط برای رفتارهای مرتبط با محصول: '(Event.BehaviorTypeValue >= 1) && (Event.BehaviorTypeValue <= 7)' (خرید تا تکمیل تسویه) " +
        "مثال شرط برای رفتارهای مرتبط با کمپین: '(Event.BehaviorTypeValue >= 31) && (Event.BehaviorTypeValue <= 59)' (تمام رفتارهای کمپین) " +
        "مثال شرط پیچیده: '((Event.BehaviorTypeValue == 1) && (Event.Cost > 1000000)) || (Event.BehaviorType == \"Registration\")' " +
        "نکته: استفاده از BehaviorTypeValue (عدد) برای مقایسه سریع‌تر است. " +
        "نکته: استفاده از BehaviorType (string) برای خوانایی بهتر است. " +
        "نکته: می‌توانید از عملگرهای منطقی && (AND) و || (OR) برای ترکیب چند رفتار استفاده کنید.")]
    public CustomerBehaviorType CustomerBehaviorType { get; set; } = CustomerBehaviorType.Custom;
}