namespace Hyper.AdminPanel.Domain.UiDefinitions.Attributes;

/// <summary>
/// تعاریف BI برای تجمیع‌های روزانه و ماهانه ویژگی‌های tenant
/// </summary>
public partial class TenantAttributeDailyAggregationUiDefinitions
    : CRUDDefinition<TenantAttributeDailyAggregation>
{
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Daily Aggregation Charts
        // =====================================================

        /// <summary>
        /// تقویم تجمیع‌های روزانه ویژگی‌ها
        /// </summary>
        public class DailyAggregationCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تقویم تجمیع‌های روزانه";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeDailyAggregation.DateTime), "تاریخ");
                Sum(nameof(TenantAttributeDailyAggregation.Sum), "مجموع مقادیر");
                Sum(nameof(TenantAttributeDailyAggregation.Count), "تعداد مقادیر");
                Average(nameof(TenantAttributeDailyAggregation.Average), "میانگین");
            }
        }

        /// <summary>
        /// روند روزانه ویژگی‌ها (نمودار خطی)
        /// </summary>
        public class DailyAggregationTrendConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "روند روزانه ویژگی‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeDailyAggregation.DateTime), "تاریخ");
                Sum(nameof(TenantAttributeDailyAggregation.Sum), "مجموع");
                Average(nameof(TenantAttributeDailyAggregation.Average), "میانگین");
                Count(null, "تعداد رویدادها");
            }
        }

        /// <summary>
        /// توزیع مقادیر روزانه ویژگی‌ها
        /// </summary>
        public class DailyAggregationDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "توزیع مقادیر روزانه";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeDailyAggregation.Attribute), "ویژگی");
                Sum(nameof(TenantAttributeDailyAggregation.Sum), "مجموع");
                Sum(nameof(TenantAttributeDailyAggregation.Count), "تعداد");
                Average(nameof(TenantAttributeDailyAggregation.Average), "میانگین");
            }
        }

        /// <summary>
        /// تحلیل روزانه به تفکیک کانال
        /// </summary>
        public class DailyAggregationByChannelConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تجمیع روزانه به تفکیک کانال";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeDailyAggregation.Channel), "کانال");
                Sum(nameof(TenantAttributeDailyAggregation.Sum), "مجموع مقادیر");
                Average(nameof(TenantAttributeDailyAggregation.Average), "میانگین");
                Sum(nameof(TenantAttributeDailyAggregation.Count), "تعداد رویدادها");
            }
        }

        /// <summary>
        /// تحلیل روزانه به تفکیک محصول
        /// </summary>
        public class DailyAggregationByProductConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "توزیع روزانه به تفکیک محصول";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeDailyAggregation.Product), "محصول");
                Sum(nameof(TenantAttributeDailyAggregation.Sum), "مجموع");
                Sum(nameof(TenantAttributeDailyAggregation.Count), "تعداد");
            }
        }

        /// <summary>
        /// آمار روزانه (KPI)
        /// </summary>
        public class DailyAggregationStatsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "آمار کلی تجمیع‌های روزانه";

            protected override void DefineGroupBy()
            {
                Sum(nameof(TenantAttributeDailyAggregation.Count), "تعداد رویدادها");
            }
        }
    }

    public partial class PersianMonthlyAggregationReport
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
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.MonthStartDate), "روز ابتدای ماه شمسی");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Count), "تعداد");
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
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.ShamsiYear), "سال");
                GroupBy(nameof(TenantAttributeMonthlyAggregationPersian.ShamsiMonth), "ماه");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Count), "تعداد");
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
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Count), "تعداد");
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
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Sum), "مجموع");
                Average(nameof(TenantAttributeMonthlyAggregationPersian.Average), "میانگین");
                Sum(nameof(TenantAttributeMonthlyAggregationPersian.Count), "تعداد");
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

    public partial class GregorianMonthlyAggregationReport
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
                GroupByFormula($"DATEFROMPARTS({nameof(TenantAttributeMonthlyAggregationGregorian.GregorianYear)}, {nameof(TenantAttributeMonthlyAggregationGregorian.GregorianMonth)}, 1)", "ماه میلادی");
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
                GroupByFormula($"{nameof(TenantAttributeMonthlyAggregationGregorian.GregorianYear)} & '-' & {nameof(TenantAttributeMonthlyAggregationGregorian.GregorianMonth)}", "سال-ماه");
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
