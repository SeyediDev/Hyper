using Hyper.AdminPanel.Domain.UiDefinitions.Analytics;
using Hyper.Domain.Entities.Events.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// تب داشبورد محصول با تمرکز بر تمامی ویجت‌های محصول‌محور
        /// </summary>
        public class ProductDashboard : DashboardConfigDefinition
        {
            protected override string Title => "محصولات";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst, HyperRoles.MarketingManager];
            protected override bool IsDefault => false;
            protected override string Icon => "box-package";

            /// <summary>
            /// ارزش طول عمر محصولات
            /// </summary>
            public class ProductClvWidget : DashboardDivWidgetDefinition<Product, ProductUiDefinitions.PublicReport, ProductUiDefinitions.PublicReport.ProductCLVConfig>
            {
                public override string Title => "ارزش طول عمر محصولات";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "coins-money";
            }

            /// <summary>
            /// تکرار خرید به تفکیک محصول
            /// </summary>
            public class ProductRepeatPurchaseWidget : DashboardDivWidgetDefinition<Product, ProductUiDefinitions.PublicReport, ProductUiDefinitions.PublicReport.ProductRepeatPurchaseConfig>
            {
                public override string Title => "تکرار خرید به تفکیک محصول";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "store-shop";
            }

            /// <summary>
            /// محبوب‌ترین محصولات
            /// </summary>
            public class PopularProductsWidget : DashboardDivWidgetDefinition<Product, ProductUiDefinitions.PublicReport, ProductUiDefinitions.PublicReport.PopularProductsConfig>
            {
                public override string Title => "محبوب‌ترین محصولات";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "box-package";
            }

            /// <summary>
            /// محصولات به تفکیک دسته
            /// </summary>
            public class ProductsByCategoryWidget : DashboardDivWidgetDefinition<Product, ProductUiDefinitions.PublicReport, ProductUiDefinitions.PublicReport.ProductsByCategoryConfig>
            {
                public override string Title => "محصولات به تفکیک دسته";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "chart-pie";
            }

            /// <summary>
            /// نظارت بر محصولات فعال
            /// </summary>
            public class EventLogByProductWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.EventLogByProductConfig>
            {
                public override string Title => "نظارت بر محصولات فعال";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "list-checklist";
            }

            /// <summary>
            /// محصولات در جریان
            /// </summary>
            public class ActiveProductsWidget : DashboardDivWidgetDefinition<Product, ProductUiDefinitions.PublicReport, ProductUiDefinitions.PublicReport.ActiveProductsConfig>
            {
                public override string Title => "محصولات در جریان";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "box-package";
            }

            /// <summary>
            /// تحلیل تناسب محصول - نمای کلی
            /// </summary>
            public class ProductFitOverviewWidget : DashboardDivWidgetDefinition<ProductFitAnalysis, ProductFitAnalysisUiDefinitions.PublicReport, ProductFitAnalysisUiDefinitions.PublicReport.ProductFitOverviewConfig>
            {
                public override string Title => "تحلیل تناسب محصول";
                protected override int? MaxRecordCount => 15;
                protected override int? HeightInPixels => 300;
                public override int Width => 12;
                protected override string Icon => "chart-bar";
            }

            /// <summary>
            /// مقایسه نمرات تحلیل محصول
            /// </summary>
            public class ProductFitScoresComparisonWidget : DashboardDivWidgetDefinition<ProductFitAnalysis, ProductFitAnalysisUiDefinitions.PublicReport, ProductFitAnalysisUiDefinitions.PublicReport.ProductFitScoresComparisonConfig>
            {
                public override string Title => "مقایسه نمرات تحلیل محصول";
                protected override int? HeightInPixels => 300;
                public override int Width => 6;
                protected override string Icon => "chart-bar";
            }

            /// <summary>
            /// محصولات بر اساس احتمال موفقیت
            /// </summary>
            public class ProductsBySuccessProbabilityWidget : DashboardDivWidgetDefinition<ProductFitAnalysis, ProductFitAnalysisUiDefinitions.PublicReport, ProductFitAnalysisUiDefinitions.PublicReport.ProductsBySuccessProbabilityConfig>
            {
                public override string Title => "محصولات بر اساس احتمال موفقیت";
                protected override int? HeightInPixels => 300;
                public override int Width => 6;
                protected override string Icon => "chart-line";
            }

            /// <summary>
            /// ماتریس محصول × مشتری
            /// </summary>
            public class ProductCustomerMatrixWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.ProductCustomerMatrixConfig>
            {
                public override string Title => "ماتریس محصول × مشتری";
                protected override int? HeightInPixels => 400;
                protected override int? MaxRecordCount => 10;
				public override int Width => 12;
                protected override string Icon => "table";
            }

            /// <summary>
            /// ماتریس محصول × کمپین
            /// </summary>
            public class ProductPromotionMatrixWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.ProductPromotionMatrixConfig>
            {
                public override string Title => "ماتریس محصول × کمپین";
                protected override int? HeightInPixels => 400;
				protected override int? MaxRecordCount => 10;
				public override int Width => 12;
                protected override string Icon => "table";
            }
        }
    }
}

