namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerSegments;

public partial class CustomerSegmentUiDefinitions
{
    public new partial class PublicReport: CRUDDefinition.PublicReport
    {
        // =====================================================
        // Segment Analytics Reports
        // =====================================================

        /// <summary>
        /// گزارش توزیع جوامع/بازارهای مشتریان
        /// </summary>
        public class SegmentDistributionConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "توزیع جوامع/بازارهای مشتریان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                Sum(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش مقایسه اندازه تخمینی و واقعی جوامع/بازارها
        /// </summary>
        public class SegmentSizeComparisonConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "مقایسه اندازه جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                Sum(nameof(CustomerSegment.EstimatedSize), "تخمینی");
                Sum(nameof(CustomerSegment.ActualSize), "واقعی");
            }
        }

        /// <summary>
        /// گزارش نرخ رشد جوامع/بازارها
        /// </summary>
        public class SegmentGrowthRateConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "نرخ رشد جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                Average(nameof(CustomerSegment.GrowthRate), "نرخ رشد (%)");
                Sum(nameof(CustomerSegment.ActualSize), "تعداد فعلی");
            }
        }

        /// <summary>
        /// گزارش نرخ حفظ مشتریان جوامع/بازارها
        /// </summary>
        public class SegmentRetentionRateConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "نرخ حفظ مشتریان جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                Average(nameof(CustomerSegment.RetentionRate), "نرخ حفظ (%)");
                Sum(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش نرخ تعامل جوامع/بازارها
        /// </summary>
        public class SegmentEngagementRateConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "نرخ تعامل جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                Average(nameof(CustomerSegment.EngagementRate), "نرخ تعامل (%)");
                Sum(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش جوامع/بازارها به تفکیک محدوده امتیاز عضویت
        /// </summary>
        public class SegmentsByMembershipScoreConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "جوامع/بازارها بر اساس امتیاز عضویت";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                DisplayColumn(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش آخرین محاسبه جوامع/بازارها
        /// </summary>
        public class SegmentLastCalculationConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "آخرین محاسبه جوامع/بازارها";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                DisplayColumn(nameof(CustomerSegment.LastCalculationDate), "آخرین محاسبه");
                DisplayColumn(nameof(CustomerSegment.CalculationIntervalDays), "فاصله محاسبه (روز)");
                DisplayColumn(nameof(CustomerSegment.ActualSize), "تعداد فعلی");
            }
        }

        /// <summary>
        /// گزارش عملکرد کلی جوامع/بازارها
        /// </summary>
        public class SegmentPerformanceOverviewConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "عملکرد کلی جوامع/بازارها";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerSegment.Title), "عنوان جامعه/بازار");
                DisplayColumn(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
                DisplayColumn(nameof(CustomerSegment.GrowthRate), "نرخ رشد");
                DisplayColumn(nameof(CustomerSegment.RetentionRate), "نرخ حفظ");
                DisplayColumn(nameof(CustomerSegment.EngagementRate), "نرخ تعامل");
            }
        }

        // =====================================================
        // KPI Configs for Dashboard
        // =====================================================

        /// <summary>
        /// KPI: تعداد کل جوامع/بازارهای فعال
        /// </summary>
        public class TotalActiveCommunitiesConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "تعداد کل جوامع/بازارهای فعال";

            protected override void DefineGroupBy()
            {
                Count(null, string.Empty);
            }
        }

        /// <summary>
        /// KPI: تعداد کل اعضای جوامع/بازارها
        /// </summary>
        public class TotalCommunityMembersConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "تعداد کل اعضای جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                Sum(nameof(CustomerSegment.ActualSize), string.Empty);
            }
        }

        /// <summary>
        /// KPI: میانگین اندازه جوامع/بازارها
        /// </summary>
        public class AverageCommunitySizeConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "میانگین اندازه جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                Average(nameof(CustomerSegment.ActualSize), string.Empty);
            }
        }

        /// <summary>
        /// KPI: میانگین نرخ رشد جوامع/بازارها
        /// </summary>
        public class AverageGrowthRateConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerSegment.IsActive)} == true";
            protected override string Name => "میانگین نرخ رشد جوامع/بازارها";

            protected override void DefineGroupBy()
            {
                Average(nameof(CustomerSegment.GrowthRate), string.Empty);
            }
        }
    }
}
