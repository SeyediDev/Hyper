namespace Club.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions
{
    public partial class PublicReport
    {
        // =====================================================
        // Geography Reports
        // =====================================================

        /// <summary>
        /// گزارش نقشه جهانی توزیع مشتریان
        /// </summary>
        public class GlobalGeographicDistributionConfig() : ChartConfigDefinition(ChartType.WorldMap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.CountryId)} != null";
            protected override string Name => "توزیع جهانی مشتریان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.Country), "کشور");
                GroupBy($"{nameof(CustomerTenant.Country)}.Iso2", "Iso2");
                Count(null, "تعداد مشتری");
				AverageDD1(nameof(CustomerTenant.SatisfactionScore), "میانگین رضایت");
				AverageDD1(nameof(CustomerTenant.CustomerLifetimeValue), "میانگین CLV");
				AverageDD1(nameof(CustomerTenant.EngagementScore), "میانگین تعامل");
            }

            protected override void DefineSubReports()
            {
                // Drilldown to customer details list for selected country(ies)
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }
        /// <summary>
        /// گزارش نقشه ایران بر اساس استان
        /// </summary>
        public class IranGeographicDistributionConfig() : ChartConfigDefinition(ChartType.IranMap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.ProvinceId)} != null";
            protected override string Name => "توزیع استانی مشتریان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.Province));
                GroupBy($"{nameof(CustomerTenant.Province)}.Iso2", "Iso2");
                Count(null, "تعداد مشتری");
				AverageDD1(nameof(CustomerTenant.SatisfactionScore), "میانگین رضایت");
                AverageDD1(nameof(CustomerTenant.CustomerLifetimeValue), "میانگین CLV");
                AverageDD1(nameof(CustomerTenant.EngagementScore), "میانگین تعامل");
            }

            protected override void DefineSubReports()
            {
                // Drilldown to customer details list for selected province(s)
                AddSubReport<CustomerTenantDetailsListConfig>();
            }
        }

        /// <summary>
        /// گزارش Pie Chart توزیع استانی مشتریان
        /// </summary>
        public class ProvinceBarDistributionConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.ProvinceId)} != null";
            protected override string Name => "نمودار دایره‌ای توزیع استانی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.Province), "استان");
                Count(null, "تعداد مشتری");
                OrderByDesc("COUNT");
            }
        }

        /// <summary>
        /// توزیع استانی به صورت TreeMap با دریل‌داون به شهرها
        /// </summary>
        public class ProvinceTreeMapConfig() : ChartConfigDefinition(ChartType.Treemap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string Name => "درخت‌نقشه توزیع استان‌ها";

            protected override void DefineGroupBy()
            {
                // Category (InColumn) for treemap
                GroupBy(nameof(CustomerTenant.Province), "استان");
                GroupByFormula($"{nameof(CustomerTenant.Province)}.{nameof(CustomerTenant.Province.Tam)}", "TAM");
                GroupByFormula($"{nameof(CustomerTenant.Province)}.{nameof(CustomerTenant.Province.Sam)}", "SAM");
				GroupByFormula($"{nameof(CustomerTenant.Province)}.{nameof(CustomerTenant.Province.Som)}", "SOM");
                // Size/color metric
                Count(null, "تعداد مشتری");
            }

            protected override void DefineSubReports()
            {
                // Drilldown to cities of selected province(s)
                AddSubReport<ProvinceCitiesTreeMapConfig>();
            }
        }

        /// <summary>
        /// دریل‌داون: توزیع شهرها در استان انتخاب‌شده (TreeMap)
        /// </summary>
        public class ProvinceCitiesTreeMapConfig() : ChartConfigDefinition(ChartType.Treemap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.CityId)} != null";
            protected override string Name => "درخت‌نقشه توزیع شهرها";

            protected override void DefineGroupBy()
            {
                // The parent drilldown passes Province Ids; engine applies parentFilter automatically
                GroupBy(nameof(CustomerTenant.City), "شهر");
				GroupByFormula($"{nameof(CustomerTenant.City)}.{nameof(CustomerTenant.City.Tam)}", "TAM");
				GroupByFormula($"{nameof(CustomerTenant.City)}.{nameof(CustomerTenant.City.Sam)}", "SAM");
				GroupByFormula($"{nameof(CustomerTenant.City)}.{nameof(CustomerTenant.City.Som)}", "SOM");
				Count(null, "تعداد مشتری");
            }
        }

        // =====================================================
        // Global Geography Reports (Countries)
        // =====================================================

        /// <summary>
        /// گزارش Pie Chart توزیع کشوری مشتریان
        /// </summary>
        public class CountryBarDistributionConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.CountryId)} != null";
            protected override string Name => "نمودار دایره‌ای توزیع کشوری";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.Country), "کشور");
                Count(null, "تعداد مشتری");
                OrderByDesc("COUNT");
            }
        }

        /// <summary>
        /// توزیع کشوری به صورت TreeMap با دریل‌داون به استان‌ها/شهرها
        /// </summary>
        public class CountryTreeMapConfig() : ChartConfigDefinition(ChartType.Treemap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.CountryId)} != null";
            protected override string Name => "درخت‌نقشه توزیع کشورها";

            protected override void DefineGroupBy()
            {
                // Category (InColumn) for treemap
                GroupBy(nameof(CustomerTenant.Country), "کشور");
                Count(null, "تعداد مشتری");
            }

            protected override void DefineSubReports()
            {
                // Drilldown to provinces/cities of selected country(ies)
                AddSubReport<CountryProvincesTreeMapConfig>();
            }
        }

        /// <summary>
        /// دریل‌داون: توزیع استان‌ها/شهرها در کشور انتخاب‌شده (TreeMap)
        /// </summary>
        public class CountryProvincesTreeMapConfig() : ChartConfigDefinition(ChartType.Treemap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, ClubRoles.Manager, ClubRoles.Analyst];
            protected override string WhereCondition => $"{nameof(CustomerTenant.ProvinceId)} != null OR {nameof(CustomerTenant.CityId)} != null";
            protected override string Name => "درخت‌نقشه توزیع استان‌ها/شهرها";

            protected override void DefineGroupBy()
            {
                // The parent drilldown passes Country Ids; engine applies parentFilter automatically
                // Try to group by Province first, if not available, use City
                GroupBy(nameof(CustomerTenant.Province), "استان/منطقه");
                GroupBy(nameof(CustomerTenant.City), "شهر");
                Count(null, "تعداد مشتری");
            }
        }
    }
}
