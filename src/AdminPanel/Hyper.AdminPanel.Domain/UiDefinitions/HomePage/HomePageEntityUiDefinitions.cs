using Hyper.Domain.Entities.Database;
using Hyper.AdminPanel.Domain.UiDefinitions.Database;
namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;
public sealed class HomePageEntityUiDefinitions : CRUDDefinition<HomePageEntity>
{
    public override List<string>? Roles => [HyperRoles.Admin];
    public sealed class HomePageDashboard : DashboardDefinition
    {
        public override List<string>? Roles => [HyperRoles.Admin];
        protected override Form Identify() => DefineDashboard("داشبورد هایپریک و یکسان‌سازی");
        protected override void DataSources()
        {
            AddReport<SqlTblShop>();
            AddReport<SqlTblProduct>();
            AddReport<SqlTblPerson>();
            AddReport<SqlTblWarehouse>();
            AddReport<SqlTblSaleorder>();
            AddReport<SqlTblPurchaseorder>();
            AddReport<SqlTblServicerequest>();
            AddReport<SqlVwMarketingsubscriptionmonthly>();
            AddReport<SqlExternalintegrationconnections>();
            AddReport<SqlExternalproductmappings>();
            AddReport<SqlIntegrationoutbox>();
            AddReport<SqlIntegrationwebhookinbox>();
            AddReport<SqlIntegrationsyncruns>();
        }
public class BusinessDashboard : DashboardConfigDefinition
        {
            protected override string Title => "مorig و حسابداری";
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override bool IsDefault => true;
            public class Widget1 : DashboardDivWidgetDefinition<SqlTblShop, SqlTblShopUiDefinitions.PublicReport, SqlTblShopUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "م.orig‌ها";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget2 : DashboardDivWidgetDefinition<SqlTblProduct, SqlTblProductUiDefinitions.PublicReport, SqlTblProductUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "کالاها";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget3 : DashboardDivWidgetDefinition<SqlTblPerson, SqlTblPersonUiDefinitions.PublicReport, SqlTblPersonUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "طرف حساب‌ها";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget4 : DashboardDivWidgetDefinition<SqlTblWarehouse, SqlTblWarehouseUiDefinitions.PublicReport, SqlTblWarehouseUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "انبارها";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget5 : DashboardDivWidgetDefinition<SqlTblSaleorder, SqlTblSaleorderUiDefinitions.PublicReport, SqlTblSaleorderUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "فاکتورهای فروش";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget6 : DashboardDivWidgetDefinition<SqlTblPurchaseorder, SqlTblPurchaseorderUiDefinitions.PublicReport, SqlTblPurchaseorderUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "فاکتورهای خرید";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget7 : DashboardDivWidgetDefinition<SqlTblProduct, SqlTblProductUiDefinitions.PublicReport, SqlTblProductUiDefinitions.PublicReport.ByShopidConfig>
            {
                public override string Title => "کالا به تفکیک م.orig";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget8 : DashboardDivWidgetDefinition<SqlTblSaleorder, SqlTblSaleorderUiDefinitions.PublicReport, SqlTblSaleorderUiDefinitions.PublicReport.ByShopidConfig>
            {
                public override string Title => "فروش به تفکیک م.orig";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
        }
        public class MarketingDashboard : DashboardConfigDefinition
        {
            protected override string Title => "بازاریابی و فروش";
            protected override List<string> Roles => [HyperRoles.Admin, HyperRoles.MarketingManager, HyperRoles.Analyst];
            protected override bool IsDefault => false;
            protected override string Icon => "gift-present";

            public class MonthlyConversionRateWidget : DashboardDivWidgetDefinition<SqlVwMarketingsubscriptionmonthly, SqlVwMarketingsubscriptionmonthlyUiDefinitions.PublicReport, SqlVwMarketingsubscriptionmonthlyUiDefinitions.PublicReport.MonthlyConversionRateConfig>
            {
                public override string Title => "نرخ تبدیل ماهانه";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 320;
                protected override int? CacheTimeMinutes => 60;
                public override int Width => 6;
            }

            public class MonthlyRetentionRateWidget : DashboardDivWidgetDefinition<SqlVwMarketingsubscriptionmonthly, SqlVwMarketingsubscriptionmonthlyUiDefinitions.PublicReport, SqlVwMarketingsubscriptionmonthlyUiDefinitions.PublicReport.MonthlyRetentionRateConfig>
            {
                public override string Title => "نرخ حفظ ماهانه";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 320;
                protected override int? CacheTimeMinutes => 60;
                public override int Width => 6;
            }
        }
        public class SynchronizationDashboard : DashboardConfigDefinition
        {
            protected override string Title => "یکسان‌سازی مغازه و غرفه";
            protected override List<string> Roles => [HyperRoles.Admin];
            protected override bool IsDefault => false;
            public class Widget1 : DashboardDivWidgetDefinition<SqlExternalintegrationconnections, SqlExternalintegrationconnectionsUiDefinitions.PublicReport, SqlExternalintegrationconnectionsUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "اتصال‌ها";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget2 : DashboardDivWidgetDefinition<SqlExternalproductmappings, SqlExternalproductmappingsUiDefinitions.PublicReport, SqlExternalproductmappingsUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "نگاشت کالا";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget3 : DashboardDivWidgetDefinition<SqlIntegrationoutbox, SqlIntegrationoutboxUiDefinitions.PublicReport, SqlIntegrationoutboxUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "پیام‌های خروجی";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget4 : DashboardDivWidgetDefinition<SqlIntegrationwebhookinbox, SqlIntegrationwebhookinboxUiDefinitions.PublicReport, SqlIntegrationwebhookinboxUiDefinitions.PublicReport.CountConfig>
            {
                public override string Title => "رویدادهای ورودی";
                public override int Width => 3;
                protected override int? HeightInPixels => 160;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget5 : DashboardDivWidgetDefinition<SqlIntegrationsyncruns, SqlIntegrationsyncrunsUiDefinitions.PublicReport, SqlIntegrationsyncrunsUiDefinitions.PublicReport.ByStatusConfig>
            {
                public override string Title => "وضعیت اجراها";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget6 : DashboardDivWidgetDefinition<SqlIntegrationoutbox, SqlIntegrationoutboxUiDefinitions.PublicReport, SqlIntegrationoutboxUiDefinitions.PublicReport.ByStatusConfig>
            {
                public override string Title => "وضعیت صف خروجی";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget7 : DashboardDivWidgetDefinition<SqlIntegrationwebhookinbox, SqlIntegrationwebhookinboxUiDefinitions.PublicReport, SqlIntegrationwebhookinboxUiDefinitions.PublicReport.ByStatusConfig>
            {
                public override string Title => "وضعیت صندوق ورودی";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
            public class Widget8 : DashboardDivWidgetDefinition<SqlExternalintegrationconnections, SqlExternalintegrationconnectionsUiDefinitions.PublicReport, SqlExternalintegrationconnectionsUiDefinitions.PublicReport.ByProviderConfig>
            {
                public override string Title => "اتصال به تفکیک پلتفرم";
                public override int Width => 6;
                protected override int? HeightInPixels => 300;
                protected override int? CacheTimeMinutes => 1;
            }
        }
    }
}
