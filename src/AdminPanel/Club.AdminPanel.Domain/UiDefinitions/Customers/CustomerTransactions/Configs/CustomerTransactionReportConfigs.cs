using Club.Domain.Entities.Customers.Enums;

namespace Club.AdminPanel.Domain.UiDefinitions.Customers;

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
            protected override List<string> Roles => ClubRoles.RolesManager;
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
        /// گزارش توزیع تراکنش‌ها به تفکیک نوع
        /// </summary>
        public class TransactionsByTypeConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
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
			protected override List<string> Roles => ClubRoles.RolesAnalyst;

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
			protected override List<string> Roles => ClubRoles.RolesManager;

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
			protected override List<string> Roles => ClubRoles.RolesManager;
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
			protected override List<string> Roles => ClubRoles.RolesAnalyst;

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
            protected override List<string> Roles => ClubRoles.RolesManager;
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
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
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
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
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
            protected override List<string> Roles => ClubRoles.RolesManager;
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
        /// گزارش عملکرد پویش‌های امتیازدهی
        /// </summary>
        public class ScoringPromotionsPerformanceConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string Name => "عملکرد پویش‌های امتیازدهی";
            protected override string WhereCondition => $"{nameof(CustomerTransaction.Promotion)}.{nameof(Promotion.Category)} == {(int)PromotionCategory.RewardsAndPointsPrograms}";
            

            protected override void DefineColumns()
            {
                DisplayColumn($"{nameof(CustomerTransaction.Promotion)}.{nameof(Promotion.Title)}", "پویش");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTransactionDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش تعداد اجرای پویش‌های امتیازدهی
        /// </summary>
        public class ScoringPromotionExecutionsConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
            protected override string Name => "تعداد اجرای پویش‌های امتیازدهی";
            protected override string WhereCondition => $"{nameof(CustomerTransaction.Promotion)}.{nameof(Promotion.Category)} == {(int)PromotionCategory.RewardsAndPointsPrograms}";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.Promotion), "پویش");
                Count(null, "تعداد اجرا");
                Sum(nameof(CustomerTransaction.Credit), "مجموع امتیازات");
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
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
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
        /// گزارش تراکنش‌های مرتبط با پویش‌های امتیازدهی
        /// </summary>
        public class TransactionsByPromotionConfig() : GroupByConfigDefinition
        {
			protected override string Name => "تراکنش‌ها به تفکیک پویش امتیازدهی";
			protected override List<string> Roles => ClubRoles.RolesAnalyst;
            protected override string WhereCondition => $"{nameof(CustomerTransaction.Promotion)}.{nameof(Promotion.Category)} == {(int)PromotionCategory.RewardsAndPointsPrograms}";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTransaction.Promotion), "پویش امتیازدهی");
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
                [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst, ClubRoles.CallCenterManager];
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
                DisplayColumn(nameof(CustomerTransaction.Promotion), "پویش امتیازدهی");
                DisplayColumn(nameof(CustomerTransaction.Reward), "ریوارد");
                DisplayColumn(nameof(CustomerTransaction.CreateDate), "تاریخ ایجاد");
                DisplayColumn(nameof(CustomerTransaction.VisitedAt), "تاریخ بازدید");
            }
        }
    }
}
