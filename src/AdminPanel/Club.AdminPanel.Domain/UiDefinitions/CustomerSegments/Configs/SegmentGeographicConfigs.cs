namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerSegments;

public partial class CustomerSegmentUiDefinitions
{
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Segment Geographic Analysis Reports
        // =====================================================

        /// <summary>
        /// گزارش توزیع جوامع/بازارها به تفکیک کشور
        /// </summary>
        public class SegmentDistributionByCountryConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "توزیع جوامع/بازارها به تفکیک کشور";

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(CustomerSegment.Country)}.{nameof(Country.Title)}", "کشور");
                Count(null, "تعداد جوامع/بازارها");
                Sum(nameof(CustomerSegment.ActualSize), "جمع اعضا");
            }
        }

        /// <summary>
        /// گزارش توزیع جوامع/بازارها به تفکیک استان
        /// </summary>
        public class SegmentDistributionByProvinceConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "توزیع جوامع/بازارها به تفکیک استان";

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(CustomerSegment.Province)}.{nameof(Province.Title)}", "استان");
                Count(null, "تعداد جوامع/بازارها");
                Sum(nameof(CustomerSegment.ActualSize), "جمع اعضا");
            }
        }

        /// <summary>
        /// گزارش توزیع جوامع/بازارها به تفکیک شهر
        /// </summary>
        public class SegmentDistributionByCityConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "توزیع جوامع/بازارها به تفکیک شهر";

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(CustomerSegment.City)}.{nameof(City.Title)}", "شهر");
                Count(null, "تعداد جوامع/بازارها");
                Sum(nameof(CustomerSegment.ActualSize), "جمع اعضا");
            }
        }

        /// <summary>
        /// گزارش اعضای جوامع/بازارها به تفکیک موقعیت جغرافیایی
        /// </summary>
        public class SegmentMembersByGeographyConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "اعضای جوامع/بازارها به تفکیک موقعیت جغرافیایی";

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(CustomerSegment.Title)}", "عنوان جامعه/بازار");
                GroupBy($"{nameof(CustomerSegment.Country)}.{nameof(Country.Title)}", "کشور");
                GroupBy($"{nameof(CustomerSegment.Province)}.{nameof(Province.Title)}", "استان");
                GroupBy($"{nameof(CustomerSegment.City)}.{nameof(City.Title)}", "شهر");
                Sum(nameof(CustomerSegment.ActualSize), "تعداد اعضا");
            }
        }
    }
}

