using Hyper.Domain.Entities.Customers.Enums;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers;

public partial class CustomerTransactionUiDefinitions
{
    public new class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Transaction Analytics Reports
        // =====================================================

        /// <summary>
        /// گزارش تراکنش‌ها به تفکیک ماه
        /// </summary>
        public class TransactionsByMonthConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "تراکنش‌های ماهانه";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.CreateDate)})", "ماه");
                Count(null, "تعداد تراکنش‌ها");
                Sum(nameof(CustomerTransaction.Credit), "مجموع بستانکار");
                Sum(nameof(CustomerTransaction.Debit), "مجموع بدهکار");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// تقویم تراکنش‌های روزانه - نمایش تعداد تراکنش‌ها برای هر روز
        /// </summary>
        public class TransactionsByDayCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "تقویم تراکنش‌های روزانه";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"DATE({nameof(CustomerTransaction.CreateDate)})", "تاریخ");
                Count(null, "تعداد تراکنش‌ها");
                Sum(nameof(CustomerTransaction.Credit), "مجموع اعتبار");
                Sum(nameof(CustomerTransaction.Debit), "مجموع برداشت");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش توزیع تراکنش‌ها به تفکیک نوع
        /// </summary>
        public class TransactionsByTypeConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "تراکنش‌ها به تفکیک نوع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.TransactionType), "نوع تراکنش");
                Count(null, "تعداد");
                Sum(nameof(CustomerTransaction.Balance), "مجموع مانده");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش بیشترین تراکنش‌ها به تفکیک مشتری
        /// </summary>
        public class TopCustomersByTransactionsConfig() : GroupByConfigDefinition
        {
			protected override string Name => "مشتریان با بیشترین تراکنش";
			protected override List<string> Roles => HyperRoles.RolesAnalyst;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.CustomerTenant), "مشتری");
                Count(null, "تعداد تراکنش‌ها");
                Sum(nameof(CustomerTransaction.Credit), "مجموع اعتبار");
                Sum(nameof(CustomerTransaction.Debit), "مجموع برداشت");

                OrderByDesc("COUNT"); // نزولی - بیشترین تراکنش
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش موجودی امتیازات مشتریان
        /// </summary>
        public class CustomerPointsBalanceConfig() : ChartConfigDefinition(ChartType.Pie)
        {
			protected override string Name => "موجودی امتیازات مشتریان";
			protected override List<string> Roles => HyperRoles.RolesManager;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.CustomerTenant), "مشتری");
                Sum(nameof(CustomerTransaction.Balance), "مجموع موجودی");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش تراکنش‌های بازدید نشده
        /// </summary>
        public class UnvisitedTransactionsConfig() : ChartConfigDefinition(ChartType.Bar)
        {
			protected override string Name => "تراکنش‌های بازدید نشده";
			protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerTransaction.VisitedAt)} == null";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.TransactionType), "نوع تراکنش");
                Count(null, "تعداد");
                Sum(nameof(CustomerTransaction.Credit), "مجموع اعتبار");
                Sum(nameof(CustomerTransaction.Debit), "مجموع برداشت");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        // =====================================================
        // Operational Metrics
        // =====================================================

        /// <summary>
        /// گزارش آمار امتیازات اعطا شده
        /// </summary>
        public class PointsIssuedStatsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
			protected override string Name => "آمار امتیازات اعطا شده";
			protected override List<string> Roles => HyperRoles.RolesAnalyst;

            protected override void DefineGroupBy()
            {
                Sum(nameof(CustomerTransaction.Credit));
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش آمار امتیازات مصرف شده
        /// </summary>
        public class PointsRedeemedStatsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "آمار امتیازات مصرف شده";

            protected override void DefineGroupBy()
            {
                Sum(nameof(CustomerTransaction.Debit));
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش موجودی کل امتیازات
        /// </summary>
        public class TotalPointsBalanceConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "موجودی کل امتیازات";

            protected override void DefineGroupBy()
            {
                Sum(nameof(CustomerTransaction.Balance));
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش نسبت امتیازات اعطا شده به مصرف شده
        /// </summary>
        public class PointsIssuedVsRedeemedConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "امتیازات اعطا شده در مقابل مصرف شده";

            protected override void DefineGroupBy()
            {
                Sum(nameof(CustomerTransaction.Credit), "اعطا شده");
                Sum(nameof(CustomerTransaction.Debit), "مصرف شده");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش امتیازات به تفکیک نوع
        /// </summary>
        public class PointsByTypeConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "امتیازات به تفکیک نوع";

            protected override void DefineGroupBy()
            {
                // Group by Point type to show distribution
                GroupBy(nameof(CustomerTransaction.Point), "نوع امتیاز");
                Sum(nameof(CustomerTransaction.Balance), "مجموع موجودی");
                // Also show count for better visualization
                Count(null, "تعداد تراکنش‌ها");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش میانگین ارزش تراکنش
        /// </summary>
        public class AverageTransactionValueConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override string Name => "میانگین ارزش تراکنش";

            protected override void DefineGroupBy()
            {
                Average(nameof(CustomerTransaction.Credit));
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش تراکنش‌های مرتبط با پویش‌های
        /// </summary>
        public class TransactionsByPromotionConfig() : GroupByConfigDefinition
        {
			protected override string Name => "تراکنش‌ها به تفکیک پویش";
			protected override List<string> Roles => HyperRoles.RolesAnalyst;
            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.Promotion), "پویش");
                Count(null, "تعداد تراکنش‌ها");
                Sum(nameof(CustomerTransaction.Credit), "مجموع امتیازات اعطا شده");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }
        /// <summary>
        /// لیست استاندارد جزئیات تراکنش برای استفاده در زیرگزارش‌ها
        /// </summary>
        public class CustomerTransactionDetailsListConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles =>
                [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst, HyperRoles.CallCenterManager];
            protected override string Name => "جزئیات تراکنش‌ها";
            

            // رنگ‌بندی بر اساس نوع تراکنش
            protected override string TableSuccess => $"{nameof(CustomerTransaction.TransactionType)} == {(int)CustomerTransactionType.Credit}";
            protected override string TableWarning => $"{nameof(CustomerTransaction.TransactionType)} == {(int)CustomerTransactionType.Debit}";
            protected override string TableInfo => $"{nameof(CustomerTransaction.VisitedAt)} != null";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTransaction.CustomerTenant), "مشتری");
                DisplayColumn(nameof(CustomerTransaction.TransactionType), "نوع تراکنش");
                DisplayColumn(nameof(CustomerTransaction.Credit), "اعتبار");
                DisplayColumn(nameof(CustomerTransaction.Debit), "برداشت");
                DisplayColumn(nameof(CustomerTransaction.Balance), "مانده");
                DisplayColumn(nameof(CustomerTransaction.Point), "نوع امتیاز");
                DisplayColumn(nameof(CustomerTransaction.ActivePlan), "طرح فعال");
                DisplayColumn(nameof(CustomerTransaction.EventChannel), "کانال");
                DisplayColumn(nameof(CustomerTransaction.Promotion), "پویش");
                DisplayColumn(nameof(CustomerTransaction.Reward), "ریوارد");
                DisplayColumn(nameof(CustomerTransaction.CreateDate), "تاریخ ایجاد");
                DisplayColumn(nameof(CustomerTransaction.VisitedAt), "تاریخ بازدید");
            }
        }
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
                GroupBy(nameof(CustomerTransaction.EventChannel), "کانال", true, ConfiguredReport.ReportMatrixType.Horizontal);
                GroupBy(nameof(CustomerTransaction.Reward), "پاداش", true, ConfiguredReport.ReportMatrixType.Vertical);
                GroupBy(nameof(CustomerTransaction.ActivePlan), "طرح", true, ConfiguredReport.ReportMatrixType.Vertical);

                Count(null, "تعداد");
                Sum(nameof(CustomerTransaction.Debit), "مجموع امتیاز مصرف‌شده");
                Average(nameof(CustomerTransaction.Debit), "میانگین امتیاز به ازای هر واحد");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<RewardChannelPlanPivotDetailsConfig>();
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
                DisplayColumn(nameof(CustomerTransaction.CreateDate), "تاریخ");
                DisplayColumn(nameof(CustomerTransaction.CustomerTenant));
                DisplayColumn(nameof(CustomerTransaction.Point));
                DisplayColumn(nameof(CustomerTransaction.TransactionType));
                DisplayColumn(nameof(CustomerTransaction.Credit));
                DisplayColumn(nameof(CustomerTransaction.Debit));
                DisplayColumn(nameof(CustomerTransaction.Balance));
                //DisplayColumn(nameof(CustomerTransaction.ActivePlan));
            }
        }
    }
}
