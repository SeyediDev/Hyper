namespace Hyper.Domain.Entities.Lotteries.Enums;

/// <summary>
/// نوع قرعه‌کشی
/// </summary>
public enum LotteryType
{
    [Description("چرخونه - کاربر درخواست می‌دهد و فوراً قرعه‌کشی می‌شود")]
    Wheel = 1,
    
    [Description("زمان‌بندی شده - قرعه‌کشی خودکار در زمان مشخص")]
    Scheduled = 2
}