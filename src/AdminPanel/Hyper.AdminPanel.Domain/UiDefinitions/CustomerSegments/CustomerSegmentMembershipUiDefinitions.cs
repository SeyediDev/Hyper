using Hyper.Domain.Entities.CustomerSegments.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.CustomerSegments;

public class CustomerSegmentMembershipUiDefinitions : CRUDDefinition<CustomerSegmentMembership>
{
    public override string? Icon => "fa fa-user-circle";

    protected override void RefineEntity()
    {
        RenameField(nameof(CustomerTenant.CreateDate), "تاریخ عضویت", "Join Date");
    }

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(CustomerSegmentMembership.Segment),
                        nameof(CustomerSegmentMembership.CustomerTenant),
                        nameof(CustomerSegmentMembership.CreateDate),
                        nameof(CustomerSegmentMembership.IsManual));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(CustomerSegmentMembership.Segment),
                       nameof(CustomerSegmentMembership.CustomerTenant),
                       nameof(CustomerSegmentMembership.CreateDate),
                       nameof(CustomerSegmentMembership.IsManual),
                       nameof(CustomerSegmentMembership.EventLog));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        // =====================================================
        // Segment Member Geographic Analysis Reports
        // These reports analyze CustomerSegmentMemberships with their geographic data
        // =====================================================

        /// <summary>
        /// گزارش اعضای جوامع/بازارها به تفکیک کشور
        /// </summary>
        public class SegmentMembersByCountryConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "اعضای جوامع/بازارها به تفکیک کشور";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "عنوان جامعه/بازار");
                GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Country)}.{nameof(Country.Title)}", "کشور");
                Count(null, "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش اعضای جوامع/بازارها به تفکیک استان
        /// </summary>
        public class SegmentMembersByProvinceConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "اعضای جوامع/بازارها به تفکیک استان";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "عنوان جامعه/بازار");
                GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Province)}.{nameof(Province.Title)}", "استان");
                Count(null, "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش اعضای جوامع/بازارها به تفکیک شهر
        /// </summary>
        public class SegmentMembersByCityConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => HyperRoles.RolesManager;
            protected override string Name => "اعضای جوامع/بازارها به تفکیک شهر";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "عنوان جامعه/بازار");
                GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.City)}.{nameof(City.Title)}", "شهر");
                Count(null, "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش نقشه ایران: توزیع اعضای جوامع/بازارها بر اساس استان
        /// </summary>
        public class SegmentMembersIranMapConfig() : ChartConfigDefinition(ChartType.IranMap)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override string Name => "نقشه ایران: توزیع اعضای جوامع/بازارها";

            protected override void DefineGroupBy()
            {
				GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Province)}.{nameof(Province.Title)}", "استان");
				GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Province)}.{nameof(Province.Iso2)}", "Iso2");
				Count($"{nameof(CustomerSegmentMembership.SegmentId)}", "تعداد جامعه/بازار");
                Count(null, "تعداد اعضا");
            }

            protected override void DefineSubReports()
            {
                // Drilldown to segment members list for selected province(s)
                AddSubReport<SegmentMembersDetailsListConfig>();
            }
        }

        /// <summary>
        /// لیست جزئیات اعضای جوامع/بازارها برای استفاده در زیرگزارش‌ها
        /// </summary>
        public class SegmentMembersDetailsListConfig() : ReportConfigDefinition
        {
            protected override string Name => "جزئیات اعضای جوامع/بازارها";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

            protected override void DefineColumns()
            {
                DisplayColumn($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "عنوان جامعه/بازار");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Tenant)}", "اکوسیستم");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Province)}", "استان");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.City)}", "شهر");
                DisplayColumn(nameof(CustomerSegmentMembership.CreateDate), "تاریخ عضویت");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerLifetimeValue)}", "ارزش طول عمر");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.EngagementScore)}", "نمره تعامل");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.SatisfactionScore)}", "نمره رضایت");
            }
        }

        /// <summary>
        /// گزارش جامع: اعضای جوامع/بازارها به تفکیک جامعه/بازار و موقعیت جغرافیایی
        /// </summary>
        public class SegmentMembersGeographyOverviewConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "اعضای جوامع/بازارها به تفکیک موقعیت جغرافیایی";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "عنوان جامعه/بازار");
				GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Country)}.{nameof(Country.Title)}", "کشور");
				GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Province)}.{nameof(Province.Title)}", "استان");
				GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.City)}.{nameof(City.Title)}", "شهر");
                Count(null, "تعداد اعضا");
            }
        }

        /// <summary>
        /// گزارش Pivot: دسته RFM × جامعه مشتریان
        /// ماتریس کامل توزیع مشتریان به تفکیک دسته RFM و جامعه مشتریان
        /// 
        /// نکته طراحی ماتریس: 
        /// - جامعه در ستون عمودی (Vertical) قرار گرفته چون تعداد جوامع محدود است (معمولاً 5-15)
        /// - دسته RFM در ستون افقی (Horizontal) قرار گرفته چون تعداد دسته‌ها محدود است (8-10)
        /// - هرگز مشتریان را در ستون عمودی قرار ندهید چون تعداد آنها زیاد است و ماتریس طولانی می‌شود
        /// </summary>
        public class RfmSegmentCommunityPivotConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst, HyperRoles.Manager];
            protected override string Name => "Pivot دسته RFM × جامعه مشتریان";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                // جامعه در ستون عمودی (Vertical) - تعداد محدود (5-15 جامعه)
                GroupByFormula($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "جامعه", addAsDisplayColumn: false);
                LayoutColumn(false, $"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", ConfiguredReport.ReportMatrixType.Vertical);
                DisplayColumn($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "جامعه");

                // دسته RFM در ستون افقی (Horizontal) - تعداد محدود (8-10 دسته)
                GroupByFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.RfmSegment)}", "دسته RFM", addAsDisplayColumn: false);
				//TODO Cursor : یه اینام به نام RFMSegment داریم که که باید مقادیر خروجی این گزارش با این اینام تفسیر شود (هنگام نمایش) در واقع اگر 
                // تابع GetReportData به نوعی بفهمد که باید اینکار را بکند در همه انواع خروجی گزارش ها و نمودار ها این تفسیر انجام می شود.
                //این مشکل را وقتی داریم که فیلد فرمولی باشد در حالت فیلد معمولی قبلا ساپورت شده است.
				LayoutColumn(false, $"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.RfmSegment)}", ConfiguredReport.ReportMatrixType.Horizontal);
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.RfmSegment)}", "دسته RFM");

                // مقادیر: تعداد مشتریان، مجموع CLV، میانگین CLV، میانگین نمره تعامل
                Count(nameof(CustomerSegmentMembership.CustomerTenantId), "تعداد مشتریان");
                SumFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerLifetimeValue)}", "مجموع CLV");
                AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerLifetimeValue)}", "میانگین CLV");
                AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.EngagementScore)}", "میانگین نمره تعامل");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.NpsScore)}", "میانگین NPS");
				SumFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerAcquisitionCost)}", "جمع هزینه جذب (CAC)");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerAcquisitionCost)}", "میانگین هزینه جذب (CAC)");
                AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerProfitMargin)}", "میانگین حاشیه سود");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<RfmSegmentCommunityPivotDetailsConfig>();
            }
        }
		public class SegmentCommunity() : GroupByConfigDefinition
		{
			protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst, HyperRoles.Manager];
			protected override string Name => "جامعه مشتریان";
			protected override void DefineGroupBy()
			{
				// جامعه در ستون عمودی (Vertical) - تعداد محدود (5-15 جامعه)
				GroupByFormula($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "جامعه", true);
				// مقادیر: تعداد مشتریان، مجموع CLV، میانگین CLV، میانگین نمره تعامل
				Count(nameof(CustomerSegmentMembership.CustomerTenantId), "تعداد مشتریان");
				SumFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerLifetimeValue)}", "مجموع CLV");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerLifetimeValue)}", "میانگین CLV");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.EngagementScore)}", "میانگین نمره تعامل");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.NpsScore)}", "میانگین NPS");
				SumFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerAcquisitionCost)}", "جمع هزینه جذب (CAC)");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerAcquisitionCost)}", "میانگین هزینه جذب (CAC)");
				AverageFormula($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerProfitMargin)}", "میانگین حاشیه سود");
			}

			protected override void DefineSubReports()
			{
				AddSubReport<RfmSegmentCommunityPivotDetailsConfig>();
			}
		}

		/// <summary>
		/// لیست جزئیات Pivot برای استفاده در زیرگزارش‌ها
		/// </summary>
		public class RfmSegmentCommunityPivotDetailsConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst, HyperRoles.Manager];
            protected override string Name => "فهرست اعضاء";
            

            protected override void DefineColumns()
            {
                DisplayColumn($"{nameof(CustomerSegmentMembership.Segment)}.{nameof(CustomerSegment.Title)}", "جامعه");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.RfmSegment)}", "دسته RFM");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.CustomerLifetimeValue)}", "CLV");
                DisplayColumn($"{nameof(CustomerSegmentMembership.CustomerTenant)}.{nameof(CustomerTenant.EngagementScore)}", "نمره تعامل");
            }
        }
    }
}
