namespace Hyper.Domain.Entities.Lotteries;

/// <summary>
/// شرکت‌کنندگان قرعه‌کشی - ثبت شرکت‌کنندگان در قرعه‌کشی و نتیجه آن
/// این موجودیت برای ردیابی شرکت‌کنندگان، مشخص کردن برندگان و غیر فعال کردن شرکت‌کندگان
/// </summary>
[DisplayName("شرکت‌کننده قرعه‌کشی")]
[SBVR(SBVRModality.Obligatory, "ردیابی شرکت‌کنندگان", "هر شرکت در قرعه‌کشی باید ثبت شود تا تعیین برندگان، جلوگیری از شرکت مکرر و تحلیل مشارکت امکان‌پذیر باشد")]
[SBVR(SBVRModality.Recommended, "ردیابی شرکت‌کنندگان", "شرکت‌کنندگان برای تحلیل رفتار مشتریان، محاسبه نرخ مشارکت و ارزیابی اثربخشی قرعه‌کشی استفاده می‌شود")]
public class LotteryParticipant : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه قرعه‌کشی
    /// </summary>
    public int LotteryId { get; set; }

    [DisplayName("قرعه‌کشی")]
    [SBVR(SBVRModality.Obligatory, "رابطه شرکت-قرعه‌کشی", "هر شرکت‌کننده باید به یک قرعه‌کشی مشخص تعلق داشته باشد")]
    public Lottery Lottery { get; set; } = null!;

    [DisplayName("شناسه مشتری")]
    public int CustomerTenantId { get; set; }

    [DisplayName("مشتری")]
    [SBVR(SBVRModality.Obligatory, "رابطه مشتری-شرکت", "هر شرکت باید به یک مشتری مشخص تعلق داشته باشد")]
    [SBVR(SBVRModality.Recommended, "رابطه مشتری-شرکت", "رابطه مشتری-شرکت برای ردیابی تعداد شرکت‌های هر مشتری، جلوگیری از شرکت مکرر و تحلیل وفاداری استفاده می‌شود")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// شانس/وزن این شرکت‌کننده در قرعه‌کشی
    /// این مقدار برای محاسبه احتمال برنده شدن استفاده می‌شود
    /// می‌تواند از طریق PromotionAction (JoinLottery) افزایش یابد
    /// </summary>
    [DisplayName("شانس")]
    [SBVR(SBVRModality.Permitted, "شانس شرکت", "شانس برای تعیین احتمال برنده شدن این شرکت‌کننده استفاده می‌شود")]
    [Range(1, int.MaxValue, ErrorMessage = "شانس باید بیشتر از صفر باشد")]
    public int Chance { get; set; } = 1;

    /// <summary>
    /// آیا برنده شده است
    /// </summary>
    [DisplayName("برنده شده")]
    [SBVR(SBVRModality.Calculated, "نتیجه قرعه‌کشی", "برنده شدن برای تعیین اینکه آیا مشتری در این قرعه‌کشی برنده شده است استفاده می‌شود")]
    [SBVR(SBVRModality.Recommended, "نتیجه قرعه‌کشی", "برنده شدن برای مدیریت اهدای پاداش‌ها و تحلیل نرخ برنده‌ها استفاده می‌شود")]
    public bool IsWinner { get; set; }

    /// <summary>
    /// شناسه بخش چرخونه (LotteryReward) که روی آن متوقف شده است
    /// </summary>
    public int? LotteryRewardId { get; set; }

    [DisplayName("بخش چرخونه")]
    public LotteryReward? LotteryReward { get; set; }

    /// <summary>
    /// شناسه پاداش برنده شده - پاداشی که این شرکت‌کننده برنده شده است
    /// </summary>
    public int? RewardId { get; set; }

    [DisplayName("پاداش برنده شده")]
    [SBVR(SBVRModality.Permitted, "پاداش برنده", "پاداش برنده برای ردیابی نوع پاداشی که به برنده اهدا شده است استفاده می‌شود")]
    public Reward? Reward { get; set; }

    /// <summary>
    /// تعداد پاداش برنده شده - تعداد پاداش‌هایی که این شرکت‌کننده برنده شده است
    /// </summary>
    [DisplayName("تعداد پاداش برنده شده")]
    [SBVR(SBVRModality.Calculated, "مقدار پاداش", "تعداد پاداش برای تعیین تعداد پاداش‌هایی که به برنده اهدا می‌شود استفاده می‌شود")]
    public int? RewardAmount { get; set; }

    /// <summary>
    /// تاریخ شرکت - تاریخ و زمانی که مشتری در قرعه‌کشی شرکت کرده است
    /// </summary>
    [DisplayName("تاریخ شرکت")]
    [SBVR(SBVRModality.Obligatory, "ردیابی زمان شرکت", "تاریخ شرکت برای ردیابی زمان شرکت و جلوگیری از شرکت مکرر ضروری است")]
    [SBVR(SBVRModality.Calculated, "ردیابی زمان شرکت", "تاریخ شرکت = زمان ثبت شرکت در قرعه‌کشی")]
    public DateTime ParticipatedAt { get; set; }

    /// <summary>
    /// تاریخ اعلام برنده - تاریخ و زمانی که برنده بودن اعلام شده است (در صورت برنده شدن)
    /// </summary>
    [DisplayName("تاریخ اعلام برنده")]
    [SBVR(SBVRModality.Permitted, "ردیابی اهدای پاداش", "تاریخ اعلام برنده برای ردیابی زمان اهدای پاداش استفاده می‌شود")]
    public DateTime? AnnouncedAt { get; set; }

    /// <summary>
    /// آیا پاداش اهدا شده است
    /// </summary>
    [DisplayName("پاداش اهدا شده")]
    [SBVR(SBVRModality.Calculated, "ردیابی اهدای پاداش", "پاداش اهدا شده برای اطمینان از اهدای پاداش به برندگان استفاده می‌شود")]
    public bool IsRewardDistributed { get; set; }

    /// <summary>
    /// تاریخ توزیع پاداش - تاریخ و زمانی که پاداش به مشتری اهدا شده است
    /// </summary>
    [DisplayName("تاریخ توزیع پاداش")]
    [SBVR(SBVRModality.Permitted, "ردیابی اهدای پاداش", "تاریخ توزیع پاداش برای مستندسازی اهدای پاداش استفاده می‌شود")]
    public DateTime? RewardDistributedAt { get; set; }

    /// <summary>
    /// شناسه دارایی اهدا شده - در صورت اهدای پاداش فیزیکی، دارایی اهدا شده
    /// </summary>
    public int? RewardAssetId { get; set; }

    [DisplayName("دارایی اهدا شده")]
    [SBVR(SBVRModality.Permitted, "ردیابی دارایی", "دارایی اهدا شده برای ردیابی دارایی‌های فیزیکی اهدا شده به برندگان استفاده می‌شود")]
    public RewardAsset? RewardAsset { get; set; }
}