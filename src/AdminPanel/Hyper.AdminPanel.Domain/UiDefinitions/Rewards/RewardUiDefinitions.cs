namespace Hyper.AdminPanel.Domain.UiDefinitions.Rewards;

public class RewardUiDefinitions : CRUDDefinition<Reward>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager,
        HyperRoles.FinanceManager
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-gift";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Reward.Title),
                        nameof(Reward.RewardCategory),
                        nameof(Reward.Merchant),
                        nameof(Reward.Value),
                        nameof(Reward.Quantity),
                        nameof(Reward.TotalSales),
                        nameof(Reward.PopularityScore),
                        nameof(Reward.StockStatus),
                        nameof(Reward.Tenant),
                        nameof(Reward.CustomerSegment)
                        );
        AddSubjectColumn<RewardRelations>();
        AddSubjectColumn<Analytics>();
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Reward.Title),
                       nameof(Reward.RewardCategory),
                       nameof(Reward.Merchant),
                       nameof(Reward.Tenant),
                       nameof(Reward.CustomerSegment),
                       nameof(Reward.Value),
                       nameof(Reward.PointLevel),
                       nameof(Reward.ControlAsset),
                       nameof(Reward.Quantity),
                       nameof(Reward.SerialFormat),
                       nameof(Reward.Visible),
                       nameof(Reward.OrderId),
                       nameof(Reward.LowStockThreshold),
                       nameof(Reward.ActualCost)
                       );
        // Add Picture field as File control (not ComboBox)
        AddField(nameof(Reward.Picture), eControlTypeId.File);
    }

    public class RewardRelations() : SubjectEditForm<RewardRelations>("هزینه‌ها")
    {
        protected override void ViewModel()
        {
            AddSubTable<RewardCost>(nameof(RewardCost.Reward), "هزینه‌ها", ContainerControl.None, null, null, "Sub");
        }
    }

    public class Analytics() : SubjectEditForm<Analytics>("تحلیل عملکرد")
    {
        protected override void ViewModel()
        {
            AddField(nameof(Reward.Title), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.TotalSales), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.TotalViews), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.ConversionRate), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.AverageRating), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.ReviewCount), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.LastPurchaseDate), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.PopularityScore), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.StockStatus), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.ActualCost), eControlPropertyId.ReadOnly);
            AddField(nameof(Reward.ProfitMargin), eControlPropertyId.ReadOnly);
        }
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
        // =====================================================
        // Reward Analytics Reports
        // =====================================================

        public class PopularRewardsConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "پاداش‌های محبوب";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Reward.Title), "پاداش");
                Sum(nameof(Reward.TotalSales), "تعداد فروش");
                Average(nameof(Reward.PopularityScore), "نمره محبوبیت");
                OrderByDesc(nameof(Reward.TotalSales));
            }
        }

        public class RewardsByCategoryConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "پاداش‌ها به تفکیک دسته";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Reward.RewardCategory), "دسته");
                Count(null, "تعداد پاداش‌ها");
                Sum(nameof(Reward.TotalSales), "مجموع فروش");
            }
        }

        public class RewardConversionRateConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "نرخ تبدیل پاداش‌ها";
            
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Reward.Title), "پاداش");
                DisplayColumn(nameof(Reward.TotalViews), "بازدید");
                DisplayColumn(nameof(Reward.TotalSales), "فروش");
                DisplayColumn(nameof(Reward.ConversionRate), "نرخ تبدیل (%)");
                DisplayColumn(nameof(Reward.PopularityScore), "محبوبیت");
                OrderByDesc(nameof(Reward.ConversionRate));
            }
        }

        public class RewardProfitabilityConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "سودآوری پاداش‌ها";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Reward.Title), "پاداش");
                DisplayColumn(nameof(Reward.Value), "ارزش امتیازی");
                DisplayColumn(nameof(Reward.ActualCost), "هزینه واقعی");
                DisplayColumn(nameof(Reward.ProfitMargin), "حاشیه سود (%)");
                DisplayColumn(nameof(Reward.TotalSales), "تعداد فروش");
                OrderByDesc(nameof(Reward.ProfitMargin));
            }
        }

        public class RewardCostAnalysisConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.FinanceManager];
            protected override string Name => "تحلیل هزینه پاداش‌ها";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Reward.Title), "پاداش");
                DisplayColumn(nameof(Reward.ActualCost), "هزینه واقعی");
                DisplayColumn(nameof(Reward.Value), "ارزش امتیازی");
                DisplayColumn(nameof(Reward.ProfitMargin), "حاشیه سود");
            }
        }

        public class RewardStockStatusConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "وضعیت موجودی پاداش‌ها";
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Reward.Title), "پاداش");
                DisplayColumn(nameof(Reward.Quantity), "موجودی");
                DisplayColumn(nameof(Reward.LowStockThreshold), "آستانه کمبود");
                DisplayColumn(nameof(Reward.StockStatus), "وضعیت");
                OrderBy(nameof(Reward.Quantity));
            }
        }

        public class RewardSatisfactionConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "رضایت از پاداش‌ها";
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Reward.Title), "پاداش");
                DisplayColumn(nameof(Reward.AverageRating), "میانگین امتیاز");
                DisplayColumn(nameof(Reward.ReviewCount), "تعداد نظرات");
                DisplayColumn(nameof(Reward.TotalSales), "فروش");
                OrderByDesc(nameof(Reward.AverageRating));
            }
        }

        public class OutdatedRewardsConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(Reward.LastPurchaseDate)} < DateTime.UtcNow.AddMonths(-3)";
            protected override string Name => "پاداش‌های قدیمی";
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Reward.Title), "پاداش");
                DisplayColumn(nameof(Reward.LastPurchaseDate), "آخرین خرید");
                DisplayColumn(nameof(Reward.TotalSales), "کل فروش");
                DisplayColumn(nameof(Reward.Quantity), "موجودی");
                OrderBy(nameof(Reward.LastPurchaseDate));
            }
        }
    }
}