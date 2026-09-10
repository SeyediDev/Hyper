namespace Hyper.AdminPanel.Domain.UiDefinitions.Analytics;

public class ProductFitAnalysisUiDefinitions : CRUDDefinition<ProductFitAnalysis>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst,
        HyperRoles.MarketingManager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(ProductFitAnalysis.Title),
                        nameof(ProductFitAnalysis.Tenant),
                        nameof(ProductFitAnalysis.AnalysisDate),
                        nameof(ProductFitAnalysis.ProductMarketFitScore),
                        nameof(ProductFitAnalysis.ProductServiceFitScore),
                        nameof(ProductFitAnalysis.ValueScore),
                        nameof(ProductFitAnalysis.CustomerSatisfactionScore),
                        nameof(ProductFitAnalysis.NetPromoterScore),
                        nameof(ProductFitAnalysis.CustomerSatisfactionRating),
                        nameof(ProductFitAnalysis.ProductQualityScore),
                        nameof(ProductFitAnalysis.ProductPerformanceScore)
                        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(ProductFitAnalysis.Tenant),
                       nameof(ProductFitAnalysis.Title),
                       nameof(ProductFitAnalysis.AnalysisDate),
                       nameof(ProductFitAnalysis.ProductMarketFitScore),
                       nameof(ProductFitAnalysis.ProductServiceFitScore),
                       nameof(ProductFitAnalysis.UniqueValueProposition),
                       nameof(ProductFitAnalysis.UniqueSellingProposition),
                       nameof(ProductFitAnalysis.ValueScore),
                       nameof(ProductFitAnalysis.CustomerSatisfactionScore),
                       nameof(ProductFitAnalysis.NetPromoterScore),
                       nameof(ProductFitAnalysis.CustomerSatisfactionRating),
                       nameof(ProductFitAnalysis.ProductQualityScore),
                       nameof(ProductFitAnalysis.ProductPerformanceScore),
                       nameof(ProductFitAnalysis.LastUpdatedDate),
                       nameof(ProductFitAnalysis.ModelVersion),
                       nameof(ProductFitAnalysis.AdditionalData)
                       );
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        /// <summary>
        /// گزارش تحلیل تناسب محصول - نمای کلی
        /// </summary>
        public class ProductFitOverviewConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "تحلیل تناسب محصول";

            protected override void DefineColumns()
            {
                DisplayColumn($"{nameof(ProductFitAnalysis.Product)}.{nameof(Product.Title)}", "محصول");
                DisplayColumn(nameof(ProductFitAnalysis.ProductMarketFitScore), "PMF");
                DisplayColumn(nameof(ProductFitAnalysis.ProductServiceFitScore), "PSF");
                DisplayColumn(nameof(ProductFitAnalysis.ValueScore), "نمره ارزش");
                DisplayColumn(nameof(ProductFitAnalysis.CustomerSatisfactionScore), "رضایت مشتری");
                DisplayColumn(nameof(ProductFitAnalysis.NetPromoterScore), "NPS");
                DisplayColumn(nameof(ProductFitAnalysis.SuccessProbability), "احتمال موفقیت");
                OrderByDesc(nameof(ProductFitAnalysis.ProductMarketFitScore));
            }
        }

        /// <summary>
        /// گزارش مقایسه نمرات تحلیل محصول
        /// </summary>
        public class ProductFitScoresComparisonConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "مقایسه نمرات تحلیل محصول";

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(ProductFitAnalysis.Product)}.{nameof(Product.Title)}", "محصول");
                Average(nameof(ProductFitAnalysis.ProductMarketFitScore), "PMF");
                Average(nameof(ProductFitAnalysis.ProductServiceFitScore), "PSF");
                Average(nameof(ProductFitAnalysis.ValueScore), "نمره ارزش");
                Average(nameof(ProductFitAnalysis.CustomerSatisfactionScore), "رضایت");
            }
        }

        /// <summary>
        /// گزارش محصولات بر اساس احتمال موفقیت
        /// </summary>
        public class ProductsBySuccessProbabilityConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "محصولات بر اساس احتمال موفقیت";

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(ProductFitAnalysis.Product)}.{nameof(Product.Title)}", "محصول");
                Average(nameof(ProductFitAnalysis.SuccessProbability), "احتمال موفقیت");
                Average(nameof(ProductFitAnalysis.FailureProbability), "احتمال شکست");
                OrderByDesc("AVG");
            }
        }
    }
}
