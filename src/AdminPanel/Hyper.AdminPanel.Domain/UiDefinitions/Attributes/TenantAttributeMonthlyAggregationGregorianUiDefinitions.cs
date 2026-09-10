namespace Hyper.AdminPanel.Domain.UiDefinitions.Attributes;

/// <summary>
/// تعاریف BI برای تجمیع‌های روزانه و ماهانه ویژگی‌های tenant
/// </summary>
public partial class TenantAttributeMonthlyAggregationGregorianUiDefinitions
    : CRUDDefinition<TenantAttributeMonthlyAggregationGregorian>
{
    public partial class GregorianMonthlyAggregationReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Gregorian Monthly Aggregation Charts
        // =====================================================

        /// <summary>
        /// تقویم تجمیع‌های ماهانه (میلادی)
        /// </summary>
        public class MonthlyGregorianCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تقویم تجمیع‌های ماهانه (میلادی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationGregorian.MonthStartDate), "ماه میلادی");
                //GroupByFormula($"DATEFROMPARTS({nameof(TenantAttributeMonthlyAggregationGregorian.GregorianYear)}, {nameof(TenantAttributeMonthlyAggregationGregorian.GregorianMonth)}, 1)", "ماه میلادی");
                Sum(nameof(TenantAttributeMonthlyAggregationGregorian.Sum), "مجموع مقادیر");
                Sum(nameof(TenantAttributeMonthlyAggregationGregorian.Count), "تعداد مقادیر");
                Average(nameof(TenantAttributeMonthlyAggregationGregorian.Average), "میانگین");
            }
        }

        /// <summary>
        /// روند ماهانه (میلادی)
        /// </summary>
        public class MonthlyGregorianTrendConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "روند ماهانه (میلادی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationGregorian.GregorianYear));
                GroupBy(nameof(TenantAttributeMonthlyAggregationGregorian.GregorianMonth));
                //GroupByFormula($"{nameof(TenantAttributeMonthlyAggregationGregorian.GregorianYear)} & '-' & {nameof(TenantAttributeMonthlyAggregationGregorian.GregorianMonth)}", "سال-ماه");
                Sum(nameof(TenantAttributeMonthlyAggregationGregorian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationGregorian.Average), "میانگین");
            }
        }

        /// <summary>
        /// توزیع ماهانه (میلادی)
        /// </summary>
        public class MonthlyGregorianDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "توزیع ماهانه (میلادی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationGregorian.Attribute), "ویژگی");
                Sum(nameof(TenantAttributeMonthlyAggregationGregorian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationGregorian.Average), "میانگین");
            }
        }

        /// <summary>
        /// تحلیل ماهانه به تفکیک دسته‌بندی محصول (میلادی)
        /// </summary>
        public class MonthlyGregorianByCategoryConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تجمیع ماهانه به تفکیک دسته‌بندی (میلادی)";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeMonthlyAggregationGregorian.ProductCategory), "دسته‌بندی محصول");
                Sum(nameof(TenantAttributeMonthlyAggregationGregorian.Sum), "مجموع مقادیر");
                Average(nameof(TenantAttributeMonthlyAggregationGregorian.Average), "میانگین");
            }
        }

        /// <summary>
        /// آمار ماهانه (میلادی)
        /// </summary>
        public class MonthlyGregorianStatsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "آمار کلی (میلادی)";

            protected override void DefineGroupBy()
            {
                Sum(nameof(TenantAttributeMonthlyAggregationGregorian.Count), "کل رویدادها");
            }
        }
    }
}