namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Customer Lifetime Value (CLV) Reports
        // =====================================================

        /// <summary>
        /// گزارش توزیع CLV مشتریان
        /// </summary>
        public class ClvDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
			protected override string Name => "توزیع ارزش طول عمر مشتریان";
			protected override List<string> Roles => ClubRoles.RolesAnalyst;
            protected override string ChartAdvancedOptions => ChartAdvancedOption.ceil_categories;

            protected override void DefineGroupBy()
            {
				AddCategoryRanges("CLV",
					CategoryRange.LessThan(1090000, "کمتر از 1,090,000"),
					CategoryRange.Closed(1090000, 1110000, "1,090,000 - 1,110,000"),
					CategoryRange.Closed(1110000, 1130000, "1,110,000 - 1,130,000"),
					CategoryRange.GreaterThan(1130000, "بیشتر از 1,130,000")
				);
				// Add GroupBy field with Category property for range categorization
				GroupByWithCategory(
                    nameof(CustomerTenant.CustomerLifetimeValue), 
                    "ارزش طول عمر",
                    "CLV");
                Count(null, "تعداد مشتریان");
                OrderByDesc("COUNT");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<AverageClvByRfmSegmentConfig>(Report.SubReportType.SubReport);
            }
        }

        /// <summary>
        /// گزارش میانگین CLV به تفکیک دسته RFM
        /// </summary>
        public class AverageClvByRfmSegmentConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string Name => "میانگین CLV به تفکیک دسته RFM";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                Average(nameof(CustomerTenant.CustomerLifetimeValue), "میانگین CLV");
                Count(null, "تعداد مشتریان");
                OrderByDesc("COUNT");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<TopCustomersByClvConfig>();
            }
        }

        /// <summary>
        /// گزارش مشتریان با بالاترین CLV
        /// </summary>
        public class TopCustomersByClvConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string Name => "مشتریان با بالاترین CLV";
            
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTenant.Customer), "مشتری");
                DisplayColumn(nameof(CustomerTenant.Tenant), "اکوسیستم");
                DisplayColumn(nameof(CustomerTenant.CustomerLifetimeValue), "CLV");
                DisplayColumn(nameof(CustomerTenant.TotalTransactionValue), "ارزش کل تراکنش‌ها");
                DisplayColumn(nameof(CustomerTenant.AverageOrderValue), "میانگین ارزش سفارش");

                OrderByDesc(nameof(CustomerTenant.CustomerLifetimeValue)); // نزولی - بالاترین CLV
            }
        }

        // =====================================================
        // Customer Retention Reports
        // =====================================================

        /// <summary>
        /// گزارش نرخ حفظ مشتری به تفکیک ماه
        /// </summary>
        public class RetentionRateByMonthConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerTenant.IsActive)} == true";
            protected override string Name => "نرخ حفظ مشتری ماهانه";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.LastModified)})", "ماه");
                Count(null, "تعداد مشتریان فعال");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<CustomerGrowthByMonthConfig>(Report.SubReportType.SubReport);
            }
        }

        /// <summary>
        /// گزارش کوهورت - تحلیل حفظ مشتری
        /// </summary>
        public class CustomerCohortAnalysisConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
            protected override string Name => "تحلیل کوهورت مشتریان";
            protected override void DefineColumns()
            {
                DisplayColumn($"Month({nameof(CustomerTransaction.CreateDate)})", "ماه عضویت");
                DisplayColumn(nameof(CustomerTenant.Id), "تعداد مشتریان");
            }
        }

        // =====================================================
        // Churn Analysis Reports
        // =====================================================

        /// <summary>
        /// گزارش مشتریان در معرض خطر ریزش
        /// </summary>
        public class CustomersAtRiskConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerTenant.ChurnRiskScore)} > 50";
            protected override string Name => "مشتریان در معرض خطر ریزش";

            // رنگ‌بندی بر اساس نمره ریسک ریزش
            protected override string TableDanger => $"{nameof(CustomerTenant.ChurnRiskScore)} >= 80";
            protected override string TableWarning => $"{nameof(CustomerTenant.ChurnRiskScore)} >= 60 && {nameof(CustomerTenant.ChurnRiskScore)} < 80";
            protected override string TableInfo => $"{nameof(CustomerTenant.ChurnRiskScore)} >= 50 && {nameof(CustomerTenant.ChurnRiskScore)} < 60";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTenant.Customer), "مشتری");
                DisplayColumn(nameof(CustomerTenant.Tenant), "اکوسیستم");
                DisplayColumn(nameof(CustomerTenant.ChurnRiskScore), "نمره احتمال ریزش");
                DisplayColumn(nameof(CustomerTenant.DaysSinceLastInteraction), "روز از آخرین تعامل");
                DisplayColumn(nameof(CustomerTenant.CustomerLifetimeValue), "CLV");

                OrderByDesc(nameof(CustomerTenant.ChurnRiskScore)); // نزولی - بالاترین خطر
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش توزیع نمره احتمال ریزش
        /// </summary>
        public class ChurnRiskDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
            protected override string Name => "توزیع احتمال ریزش";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.ChurnRiskScore), "محدوده نمره ریزش");
                Count(null, "تعداد مشتریان");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<CustomersAtRiskConfig>(Report.SubReportType.SubReport);
            }
        }

        /// <summary>
        /// گزارش نرخ ریزش به تفکیک ماه
        /// </summary>
        public class ChurnRateByMonthConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string Name => "نرخ ریزش ماهانه";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Month({nameof(CustomerTransaction.LastModified)})", "ماه");
                Count(null, "مشتریان غیرفعال شده");
            }
        }

        // =====================================================
        // Engagement Score Reports
        // =====================================================

        /// <summary>
        /// گزارش توزیع نمره تعامل مشتریان
        /// </summary>
        public class EngagementScoreDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
            // Enable category ceiling to reduce distinct buckets on the chart axis
            protected override string ChartAdvancedOptions => ChartAdvancedOption.ceil_categories;
            protected override string Name => "توزیع نمره تعامل";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.EngagementScore), "نمره تعامل");
                Count(null, "تعداد مشتریان");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<HighlyEngagedCustomersConfig>(Report.SubReportType.SubReport);
            }
        }

        /// <summary>
        /// گزارش مشتریان با بالاترین تعامل
        /// </summary>
        public class HighlyEngagedCustomersConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerTenant.EngagementScore)} > 70";
            protected override string Name => "مشتریان با تعامل بالا";
            
            // رنگ‌بندی بر اساس نمره تعامل
            protected override string TableSuccess => $"{nameof(CustomerTenant.EngagementScore)} >= 90";
            protected override string TableInfo => $"{nameof(CustomerTenant.EngagementScore)} >= 80 && {nameof(CustomerTenant.EngagementScore)} < 90";
            protected override string TableWarning => $"{nameof(CustomerTenant.EngagementScore)} >= 70 && {nameof(CustomerTenant.EngagementScore)} < 80";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTenant.Customer), "مشتری");
                DisplayColumn(nameof(CustomerTenant.Tenant), "اکوسیستم");
                DisplayColumn(nameof(CustomerTenant.EngagementScore), "نمره تعامل");
                DisplayColumn(nameof(CustomerTenant.TotalInteractions), "تعداد تعاملات");
                DisplayColumn(nameof(CustomerTenant.LastInteractionDate), "آخرین تعامل");

                OrderByDesc(nameof(CustomerTenant.EngagementScore)); // نزولی - بالاترین تعامل
            }
        }

        /// <summary>
        /// گزارش میانگین نمره تعامل به تفکیک دسته RFM
        /// </summary>
        public class AverageEngagementByRfmConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => ClubRoles.RolesAnalyst;
            protected override string Name => "میانگین تعامل به تفکیک RFM";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.RfmSegment), "دسته RFM");
                Average(nameof(CustomerTenant.EngagementScore), "میانگین نمره تعامل");
                Count(null, "تعداد");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        // =====================================================
        // NPS & Satisfaction Reports
        // =====================================================

        /// <summary>
        /// گزارش توزیع NPS Score
        /// </summary>
        public class NpsDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override string Name => "توزیع نمره NPS";
            protected override List<string> Roles => ClubRoles.RolesManager;
            // Enable category ceiling to reduce distinct buckets on the chart axis
            protected override string ChartAdvancedOptions => ChartAdvancedOption.ceil_categories;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.NpsScore), "نمره NPS");
                Count(null, "تعداد مشتریان");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش دسته‌بندی NPS - Promoters, Passives, Detractors
        /// </summary>
        public class NpsSegmentationConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => ClubRoles.RolesManager;
            // Enable category ceiling to reduce distinct buckets on the chart axis
            protected override string ChartAdvancedOptions => ChartAdvancedOption.ceil_categories;
            protected override string Name => "دسته‌بندی NPS";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.NpsScore), "دسته NPS");
                Count(null, "تعداد");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<NpsDistributionConfig>(Report.SubReportType.SubReport);
            }
        }

        /// <summary>
        /// گزارش میانگین نمره رضایت
        /// </summary>
        public class AverageSatisfactionScoreConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
			protected override string Name => "میانگین نمره رضایت";
			protected override List<string> Roles => ClubRoles.RolesManager;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.Tenant));
				Average(nameof(CustomerTenant.SatisfactionScore), "میانگین رضایت");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<SatisfactionScoreDistributionConfig>(Report.SubReportType.SubReport);
            }
        }

        /// <summary>
        /// نمایش متوسط نمره رضایت به صورت Gauge
        /// این ویجت برای نمایش متوسط نمره رضایت مشتریان (0-100) مناسب است
        /// می‌توانید در تنظیمات گزارش، Range (0-100) و Levels (سطوح رنگی) را تنظیم کنید
        /// </summary>
        public class AverageSatisfactionScoreGaugeConfig() : ChartConfigDefinition(ChartType.Gauge)
        {
            protected override string Name => "متوسط نمره رضایت (Gauge)";
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"!IsNullOrEmpty({nameof(CustomerTenant.SatisfactionScore)})";

            protected override void DefineGroupBy()
            {
                // استفاده از GroupBy ثابت برای محاسبه Average کلی
                GroupByFormula("1", "کل");
                Average(nameof(CustomerTenant.SatisfactionScore), "متوسط نمره رضایت");
            }
        }

        /// <summary>
        /// نمایش متوسط نمره تعامل به صورت Gauge
        /// این ویجت برای نمایش متوسط نمره تعامل مشتریان (0-100) مناسب است
        /// می‌توانید در تنظیمات گزارش، Range (0-100) و Levels (سطوح رنگی) را تنظیم کنید
        /// </summary>
        public class AverageEngagementScoreGaugeConfig() : ChartConfigDefinition(ChartType.Gauge)
        {
            protected override string Name => "متوسط نمره تعامل (Gauge)";
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"!IsNullOrEmpty({nameof(CustomerTenant.EngagementScore)})";

            protected override void DefineGroupBy()
            {
                // استفاده از GroupBy ثابت برای محاسبه Average کلی
                GroupByFormula("1", "کل");
                Average(nameof(CustomerTenant.EngagementScore), "متوسط نمره تعامل");
            }
        }

        /// <summary>
        /// نمایش متوسط نمره وفاداری به صورت Gauge
        /// این ویجت برای نمایش متوسط نمره وفاداری مشتریان (0-100) مناسب است
        /// می‌توانید در تنظیمات گزارش، Range (0-100) و Levels (سطوح رنگی) را تنظیم کنید
        /// </summary>
        public class AverageLoyaltyScoreGaugeConfig() : ChartConfigDefinition(ChartType.Gauge)
        {
            protected override string Name => "متوسط نمره وفاداری (Gauge)";
            protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"!IsNullOrEmpty({nameof(CustomerTenant.LoyaltyScore)})";

            protected override void DefineGroupBy()
            {
                // استفاده از GroupBy ثابت برای محاسبه Average کلی
                GroupByFormula("1", "کل");
                Average(nameof(CustomerTenant.LoyaltyScore), "متوسط نمره وفاداری");
            }
        }

        /// <summary>
        /// گزارش توزیع نمره رضایت
        /// </summary>
        public class SatisfactionScoreDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
			protected override string Name => "توزیع نمره رضایت";
			protected override List<string> Roles => ClubRoles.RolesAnalyst;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.SatisfactionScore), "نمره رضایت");
                Count(null, "تعداد مشتریان");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerTenantDetailsListConfig>();
                AddSubReport<AverageSatisfactionScoreConfig>(Report.SubReportType.SubReport);
            }
        }

        // =====================================================
        // Loyalty Score Reports
        // =====================================================

        /// <summary>
        /// گزارش توزیع نمره وفاداری
        /// </summary>
        public class LoyaltyScoreDistributionConfig() : ChartConfigDefinition(ChartType.Column)
        {
			protected override string Name => "توزیع نمره وفاداری";
			protected override List<string> Roles => ClubRoles.RolesManager;

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.LoyaltyScore), "نمره وفاداری");
                Count(null, "تعداد مشتریان");
            }
        }

        /// <summary>
        /// گزارش مشتریان وفادار
        /// </summary>
        public class LoyalCustomersConfig() : ReportConfigDefinition
        {
			protected override string Name => "مشتریان وفادار";
			protected override List<string> Roles => ClubRoles.RolesManager;
            protected override string WhereCondition => $"{nameof(CustomerTenant.LoyaltyScore)} > 70";
            
            // رنگ‌بندی بر اساس نمره وفاداری
            protected override string TableSuccess => $"{nameof(CustomerTenant.LoyaltyScore)} >= 90";
            protected override string TableInfo => $"{nameof(CustomerTenant.LoyaltyScore)} >= 80 && {nameof(CustomerTenant.LoyaltyScore)} < 90";
            protected override string TableWarning => $"{nameof(CustomerTenant.LoyaltyScore)} >= 70 && {nameof(CustomerTenant.LoyaltyScore)} < 80";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(CustomerTenant.Customer), "مشتری");
                DisplayColumn(nameof(CustomerTenant.Tenant), "اکوسیستم");
                DisplayColumn(nameof(CustomerTenant.LoyaltyScore), "نمره وفاداری");
                DisplayColumn(nameof(CustomerTenant.CustomerLifetimeValue), "CLV");
                DisplayColumn(nameof(CustomerTenant.TotalInteractions), "تعداد تعاملات");

                OrderByDesc(nameof(CustomerTenant.LoyaltyScore)); // نزولی - بالاترین وفاداری
            }
        }
    }
}
