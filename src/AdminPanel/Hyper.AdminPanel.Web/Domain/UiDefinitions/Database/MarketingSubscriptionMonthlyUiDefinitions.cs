using Hyper.Domain.Entities.Database;
using Neo.Bpms.Domain.Features.MetaDefinitions.Reports;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;

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

public sealed class SqlVwMarketinggmvmonthlyUiDefinitions : CRUDDefinition<SqlVwMarketinggmvmonthly>
{
    public override List<string>? Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        public class GmvConfig : ChartConfigDefinition { public GmvConfig() : base(ChartType.Line) { } protected override List<string> Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst]; protected override string Name => "GMV ماهانه خرید و فروش"; protected override void DefineGroupBy() { GroupBy(nameof(SqlVwMarketinggmvmonthly.Monthstart), "ماه"); Sum(nameof(SqlVwMarketinggmvmonthly.Purchasegmv), "GMV خرید"); Sum(nameof(SqlVwMarketinggmvmonthly.Salegmv), "GMV فروش"); } }
        public class CountConfig : ChartConfigDefinition { public CountConfig() : base(ChartType.Line) { } protected override List<string> Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst]; protected override string Name => "تعداد فاکتور و کالای خرید و فروش"; protected override void DefineGroupBy() { GroupBy(nameof(SqlVwMarketinggmvmonthly.Monthstart), "ماه"); Sum(nameof(SqlVwMarketinggmvmonthly.Purchaseinvoicecount), "فاکتور خرید"); Sum(nameof(SqlVwMarketinggmvmonthly.Purchaseitemcount), "قلم خرید"); Sum(nameof(SqlVwMarketinggmvmonthly.Saleinvoicecount), "فاکتور فروش"); Sum(nameof(SqlVwMarketinggmvmonthly.Saleitemcount), "قلم فروش"); } }
    }
}
