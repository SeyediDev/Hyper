namespace Hyper.AdminPanel.Domain.UiDefinitions.Products;

public class ProductCategoryUiDefinitions : CRUDDefinition<ProductCategory>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-cubes";

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(ProductCategory.Tenant),
            nameof(ProductCategory.Title),
            nameof(ProductCategory.Key),
            nameof(ProductCategory.ParentCategory),
            nameof(ProductCategory.DisplayOrder),
            nameof(ProductCategory.IsActive)
        );
        AddSubjectColumn<Attributes>();
        AddSubjectColumn<ProductsTree>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(ProductCategory.Tenant)
                     , nameof(ProductCategory.Title)
                     , nameof(ProductCategory.Key)
                     , nameof(ProductCategory.ParentCategory)
                     , nameof(ProductCategory.Description)
                     , nameof(ProductCategory.DisplayOrder)
                     , nameof(ProductCategory.IsActive)
                     , nameof(ProductCategory.Picture));
    }

    public class Attributes() : SubjectEditForm<Attributes>("ویژگی‌ها")
    {
        protected override void ViewModel()
        {
            AddSubTable<TenantAttribute>(nameof(TenantAttribute.ProductCategory), "ویژگی‌ها", ContainerControl.None);
        }
    }

    public class ProductsTree() : SubjectEditForm<ProductsTree>("درخت محصول")
    {
        protected override void ViewModel()
        {
            AddSubTable<ProductCategory>(nameof(ProductCategory.ParentCategory), "دسته‌بندی زیرمجموعه");
            AddSubTable<Product>(nameof(Product.ProductCategory), "محصولات");
        }
    }
}