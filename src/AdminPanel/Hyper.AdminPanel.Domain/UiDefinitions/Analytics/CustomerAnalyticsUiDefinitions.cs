namespace Hyper.AdminPanel.Domain.UiDefinitions.Analytics;

public class CustomerAnalyticsUiDefinitions : SubCRUDDefinition<CustomerAnalytics>
{
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(CustomerAnalytics.CustomerTenant),
                        nameof(CustomerAnalytics.Tenant),
                        nameof(CustomerAnalytics.AnalysisDate),
                        nameof(CustomerAnalytics.Period),
                        nameof(CustomerAnalytics.RFMScore),
                        nameof(CustomerAnalytics.RFMSegment),
                        nameof(CustomerAnalytics.CustomerLifetimeValue),
                        nameof(CustomerAnalytics.NetPromoterScore),
                        nameof(CustomerAnalytics.EngagementScore),
                        nameof(CustomerAnalytics.ChurnProbability)
                        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(CustomerAnalytics.CustomerTenant),
                       nameof(CustomerAnalytics.Tenant),
                       nameof(CustomerAnalytics.AnalysisDate),
                       nameof(CustomerAnalytics.Period),
                       nameof(CustomerAnalytics.RecencyScore),
                       nameof(CustomerAnalytics.FrequencyScore),
                       nameof(CustomerAnalytics.MonetaryScore),
                       nameof(CustomerAnalytics.RFMScore),
                       nameof(CustomerAnalytics.RFMSegment),
                       nameof(CustomerAnalytics.CustomerLifetimeValue),
                       nameof(CustomerAnalytics.PredictedValue),
                       nameof(CustomerAnalytics.RetentionRate),
                       nameof(CustomerAnalytics.ChurnProbability),
                       nameof(CustomerAnalytics.EngagementScore),
                       nameof(CustomerAnalytics.NetPromoterScore),
                       nameof(CustomerAnalytics.SatisfactionScore),
                       nameof(CustomerAnalytics.LoyaltyScore),
                       nameof(CustomerAnalytics.ValueScore),
                       nameof(CustomerAnalytics.RiskScore),
                       nameof(CustomerAnalytics.PotentialScore)
                       );
    }

    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(CustomerAnalytics.CustomerTenant),
                        nameof(CustomerAnalytics.AnalysisDate),
                        nameof(CustomerAnalytics.RFMScore),
                        nameof(CustomerAnalytics.CustomerLifetimeValue),
                        nameof(CustomerAnalytics.NetPromoterScore),
                        nameof(CustomerAnalytics.ChurnProbability)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(CustomerAnalytics.CustomerTenant),
                       nameof(CustomerAnalytics.AnalysisDate),
                       nameof(CustomerAnalytics.Period),
                       nameof(CustomerAnalytics.RFMScore),
                       nameof(CustomerAnalytics.RFMSegment),
                       nameof(CustomerAnalytics.CustomerLifetimeValue),
                       nameof(CustomerAnalytics.NetPromoterScore),
                       nameof(CustomerAnalytics.EngagementScore),
                       nameof(CustomerAnalytics.ChurnProbability),
                       nameof(CustomerAnalytics.LoyaltyScore),
                       nameof(CustomerAnalytics.ValueScore)
                       );
    }
}
