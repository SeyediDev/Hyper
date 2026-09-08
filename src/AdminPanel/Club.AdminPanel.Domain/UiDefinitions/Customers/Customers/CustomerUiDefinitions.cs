namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers.Customers;

public partial class CustomerUiDefinitions : CRUDDefinition<Customer>
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
    public override string? Icon => "fa fa-user";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Customer.FirstName),
                        nameof(Customer.LastName),
                        nameof(Customer.MobileNo)
                        );
        AddSubjectColumn<Subs>();
    }
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Customer.FirstName),
                       nameof(Customer.LastName),
                       nameof(Customer.NationalCode),
                       nameof(Customer.MobileNo),
                       nameof(Customer.BirthDate)
                       );
    }
    
    // =====================================================
    // Customer Reports
    // =====================================================

    /// <summary>
    /// گزارش عمومی مشتریان
    /// </summary>
    public new partial class PublicReport: CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst, HyperRoles.MarketingManager, HyperRoles.CallCenterManager];
    }

    public class Subs() : SubjectEditForm<Subs>("جداول مرتبط")
    {
        protected override void ViewModel()
        {
            AddSubTable<CustomerTenant>(nameof(CustomerTenant.Customer), "اکوسیستم‌ها", ContainerControl.None, null, null, nameof(CustomerTenant.Tenant));
        }
    }
}