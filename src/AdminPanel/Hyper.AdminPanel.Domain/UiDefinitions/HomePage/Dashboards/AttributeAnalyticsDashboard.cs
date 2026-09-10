/*
using Hyper.AdminPanel.Domain.UiDefinitions.Attributes;
using Hyper.AdminPanel.Domain.UiDefinitions.Attributes.Base;
using static Hyper.AdminPanel.Domain.UiDefinitions.Attributes.TenantAttributeDailyAggregationUiDefinitions;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// داشبورد تحلیل ویژگی‌های Tenant - نمایش موارد aggregated و تحلیلی
        /// </summary>
        public class AttributeAnalyticsDashboard : DashboardConfigDefinition
        {
            protected override string Title => "تحلیل ویژگی‌ها";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override bool IsDefault => false;
            protected override string Icon => "chart-mixed";

            // =====================================================
            // Daily Aggregation Widgets
            // =====================================================

            /// <summary>
            /// تقویم روزانه - نمایش مقادیر روزانه
            /// </summary>
            public class DailyAggregationCalendarWidget :
                WidgetDailyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PublicReport.DailyAggregationCalendarConfig>
            {
                public override string Title => "تقویم تجمیع‌های روزانه";
                protected override int? HeightInPixels => 360;
                public override int Width => 6;
            }

            /// <summary>
            /// روند روزانه - نمودار خطی
            /// </summary>
            public class DailyAggregationTrendWidget :
                WidgetDailyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PublicReport.DailyAggregationTrendConfig>
            {
                public override string Title => "روند روزانه ویژگی‌ها";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// توزیع روزانه به تفکیک ویژگی
            /// </summary>
            public class DailyAggregationDistributionWidget :
                WidgetDailyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PublicReport.DailyAggregationDistributionConfig>
            {
                public override string Title => "توزیع مقادیر روزانه";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// تحلیل روزانه به تفکیک کانال
            /// </summary>
            public class DailyAggregationByChannelWidget :
                WidgetDailyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PublicReport.DailyAggregationByChannelConfig>
            {
                public override string Title => "تجمیع روزانه به تفکیک کانال";
                protected override int? MaxRecordCount => 8;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// توزیع روزانه به تفکیک محصول
            /// </summary>
            public class DailyAggregationByProductWidget :
                WidgetDailyAggregationBase<TenantAttributeDailyAggregationUiDefinitions.PublicReport.DailyAggregationByProductConfig>
            {
                public override string Title => "توزیع روزانه به تفکیک محصول";
                protected override int? MaxRecordCount => 6;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            // =====================================================
            // Persian Monthly Aggregation Widgets
            // =====================================================

            /// <summary>
            /// تقویم ماهانه شمسی
            /// </summary>
            public class MonthlyPersianCalendarWidget :
                WidgetPersianMonthlyAggregationBase<PersianMonthlyAggregationReport.MonthlyPersianCalendarConfig>
            {
                public override string Title => "تقویم تجمیع‌های ماهانه (شمسی)";
                protected override int? HeightInPixels => 360;
                public override int Width => 6;
            }

            /// <summary>
            /// روند ماهانه شمسی
            /// </summary>
            public class MonthlyPersianTrendWidget :
                WidgetPersianMonthlyAggregationBase<PersianMonthlyAggregationReport.MonthlyPersianTrendConfig>
            {
                public override string Title => "روند ماهانه (شمسی)";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// توزیع ماهانه شمسی
            /// </summary>
            public class MonthlyPersianDistributionWidget :
                WidgetPersianMonthlyAggregationBase<PersianMonthlyAggregationReport.MonthlyPersianDistributionConfig>
            {
                public override string Title => "توزیع ماهانه (شمسی)";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// تحلیل ماهانه شمسی به تفکیک جامعه
            /// </summary>
            public class MonthlyPersianBySegmentWidget :
                WidgetPersianMonthlyAggregationBase<PersianMonthlyAggregationReport.MonthlyPersianBySegmentConfig>
            {
                public override string Title => "تجمیع ماهانه به تفکیک جامعه (شمسی)";
                protected override int? MaxRecordCount => 8;
                protected override int? HeightInPixels => 280;
                public override int Width => 12;
            }

            // =====================================================
            // Gregorian Monthly Aggregation Widgets
            // =====================================================

            /// <summary>
            /// تقویم ماهانه میلادی
            /// </summary>
            public class MonthlyGregorianCalendarWidget : WidgetGregorianMonthlyAggregationBase<GregorianMonthlyAggregationReport.MonthlyGregorianCalendarConfig>
            {
                public override string Title => "تقویم تجمیع‌های ماهانه (میلادی)";
                protected override int? HeightInPixels => 360;
                public override int Width => 6;
            }

            /// <summary>
            /// روند ماهانه میلادی
            /// </summary>
            public class MonthlyGregorianTrendWidget : WidgetGregorianMonthlyAggregationBase<GregorianMonthlyAggregationReport.MonthlyGregorianTrendConfig>
            {
                public override string Title => "روند ماهانه (میلادی)";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// توزیع ماهانه میلادی
            /// </summary>
            public class MonthlyGregorianDistributionWidget : WidgetGregorianMonthlyAggregationBase<GregorianMonthlyAggregationReport.MonthlyGregorianDistributionConfig>
            {
                public override string Title => "توزیع ماهانه (میلادی)";
                protected override int? MaxRecordCount => 10;
                protected override int? HeightInPixels => 280;
                public override int Width => 6;
            }

            /// <summary>
            /// تحلیل ماهانه میلادی به تفکیک دسته‌بندی
            /// </summary>
            public class MonthlyGregorianByCategoryWidget : WidgetGregorianMonthlyAggregationBase<GregorianMonthlyAggregationReport.MonthlyGregorianByCategoryConfig>
            {
                public override string Title => "تجمیع ماهانه به تفکیک دسته‌بندی (میلادی)";
                protected override int? MaxRecordCount => 8;
                protected override int? HeightInPixels => 280;
                public override int Width => 12;
            }
        }
    }
}

*/