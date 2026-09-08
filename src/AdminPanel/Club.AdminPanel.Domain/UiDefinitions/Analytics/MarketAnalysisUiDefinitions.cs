namespace Hyper.AdminPanel.Domain.UiDefinitions.Analytics;

public class MarketAnalysisUiDefinitions : CRUDDefinition<MarketAnalysis>
{
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(MarketAnalysis.Title),
                        nameof(MarketAnalysis.Tenant),
                        nameof(MarketAnalysis.AnalysisDate),
                        nameof(MarketAnalysis.Period),
                        nameof(MarketAnalysis.TotalAddressableMarket),
                        nameof(MarketAnalysis.ServiceableAddressableMarket),
                        nameof(MarketAnalysis.ServiceableObtainableMarket),
                        nameof(MarketAnalysis.CurrentMarketShare),
                        nameof(MarketAnalysis.GrowthPotential),
                        nameof(MarketAnalysis.CompetitiveScore)
                        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(MarketAnalysis.Tenant),
                       nameof(MarketAnalysis.Title),
                       nameof(MarketAnalysis.AnalysisDate),
                       nameof(MarketAnalysis.Period),
                       nameof(MarketAnalysis.TotalAddressableMarket),
                       nameof(MarketAnalysis.ServiceableAddressableMarket),
                       nameof(MarketAnalysis.ServiceableObtainableMarket),
                       nameof(MarketAnalysis.CurrentMarketShare),
                       nameof(MarketAnalysis.GrowthPotential),
                       nameof(MarketAnalysis.PointsOfParity),
                       nameof(MarketAnalysis.PointsOfDifference),
                       nameof(MarketAnalysis.CompetitiveScore),
                       nameof(MarketAnalysis.TotalMarketSize),
                       nameof(MarketAnalysis.TargetMarketSize),
                       nameof(MarketAnalysis.AccessibleMarketSize),
                       nameof(MarketAnalysis.ObtainableMarketSize),
                       nameof(MarketAnalysis.MarketGrowthRate),
                       nameof(MarketAnalysis.AnnualGrowthRate),
                       nameof(MarketAnalysis.PredictedGrowthRate),
                       nameof(MarketAnalysis.TotalCustomers),
                       nameof(MarketAnalysis.TargetCustomers),
                       nameof(MarketAnalysis.AccessibleCustomers),
                       nameof(MarketAnalysis.ObtainableCustomers),
                       nameof(MarketAnalysis.TotalMarketRevenue),
                       nameof(MarketAnalysis.TargetMarketRevenue),
                       nameof(MarketAnalysis.AccessibleMarketRevenue),
                       nameof(MarketAnalysis.ObtainableMarketRevenue),
                       nameof(MarketAnalysis.CompetitiveRank),
                       nameof(MarketAnalysis.CompetitorCount),
                       nameof(MarketAnalysis.CompetitorShare),
                       nameof(MarketAnalysis.CompetitiveAdvantageScore),
                       nameof(MarketAnalysis.MarketTrend),
                       nameof(MarketAnalysis.MarketOpportunities),
                       nameof(MarketAnalysis.MarketThreats),
                       nameof(MarketAnalysis.Strengths),
                       nameof(MarketAnalysis.Weaknesses),
                       nameof(MarketAnalysis.AnalysisAccuracy),
                       nameof(MarketAnalysis.AnalysisConfidence),
                       nameof(MarketAnalysis.DataSources),
                       nameof(MarketAnalysis.AnalysisMethod),
                       nameof(MarketAnalysis.LastUpdatedDate),
                       nameof(MarketAnalysis.ModelVersion),
                       nameof(MarketAnalysis.AdditionalData)
                       );
    }
}
