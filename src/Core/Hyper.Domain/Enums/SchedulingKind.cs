namespace Hyper.Domain.Enums;

/// <summary>
/// نوع برنامه‌ریزی زمان‌بندی
/// </summary>
public enum SchedulingKind
{
	[Description("هر ساعت")]
	Hourly = 1,

	[Description("روزانه")]
	Daily = 2,

	[Description("هفتگی")]
	Weekly = 3,

	[Description("ماهانه")]
	Monthly = 4,

	[Description("سالانه")]
	Yearly = 5
}