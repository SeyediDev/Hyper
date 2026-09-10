namespace Hyper.AdminPanel.Domain.UiDefinitions.Products;

public class ProductUiDefinitions : CRUDDefinition<Product>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-cube";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Product.Title),
                        nameof(Product.Tenant),
                        nameof(Product.ProductCategory),
                        nameof(Product.IsActive)
                        );
        AddSubjectColumn<Attributes>();
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Product.Title),
                       nameof(Product.Key),
                       nameof(Product.Description),
                       nameof(Product.Tenant),
                       nameof(Product.ProductCategory),
                       nameof(Product.IsActive),
                       nameof(Product.ExpectedConsumptionDuration),
                       nameof(Product.TypicalUsageFrequency),
                       nameof(Product.AveragePurchaseCycle),
                       nameof(Product.ReorderThreshold),
                       nameof(Product.TypicalCustomerLifetime),
                       nameof(Product.AveragePurchasesPerCustomerLifetime),
                       nameof(Product.RepeatPurchaseRate)
                       );
        AddField(nameof(Product.Picture), eControlTypeId.File);
    }

    public class Attributes : SubjectEditForm2<Attributes>
    {
        public override string Name => "ویژگی‌ها";
        protected override void ViewModel()
        {
            AddSubTable<TenantAttribute>(nameof(TenantAttribute.Product), "ویژگی‌ها", ContainerControl.None);
        }
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        public class ActiveProductsConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(Product.IsActive)} == true";
            protected override string Name => "محصولات فعال";
            
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Product.Title), "عنوان");
                DisplayColumn(nameof(Product.ProductCategory), "دسته‌بندی");
            }
        }

        public class PopularProductsConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string WhereCondition => $"{nameof(Product.IsActive)} == true";
            protected override string Name => "محبوب‌ترین محصولات";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Product.Title), "محصول");
                OrderByDesc("SUM");
            }
        }

		public class ProductsByCategoryConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "محصولات به تفکیک اکوسیستم";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Product.Tenant), "اکوسیستم");
                Count(null, "تعداد");
            }
        }

        public class ProductsByTypeConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "محصولات به تفکیک نوع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Product.ProductCategory), "دسته‌بندی"); // Changed from ProductType to Category
                Count(null, "تعداد");
            }
        }

		public class ProductsByTenantConfig : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "محصولات به تفکیک اکوسیستم";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Product.Tenant), "اکوسیستم");
                Count(null, "تعداد محصولات");
                OrderByDesc("SUM");
            }
        }

        public class HighPointProductsConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(Product.IsActive)} == true"; // PointsEarnable removed - use promotion rules instead
            protected override string Name => "محصولات با بیشترین امتیاز";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Product.Title), "عنوان");
                DisplayColumn(nameof(Product.ProductCategory), "دسته‌بندی");
            }
        }

        public class ProductRepeatPurchaseConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string WhereCondition => $"{nameof(Product.RepeatPurchaseRate)} > 0";
            protected override string Name => "نرخ تکرار خرید محصولات";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Product.Title), "محصول");
                DisplayColumn(nameof(Product.RepeatPurchaseRate), "نرخ تکرار خرید");
                DisplayColumn(nameof(Product.AveragePurchaseCycle), "چرخه خرید");
                OrderByDesc(nameof(Product.RepeatPurchaseRate));
            }
        }

        public class ProductCLVConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string WhereCondition => $"{nameof(Product.IsActive)} == true";
            protected override string Name => "ارزش طول عمر مشتری محصولات";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Product.Title), "محصول");
                DisplayColumn(nameof(Product.AveragePurchasesPerCustomerLifetime), "میانگین خرید");
                DisplayColumn(nameof(Product.TypicalCustomerLifetime), "طول عمر مشتری");
                DisplayColumn(nameof(Product.ProductCategory), "دسته‌بندی");
            }
        }

        public class ProductCustomerMatrixConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "ماتریس محصول × مشتری";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                GroupBy($"{nameof(Product.Title)}", "محصول", true, ConfiguredReport.ReportMatrixType.Vertical);
                GroupByFormula($"{nameof(Product.Tenant)}.{nameof(Tenant.Title)}", "اکوسیستم", true, ConfiguredReport.ReportMatrixType.Horizontal);

            }
        }
    }
}