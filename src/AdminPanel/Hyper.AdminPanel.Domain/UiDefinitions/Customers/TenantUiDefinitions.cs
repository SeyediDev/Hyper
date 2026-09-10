namespace Hyper.AdminPanel.Domain.UiDefinitions.Customers;

public class TenantDefinitions : CRUDDefinition<Tenant>
{
    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(Tenant.Title),
                       nameof(Tenant.ApiKey),
                       nameof(Tenant.CreateDate),
                       nameof(Tenant.CreatedBy)
                       );
        form.AddSubjectColumn<TenantRelations>();
    }
    
    protected override void CUDFormsViewModel(CUDForm form)
    {
        form.AddFields(nameof(Tenant.Title),
                       nameof(Tenant.ApiKey),
                       nameof(Tenant.CreateDate),
                       nameof(Tenant.CreatedBy)
                       );
    }
    public class TenantRelations : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(TenantRelations);
        public override string Name => "اطلاعات وابسته";

        protected override void ViewModel()
        {
            AddField(nameof(Tenant.Title), eControlPropertyId.ReadOnly);
            var groupControlType = eControlTypeId.None;
            AddSubTable(nameof(CustomerTenant), nameof(CustomerTenant.Tenant), null,
                "مشتریان", null, true, groupControlType);
            AddSubTable(nameof(Point), nameof(Point.Tenant), null,
                "امتیازات تعریف شده", null, false, groupControlType);
            AddSubTable(nameof(Reward), nameof(Reward.Tenant), null,
                "پاداش‌های تعریف شده", null, false, groupControlType);
            AddSubTable(nameof(CustomerParameter), nameof(CustomerParameter.Tenant), null,
                "پارامترهای مشتری", null, true, groupControlType);
            AddSubTable(nameof(Promotion), nameof(Promotion.Tenant), null,
				"پویش‌ها و کمپین‌ها", null, false, groupControlType);
        }
    }
}
