using Hyper.Domain.Entities.Rewards.Views;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Rewards.Views;

/// <summary>
/// UiDefinition برای View گزارش Pivot: پاداش × کانال × طرح
/// </summary>
public class RewardChannelPlanPivotSourceViewUiDefinitions : CRUDDefinition<RewardChannelPlanPivotSourceView>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Analyst,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(
            nameof(RewardChannelPlanPivotSourceView.TxDate),
            nameof(RewardChannelPlanPivotSourceView.RewardTitle),
            nameof(RewardChannelPlanPivotSourceView.ChannelTitle),
            nameof(RewardChannelPlanPivotSourceView.PlanTitle),
            nameof(RewardChannelPlanPivotSourceView.Qty),
            nameof(RewardChannelPlanPivotSourceView.PointsSpent),
            nameof(RewardChannelPlanPivotSourceView.AvgPointsPerUnit)
        );
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

		/// <summary>
		/// گزارش Pivot: پاداش × کانال × طرح
		/// ماتریس کامل فروش پاداشها به تفکیک کانال و طرح فعال مشتری
		/// </summary>
		public class RewardChannelPlanPivotConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "تقاطع کانال و پاداش/طرح";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
				GroupBy(nameof(RewardChannelPlanPivotSourceView.ChannelTitle), "کانال", true, ConfiguredReport.ReportMatrixType.Horizontal);
				GroupBy(nameof(RewardChannelPlanPivotSourceView.RewardTitle), "پاداش", true, ConfiguredReport.ReportMatrixType.Vertical);
				GroupBy(nameof(RewardChannelPlanPivotSourceView.PlanTitle), "طرح", true, ConfiguredReport.ReportMatrixType.Vertical);

				Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
                Sum(nameof(RewardChannelPlanPivotSourceView.PointsSpent), "مجموع امتیاز مصرف‌شده");
                Average(nameof(RewardChannelPlanPivotSourceView.AvgPointsPerUnit), "میانگین امتیاز به ازای هر واحد");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<RewardChannelPlanPivotDetailsConfig>();
            }
        }

		/// <summary>
		/// گزارش Pivot: پاداش × کانال (بدون طرح)
		/// </summary>
		public class RewardChannelPivotConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "Pivot پاداش × کانال";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.RewardTitle), "پاداش");
                LayoutColumn(false, nameof(RewardChannelPlanPivotSourceView.RewardTitle), ConfiguredReport.ReportMatrixType.Vertical);

                GroupBy(nameof(RewardChannelPlanPivotSourceView.ChannelTitle), "کانال");
                LayoutColumn(false, nameof(RewardChannelPlanPivotSourceView.ChannelTitle), ConfiguredReport.ReportMatrixType.Horizontal);

                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
                Sum(nameof(RewardChannelPlanPivotSourceView.PointsSpent), "مجموع امتیاز مصرف‌شده");
            }
        }

		/// <summary>
		/// گزارش Pivot: پاداش × طرح (بدون کانال)
		/// </summary>
		public class RewardPlanPivotConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "Pivot پاداش × طرح";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.RewardTitle), "پاداش");
                LayoutColumn(false, nameof(RewardChannelPlanPivotSourceView.RewardTitle), ConfiguredReport.ReportMatrixType.Vertical);

                GroupBy(nameof(RewardChannelPlanPivotSourceView.PlanTitle), "طرح");
                LayoutColumn(false, nameof(RewardChannelPlanPivotSourceView.PlanTitle), ConfiguredReport.ReportMatrixType.Horizontal);

                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
                Sum(nameof(RewardChannelPlanPivotSourceView.PointsSpent), "مجموع امتیاز مصرف‌شده");
            }
        }

		/// <summary>
		/// گزارش Pivot: کانال × طرح (بدون پاداش)
		/// </summary>
		public class ChannelPlanPivotConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "Pivot کانال × طرح";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.ChannelTitle), "کانال");
                LayoutColumn(false, nameof(RewardChannelPlanPivotSourceView.ChannelTitle), ConfiguredReport.ReportMatrixType.Vertical);

                GroupBy(nameof(RewardChannelPlanPivotSourceView.PlanTitle), "طرح");
                LayoutColumn(false, nameof(RewardChannelPlanPivotSourceView.PlanTitle), ConfiguredReport.ReportMatrixType.Horizontal);

                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
                Sum(nameof(RewardChannelPlanPivotSourceView.PointsSpent), "مجموع امتیاز مصرف‌شده");
            }
        }

		/// <summary>
		/// گزارش نمودار: فروش پاداشها به تفکیک کانال
		/// </summary>
		public class RewardsByChannelChartConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "فروش پاداشها به تفکیک کانال";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.RewardTitle), "پاداش");
                GroupBy(nameof(RewardChannelPlanPivotSourceView.ChannelTitle), "کانال");
                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
            }
        }

		/// <summary>
		/// گزارش نمودار: فروش پاداشها به تفکیک طرح
		/// </summary>
		public class RewardsByPlanChartConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "فروش پاداشها به تفکیک طرح";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.RewardTitle), "پاداش");
                GroupBy(nameof(RewardChannelPlanPivotSourceView.PlanTitle), "طرح");
                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
            }
        }

        /// <summary>
        /// گزارش نمودار: توزیع فروش بر اساس کانال
        /// </summary>
        public class SalesByChannelDistributionConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "توزیع فروش بر اساس کانال";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.ChannelTitle), "کانال");
                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
            }
        }

        /// <summary>
        /// گزارش نمودار: توزیع فروش بر اساس طرح
        /// </summary>
        public class SalesByPlanDistributionConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "توزیع فروش بر اساس طرح";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.PlanTitle), "طرح");
                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
            }
        }

        /// <summary>
        /// گزارش روند فروش در طول زمان
        /// </summary>
        public class SalesTrendOverTimeConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "روند فروش در طول زمان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(RewardChannelPlanPivotSourceView.TxDate), "تاریخ");
                Sum(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
                Sum(nameof(RewardChannelPlanPivotSourceView.PointsSpent), "مجموع امتیاز مصرف‌شده");
            }
        }

        /// <summary>
        /// لیست جزئیات Pivot برای استفاده در زیرگزارش‌ها
        /// </summary>
        public class RewardChannelPlanPivotDetailsConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "جزئیات Pivot";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(RewardChannelPlanPivotSourceView.TxDate), "تاریخ");
                DisplayColumn(nameof(RewardChannelPlanPivotSourceView.Qty), "تعداد خرید");
                DisplayColumn(nameof(RewardChannelPlanPivotSourceView.PointsSpent), "مجموع امتیاز مصرف‌شده");
                DisplayColumn(nameof(RewardChannelPlanPivotSourceView.AvgPointsPerUnit), "میانگین امتیاز به ازای هر واحد");
            }
        }
    }
}

