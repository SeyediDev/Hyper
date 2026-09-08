namespace Hyper.Domain.Entities.Rewards.Views;

/// <summary>
/// ویو سورس گزارش Pivot: پاداش × کانال × طرح
/// این ویو داده‌های تراکنش‌های خرید پاداش را به تفکیک کانال و طرح فعال مشتری در لحظه خرید گروه‌بندی می‌کند
/// </summary>
[DontAudit]
[View(@"SELECT
	ct.CustomerTenantId,
	CAST(ct.CreateDate AS date)                AS TxDate,
	ct.RewardId,
	r.Title                                     AS RewardTitle,
	ct.EventChannelId                           AS ChannelId,
	ec.Title                                    AS ChannelTitle,
	ISNULL(ct.ActivePlanId, 0)                  AS PlanId,
	COALESCE(p.Title, N'بدون طرح')             AS PlanTitle,
	COUNT_BIG(1)                                AS Qty,
	SUM(ISNULL(ct.Debit, 0))                    AS PointsSpent,
	AVG(CASE WHEN ct.Debit IS NOT NULL THEN CONVERT(decimal(18,2), ct.Debit) END) AS AvgPointsPerUnit
FROM Core.CustomerTransactions AS ct
LEFT JOIN Core.Rewards          AS r  ON r.Id = ct.RewardId
LEFT JOIN CoreConfig.EventChannels AS ec ON ec.Id = ct.EventChannelId
LEFT JOIN Core.Plans            AS p  ON p.Id = ct.ActivePlanId
WHERE
	ct.IsDeleted = 0
	AND ct.RewardId IS NOT NULL
	AND ct.Debit IS NOT NULL
GROUP BY
	ct.CustomerTenantId,
	CAST(ct.CreateDate AS date),
	ct.RewardId,
	r.Title,
	ct.EventChannelId,
	ec.Title,
	ISNULL(ct.ActivePlanId, 0),
	COALESCE(p.Title, N'بدون طرح')", true)]
[DisplayName("گزارش Pivot پاداش × کانال × طرح")]
[Schema(nameof(DomainSchema.Core))]
[FileGroup(nameof(DomainSchema.Core))]
[ArchivePartition(nameof(DomainSchema.Core))]
public class RewardChannelPlanPivotSourceView: IView
{
	/// <summary>
	/// شناسه مشتری
	/// </summary>
	[DisplayName("شناسه مشتری")]
	public int CustomerTenantId { get; set; }

	/// <summary>
	/// تاریخ تراکنش
	/// </summary>
	[DisplayName("تاریخ تراکنش")]
	public DateTime TxDate { get; set; }

	/// <summary>
	/// شناسه پاداش
	/// </summary>
	[DisplayName("شناسه پاداش")]
	public int RewardId { get; set; }

	/// <summary>
	/// عنوان پاداش
	/// </summary>
	[DisplayName("عنوان پاداش")]
	[InDisplayString]
	[MaxLength(100)]
	public string RewardTitle { get; set; } = null!;

	/// <summary>
	/// شناسه کانال
	/// </summary>
	[DisplayName("شناسه کانال")]
	public int? ChannelId { get; set; }

	/// <summary>
	/// عنوان کانال
	/// </summary>
	[DisplayName("عنوان کانال")]
	[InDisplayString]
	[MaxLength(100)]
	public string? ChannelTitle { get; set; }

	/// <summary>
	/// شناسه طرح (0 = بدون طرح)
	/// </summary>
	[DisplayName("شناسه طرح")]
	public int PlanId { get; set; }

	/// <summary>
	/// عنوان طرح
	/// </summary>
	[DisplayName("عنوان طرح")]
	[InDisplayString]
	[MaxLength(100)]
	public string PlanTitle { get; set; } = null!;

	/// <summary>
	/// تعداد خرید
	/// </summary>
	[DisplayName("تعداد خرید")]
	public long Qty { get; set; }

	/// <summary>
	/// مجموع امتیاز مصرف‌شده
	/// </summary>
	[DisplayName("مجموع امتیاز مصرف‌شده")]
	public long PointsSpent { get; set; }

	/// <summary>
	/// میانگین امتیاز به ازای هر واحد
	/// </summary>
	[DisplayName("میانگین امتیاز به ازای هر واحد")]
	public decimal? AvgPointsPerUnit { get; set; }
}

