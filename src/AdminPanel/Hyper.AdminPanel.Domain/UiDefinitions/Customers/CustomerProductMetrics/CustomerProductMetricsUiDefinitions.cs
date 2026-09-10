namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerProductMetrics;

public class CustomerProductMetricsUiDefinitions : CRUDDefinition<Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics>
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
        AddColumns(
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerLifetimeValue),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerAcquisitionCost),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RetentionRate),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RepeatPurchaseRate),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.NextPurchaseProbability),
            nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LastPurchaseDate)
        );

        form.AddOrderBy(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenantId));
        AddSubjectColumn<FinancialMetrics>();
        AddSubjectColumn<PurchaseMetrics>();
        AddSubjectColumn<EngagementMetrics>();
        AddSubjectColumn<PredictiveMetrics>();
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product));

        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerLifetimeValue));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerAcquisitionCost));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PaybackPeriodDays));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin));

        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.AverageOrderValue));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseFrequency));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.AverageTimeBetweenPurchasesDays));

        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RetentionRate));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RepeatPurchaseRate));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.SatisfactionScore));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.EngagementScore));

        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.NextPurchaseProbability));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PredictedNextPurchaseDate));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PredictedNextPurchaseValue));
        AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LastMetricsUpdateDate));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        /// <summary>
        /// ماتریس سودآوری مشتری × محصول
        /// </summary>
        public class CustomerProductProfitabilityMatrixConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => DefaultRoles;
            protected override string Name => "ماتریس سودآوری مشتری × محصول";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری", addAsDisplayColumn: false);
                LayoutColumn(false, $"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", ConfiguredReport.ReportMatrixType.Vertical);
                DisplayColumn($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری");

                GroupByFormula($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product)}.{nameof(Product.Title)}", "محصول", addAsDisplayColumn: false);
                LayoutColumn(false, $"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product)}.{nameof(Product.Title)}", ConfiguredReport.ReportMatrixType.Horizontal);
                DisplayColumn($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product)}.{nameof(Product.Title)}", "محصول");

                SumFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount), "تعداد خرید");
                SumFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue), "مجموع درآمد");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio), "میانگین LTV:CAC");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin), "میانگین حاشیه سود");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RetentionRate), "میانگین نرخ حفظ");
            }

            protected override void DefineSubReports()
            {
                AddSubReport<CustomerProductProfitabilityDetailsConfig>();
            }
        }

        public class CustomerProductProfitabilityDetailsConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => DefaultRoles;
            protected override string Name => "جزئیات سودآوری مشتری-محصول";

            protected override void DefineColumns()
            {
                DisplayColumn($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری");
                DisplayColumn($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product)}.{nameof(Product.Title)}", "محصول");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount), "تعداد خرید");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue), "مجموع درآمد");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerLifetimeValue), "CLV");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerAcquisitionCost), "CAC");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio), "LTV:CAC");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin), "حاشیه سود");
                DisplayColumn(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LastPurchaseDate), "آخرین خرید");
            }
        }

        public class TopProductsByCustomerConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => DefaultRoles;
            protected override string Name => "محصولات برتر هر مشتری";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری");
                GroupByFormula($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product)}.{nameof(Product.Title)}", "محصول");
                SumFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue), "مجموع درآمد");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio), "میانگین LTV:CAC");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin), "میانگین حاشیه سود");
                Count(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount), "تعداد خرید");
            }
        }

        public class TopCustomersByProductConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => DefaultRoles;
            protected override string Name => "مشتریان برتر هر محصول";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product)}.{nameof(Product.Title)}", "محصول");
                GroupByFormula($"{nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری");
                SumFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue), "مجموع درآمد");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio), "میانگین LTV:CAC");
                AverageFormula(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin), "میانگین حاشیه سود");
                Count(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount), "تعداد خرید");
            }
        }
    }

    public class FinancialMetrics : SubjectEditForm2<FinancialMetrics>
    {
        public override string Name => "معیارهای مالی";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerLifetimeValue));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerAcquisitionCost));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LtvToCacRatio));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PaybackPeriodDays));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerProfitMargin));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.ReferralValue));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.TotalRevenue));
        }
    }

    public class PurchaseMetrics : SubjectEditForm2<PurchaseMetrics>
    {
        public override string Name => "معیارهای خرید";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseCount));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.AverageOrderValue));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PurchaseFrequency));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.AverageTimeBetweenPurchasesDays));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.FirstPurchaseDate));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LastPurchaseDate));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.DaysSinceLastPurchase));
        }
    }

    public class EngagementMetrics : SubjectEditForm2<EngagementMetrics>
    {
        public override string Name => "تعامل و وفاداری";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RetentionRate));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.RepeatPurchaseRate));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.SatisfactionScore));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.EngagementScore));
        }
    }

    public class PredictiveMetrics : SubjectEditForm2<PredictiveMetrics>
    {
        public override string Name => "پیش‌بینی";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.CustomerTenant), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.Product), eControlPropertyId.ReadOnly);
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.NextPurchaseProbability));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PredictedNextPurchaseDate));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.PredictedNextPurchaseValue));
            AddField(nameof(Hyper.Domain.Entities.Metrics.Data.CustomerProductMetrics.LastMetricsUpdateDate));
        }
    }
}