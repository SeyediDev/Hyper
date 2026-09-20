using Hyper.Domain.Entities.Database;
using Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Database;

/// <summary>
/// گزارش‌های بازاریابی اشتراک. منبع داده‌ی این گزارش یک view متراکم ماهانه است
/// تا ماه‌های بدون فعالیت نیز با مقدار صفر در نمودار دیده شوند.
/// </summary>
public sealed class SqlVwMarketingsubscriptionmonthlyUiDefinitions : CRUDDefinition<SqlVwMarketingsubscriptionmonthly>
{
    public override List<string>? Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];

    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];

        public class MonthlyConversionRateConfig : ChartConfigDefinition
        {
            public MonthlyConversionRateConfig() : base(ChartType.Line) { }

            protected override List<string> Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override string Name => "نرخ تبدیل ماهانه مشتریان جدید";
            protected override string WhereCondition => $"monthdiff({nameof(SqlVwMarketingsubscriptionmonthly.Monthstart)},getdate()) >= 0 && monthdiff({nameof(SqlVwMarketingsubscriptionmonthly.Monthstart)},getdate()) < 12";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(SqlVwMarketingsubscriptionmonthly.Monthstart), "ماه");
                Average(nameof(SqlVwMarketingsubscriptionmonthly.Conversionrate), "نرخ تبدیل (درصد)");
                Sum(nameof(SqlVwMarketingsubscriptionmonthly.Newpaidshopcount), "مشتری جدید");
            }
        }

        public class MonthlyRetentionRateConfig : ChartConfigDefinition
        {
            public MonthlyRetentionRateConfig() : base(ChartType.Line) { }

            protected override List<string> Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override string Name => "نرخ حفظ ماهانه مشتریان";
            protected override string WhereCondition => $"monthdiff({nameof(SqlVwMarketingsubscriptionmonthly.Monthstart)},getdate()) >= 0 && monthdiff({nameof(SqlVwMarketingsubscriptionmonthly.Monthstart)},getdate()) < 12";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(SqlVwMarketingsubscriptionmonthly.Monthstart), "ماه");
                Average(nameof(SqlVwMarketingsubscriptionmonthly.Retentionrate), "نرخ حفظ (درصد)");
                Sum(nameof(SqlVwMarketingsubscriptionmonthly.Retainedshopcount), "مشتری حفظ‌شده");
            }
        }
    }
}
