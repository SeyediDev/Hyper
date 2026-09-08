namespace Hyper.AdminPanel.Domain.UiDefinitions.Attributes;

/// <summary>
/// تعاریف BI برای تجمیع‌های روزانه و ماهانه ویژگی‌های tenant
/// </summary>
public partial class TenantAttributeMonthlyAggregationPersianUiDefinitions
    : CRUDDefinition<TenantAttributeMonthlyAggregationPersian>
{
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Persian Monthly Aggregation Charts
        // =====================================================

        /// <summary>
        /// تقویم تجمیع‌های ماهانه (شمسی)
        /// </summary>
        public class MonthlyPersianCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تقویم تجمیع‌های ماهانه (شمسی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.MonthStartDate), "روز ابتدای ماه");
                //GroupByFormula($"DATEFROMPARTS({nameof(TenantAttributeMonthlyAggregationPersian.ShamsiYear)}, {nameof(TenantAttributeMonthlyAggregationPersian.ShamsiMonth)}, 1)", "ماه شمسی");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع مقادیر");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Count), "تعداد مقادیر");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
            }
        }

        /// <summary>
        /// روند ماهانه (شمسی)
        /// </summary>
        public class MonthlyPersianTrendConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "روند ماهانه (شمسی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.ShamsiYear));
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.ShamsiMonth));
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
            }
        }

        /// <summary>
        /// توزیع ماهانه (شمسی)
        /// </summary>
        public class MonthlyPersianDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "توزیع ماهانه (شمسی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.Attribute), "ویژگی");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
            }
        }

        /// <summary>
        /// تحلیل ماهانه به تفکیک جامعه (شمسی)
        /// </summary>
        public class MonthlyPersianBySegmentConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تجمیع ماهانه به تفکیک جامعه (شمسی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.Segment), "جامعه/بازار");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع مقادیر");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
            }
        }

        /// <summary>
        /// آمار ماهانه (شمسی)
        /// </summary>
        public class MonthlyPersianStatsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "آمار کلی (شمسی)";

            protected override void DefineGroupBy()
            {
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Count), "کل رویدادها");
            }
        }
    }
}
