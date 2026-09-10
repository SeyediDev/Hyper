namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTenants;

public partial class CustomerTenantUiDefinitions : CRUDDefinition<CustomerTenant>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst,
        HyperRoles.MarketingManager,
        HyperRoles.CallCenterManager,
        HyperRoles.CallCenterSupport
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(
            nameof(CustomerTenant.Customer),
            nameof(CustomerTenant.Tenant),
            nameof(CustomerTenant.RfmSegment),
            nameof(CustomerTenant.CustomerLifetimeValue),
            nameof(CustomerTenant.CustomerAcquisitionCost),
            nameof(CustomerTenant.LtvToCacRatio),
            nameof(CustomerTenant.CustomerProfitMargin),
            nameof(CustomerTenant.EngagementScore),
            nameof(CustomerTenant.LoyaltyScore),
            nameof(CustomerTenant.ChurnRiskScore),
            nameof(CustomerTenant.NpsScore),
            nameof(CustomerTenant.SatisfactionScore),
            nameof(CustomerTenant.RetentionRate),
            nameof(CustomerTenant.RepeatPurchaseRate),
            nameof(CustomerTenant.AverageTimeBetweenPurchasesDays),
            nameof(CustomerTenant.LastInteractionDate),
            nameof(CustomerTenant.DaysSinceLastInteraction),
            nameof(CustomerTenant.TotalTransactionValue),
            nameof(CustomerTenant.TotalPointsEarned),
            nameof(CustomerTenant.TotalPointsRedeemed),
            nameof(CustomerTenant.CurrentPointsBalance),
            nameof(CustomerTenant.IsActive)
        );
        form.AddOrderBy(nameof(CustomerTenant.TenantId));
        form.AddSubjectColumn<RfmAnalytics>();
        form.AddSubjectColumn<ClvMetrics>();
        form.AddSubjectColumn<AdvancedValueMetrics>();
        form.AddSubjectColumn<EngagementMetrics>();
        form.AddSubjectColumn<ActivityMetrics>();
    }

    protected override void CUDFormsViewModel(CUDForm form)
    {
        form.AddField(nameof(CustomerTenant.Customer));
        form.AddField(nameof(CustomerTenant.Tenant));
        form.AddField(nameof(CustomerTenant.JoinDate));
        form.AddField(nameof(CustomerTenant.LeaveDate));
        form.AddField(nameof(CustomerTenant.IsActive));

        form.AddField(nameof(CustomerTenant.LastInteractionDate));
        form.AddField(nameof(CustomerTenant.RecencyScore));
        form.AddField(nameof(CustomerTenant.TotalInteractions));
        form.AddField(nameof(CustomerTenant.FrequencyScore));
        form.AddField(nameof(CustomerTenant.TotalTransactionValue));
        form.AddField(nameof(CustomerTenant.MonetaryScore));
        form.AddField(nameof(CustomerTenant.RfmSegment));

        form.AddField(nameof(CustomerTenant.CustomerLifetimeValue));
        form.AddField(nameof(CustomerTenant.AverageOrderValue));
        form.AddField(nameof(CustomerTenant.PurchaseFrequency));
        form.AddField(nameof(CustomerTenant.CustomerAcquisitionCost));
        form.AddField(nameof(CustomerTenant.LtvToCacRatio));
        form.AddField(nameof(CustomerTenant.PaybackPeriodDays));
        form.AddField(nameof(CustomerTenant.CustomerProfitMargin));
        form.AddField(nameof(CustomerTenant.ReferralValue));
        form.AddField(nameof(CustomerTenant.RetentionRate));
        form.AddField(nameof(CustomerTenant.RepeatPurchaseRate));
        form.AddField(nameof(CustomerTenant.AverageTimeBetweenPurchasesDays));

        form.AddField(nameof(CustomerTenant.EngagementScore));
        form.AddField(nameof(CustomerTenant.LoyaltyScore));
        form.AddField(nameof(CustomerTenant.ChurnRiskScore));
        form.AddField(nameof(CustomerTenant.NpsScore));
        form.AddField(nameof(CustomerTenant.SatisfactionScore));

        form.AddField(nameof(CustomerTenant.FirstInteractionDate));
        form.AddField(nameof(CustomerTenant.DaysSinceLastInteraction));
        form.AddField(nameof(CustomerTenant.TotalPointsEarned));
        form.AddField(nameof(CustomerTenant.TotalPointsRedeemed));
        form.AddField(nameof(CustomerTenant.CurrentPointsBalance));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst, HyperRoles.MarketingManager, HyperRoles.CallCenterManager];
    }

    public class RfmAnalytics : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(RfmAnalytics);
        public override string Name => "تحلیل RFM";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.Customer), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.Tenant), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.LastInteractionDate));
            AddField(nameof(CustomerTenant.RecencyScore));
            AddField(nameof(CustomerTenant.TotalInteractions));
            AddField(nameof(CustomerTenant.FrequencyScore));
            AddField(nameof(CustomerTenant.TotalTransactionValue));
            AddField(nameof(CustomerTenant.MonetaryScore));
            AddField(nameof(CustomerTenant.RfmSegment));
        }
    }

    public class ClvMetrics : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(ClvMetrics);
        public override string Name => "ارزش طول عمر مشتری";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.Customer), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.Tenant), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.CustomerLifetimeValue));
            AddField(nameof(CustomerTenant.AverageOrderValue));
            AddField(nameof(CustomerTenant.PurchaseFrequency));
            AddField(nameof(CustomerTenant.CustomerAcquisitionCost));
        }
    }

    public class AdvancedValueMetrics : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(AdvancedValueMetrics);
        public override string Name => "معیارهای پیشرفته ارزش";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.Customer), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.Tenant), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.LtvToCacRatio));
            AddField(nameof(CustomerTenant.PaybackPeriodDays));
            AddField(nameof(CustomerTenant.CustomerProfitMargin));
            AddField(nameof(CustomerTenant.ReferralValue));
            AddField(nameof(CustomerTenant.RetentionRate));
            AddField(nameof(CustomerTenant.RepeatPurchaseRate));
            AddField(nameof(CustomerTenant.AverageTimeBetweenPurchasesDays));
        }
    }

    public class EngagementMetrics : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(EngagementMetrics);
        public override string Name => "تعامل و رضایت";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.Customer), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.Tenant), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.EngagementScore));
            AddField(nameof(CustomerTenant.LoyaltyScore));
            AddField(nameof(CustomerTenant.ChurnRiskScore));
            AddField(nameof(CustomerTenant.NpsScore));
            AddField(nameof(CustomerTenant.SatisfactionScore));
        }
    }

    public class ActivityMetrics : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(ActivityMetrics);
        public override string Name => "فعالیت و امتیازات";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.Customer), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.Tenant), eControlPropertyId.ReadOnly);
            AddField(nameof(CustomerTenant.IsActive));
            AddField(nameof(CustomerTenant.JoinDate));
            AddField(nameof(CustomerTenant.LeaveDate));
            AddField(nameof(CustomerTenant.FirstInteractionDate));
            AddField(nameof(CustomerTenant.DaysSinceLastInteraction));
            AddField(nameof(CustomerTenant.TotalPointsEarned));
            AddField(nameof(CustomerTenant.TotalPointsRedeemed));
            AddField(nameof(CustomerTenant.CurrentPointsBalance));
        }
    }
}
