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
    public override string? Icon => "fa fa-id-card";
    protected override void RefineEntity()
    {
        RenameField(nameof(CustomerTenant.CreateDate), "تاریخ عضویت", "Join Date");
    }

    public class TanantIndexForm : SubjectIndexForm<Tenant>
    {
        protected override void ViewModel()
        {
            AddColumns(nameof(CustomerTenant.Customer)
                      , nameof(CustomerTenant.Province)
                      , nameof(CustomerTenant.City)
                      , nameof(CustomerTenant.JoinToTenantDate)
                      , nameof(CustomerTenant.LastInteractionDate)
                      );
            AddOrderBy(nameof(CustomerTenant.CreateDate), SortType.Descending);
        }
    }

    protected override void IndexFormViewModel()
    {
        AddColumns(
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
        AddOrderBy(nameof(CustomerTenant.TenantId));
        AddSubjectColumn<RfmAnalytics>();
        AddSubjectColumn<ClvMetrics>();
        AddSubjectColumn<AdvancedValueMetrics>();
        AddSubjectColumn<EngagementMetrics>();
        AddSubjectColumn<ActivityMetrics>();
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(CustomerTenant.Customer));
        AddField(nameof(CustomerTenant.Tenant));
        AddField(nameof(CustomerTenant.JoinDate));
        AddField(nameof(CustomerTenant.LeaveDate));
        AddField(nameof(CustomerTenant.IsActive));

        AddField(nameof(CustomerTenant.LastInteractionDate));
        AddField(nameof(CustomerTenant.RecencyScore));
        AddField(nameof(CustomerTenant.TotalInteractions));
        AddField(nameof(CustomerTenant.FrequencyScore));
        AddField(nameof(CustomerTenant.TotalTransactionValue));
        AddField(nameof(CustomerTenant.MonetaryScore));
        AddField(nameof(CustomerTenant.RfmSegment));

        AddField(nameof(CustomerTenant.CustomerLifetimeValue));
        AddField(nameof(CustomerTenant.AverageOrderValue));
        AddField(nameof(CustomerTenant.PurchaseFrequency));
        AddField(nameof(CustomerTenant.CustomerAcquisitionCost));
        AddField(nameof(CustomerTenant.LtvToCacRatio));
        AddField(nameof(CustomerTenant.PaybackPeriodDays));
        AddField(nameof(CustomerTenant.CustomerProfitMargin));
        AddField(nameof(CustomerTenant.ReferralValue));
        AddField(nameof(CustomerTenant.RetentionRate));
        AddField(nameof(CustomerTenant.RepeatPurchaseRate));
        AddField(nameof(CustomerTenant.AverageTimeBetweenPurchasesDays));

        AddField(nameof(CustomerTenant.EngagementScore));
        AddField(nameof(CustomerTenant.LoyaltyScore));
        AddField(nameof(CustomerTenant.ChurnRiskScore));
        AddField(nameof(CustomerTenant.NpsScore));
        AddField(nameof(CustomerTenant.SatisfactionScore));

        AddField(nameof(CustomerTenant.FirstInteractionDate));
        AddField(nameof(CustomerTenant.DaysSinceLastInteraction));
        AddField(nameof(CustomerTenant.TotalPointsEarned));
        AddField(nameof(CustomerTenant.TotalPointsRedeemed));
        AddField(nameof(CustomerTenant.CurrentPointsBalance));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst, HyperRoles.MarketingManager, HyperRoles.CallCenterManager];

        /// <summary>
        /// تقویم جذب مشتریان بر اساس تاریخ عضویت
        /// </summary>
        public class CustomerJoinCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst, HyperRoles.MarketingManager];
            protected override string Name => "تقویم جذب مشتریان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(CustomerTenant.JoinToTenantDate), "تاریخ عضویت");
                Count(null, "تعداد مشتریان جدید");
            }
        }
    }

    public class RfmAnalytics : SubjectEditForm2<RfmAnalytics>
    {
        public override string Name => "تحلیل RFM";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.LastInteractionDate));
            AddField(nameof(CustomerTenant.RecencyScore));
            AddField(nameof(CustomerTenant.TotalInteractions));
            AddField(nameof(CustomerTenant.FrequencyScore));
            AddField(nameof(CustomerTenant.TotalTransactionValue));
            AddField(nameof(CustomerTenant.MonetaryScore));
            AddField(nameof(CustomerTenant.RfmSegment));
        }
    }

    public class ClvMetrics : SubjectEditForm2<ClvMetrics>
    {
        public override string Name => "ارزش طول عمر مشتری";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.CustomerLifetimeValue));
            AddField(nameof(CustomerTenant.AverageOrderValue));
            AddField(nameof(CustomerTenant.PurchaseFrequency));
            AddField(nameof(CustomerTenant.CustomerAcquisitionCost));
        }
    }

    public class AdvancedValueMetrics : SubjectEditForm2<AdvancedValueMetrics>
    {
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

    public class EngagementMetrics : SubjectEditForm2<EngagementMetrics>
    {
        public override string Name => "تعامل و رضایت";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(CustomerTenant.EngagementScore));
            AddField(nameof(CustomerTenant.LoyaltyScore));
            AddField(nameof(CustomerTenant.ChurnRiskScore));
            AddField(nameof(CustomerTenant.NpsScore));
            AddField(nameof(CustomerTenant.SatisfactionScore));
        }
    }

    public class ActivityMetrics : SubjectEditForm2<ActivityMetrics>
    {
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
