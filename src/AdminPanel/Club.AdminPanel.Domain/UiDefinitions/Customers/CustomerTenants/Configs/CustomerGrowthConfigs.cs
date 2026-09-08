namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Customer Growth Reports
        // =====================================================

        /// <summary>
        /// گزارش رشد مشتریان به تفکیک ماه
        /// </summary>
        public class CustomerGrowthByMonthConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager];
            protected override string Name => "رشد مشتریان ماهانه";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.CreateDate)})", "ماه");
                Count(null, "تعداد مشتریان جدید");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش رشد تجمعی مشتریان
        /// </summary>
        public class CumulativeCustomerGrowthConfig() : ChartConfigDefinition(ChartType.Area)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager];
            protected override string Name => "رشد تجمعی مشتریان";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.CreateDate)})", "ماه");
                Count(null, "تعداد کل");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش نرخ رشد مشتریان
        /// </summary>
        public class CustomerGrowthRateConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Analyst];
            protected override string Name => "نرخ رشد مشتریان";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.CreateDate)})", "ماه");
                Count(null, "تعداد مشتریان جدید");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش مقایسه مشتریان جدید با مشتریان از دست رفته
        /// </summary>
        public class NewVsChurnedCustomersConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager];
            protected override string Name => "مشتریان جدید در مقابل از دست رفته";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.CreateDate)})", "ماه");
                Count(null, "مشتریان جدید");
            }
        }

        /// <summary>
        /// گزارش توزیع مشتریان فعال در مقابل غیرفعال
        /// </summary>
        public class ActiveVsInactiveCustomersConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager];
            protected override string Name => "مشتریان فعال/غیرفعال";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.IsActive), "وضعیت");
                Count(null, "تعداد");
            }
        }

        /// <summary>
        /// گزارش مشتریان جدید به تفکیک هفته
        /// </summary>
        public class NewCustomersByWeekConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager];
            protected override string Name => "مشتریان جدید هفتگی";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Week({nameof(CustomerTransaction.CreateDate)})", "هفته");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش مشتریان جدید امروز/این هفته/این ماه
        /// </summary>
        public class NewCustomersTimeRangeConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager];
            protected override string Name => "مشتریان جدید بازه زمانی";

            protected override void DefineGroupBy()
            {
                Count(null, "تعداد");
            }
        }
    }
}