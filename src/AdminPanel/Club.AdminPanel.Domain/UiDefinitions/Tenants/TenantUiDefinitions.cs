using Hyper.Domain.Entities.Channels;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Tenants;

public class TenantUiDefinitions : CRUDDefinition<Tenant>
{
    public override string? Icon => "fa fa-building";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Tenant.Title), nameof(Tenant.Key));
        AddSubjectColumn<TenantDefinitions>();
        AddSubjectColumn<TenantData>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Tenant.Title), nameof(Tenant.Key));
    }

    public class TenantDefinitions() : SubjectEditForm<TenantDefinitions>("تعاریف")
    {
        protected override void ViewModel()
        {
            var groupControlType = ContainerControl.MultiTab;
            AddTable<TenantAttribute>("ویژگی‌ها", groupControlType, $"{nameof(TenantAttribute.IsActive)}==true");//({nameof(TenantAttribute.Area)}=={(int)AttributeArea.Tenant}) And (
            AddTable<MetricDefinition>("شاخص‌ها");
            AddTable<ProductCategory>("دسته‌بندی محصولات");
            AddTable<Product>("محصولات");
            AddTable<EventChannel>("کانال‌ها");
            AddTable<EventType>("رویداد‌ها");
            AddTable<Point>("امتیازات");
            AddTable<RewardCategory>("طبقه‌بندی پاداش‌ها");
            AddTable<Reward>("پاداش‌ها");
            AddTable<Promotion>("پویش‌ها", groupControlType, $"{nameof(Promotion.Status)}!={(int)PromotionStatus.Cancelled}");
            void AddTable<TTableEntity>(
                string labelName, ContainerControl containerControl = ContainerControl.MultiTab,
                string? filter = null, int? recordCount = null,
                string? tableIndexFormSubjectId = null, string? association = null, bool editable = false, string? enLabelName = null)
                where TTableEntity : IEntity, ISubOfTenant
            {
                AddSubTable<TTableEntity>(nameof(ISubOfTenant.Tenant), labelName,
                    containerControl, filter, recordCount, tableIndexFormSubjectId, association, editable, enLabelName);
            }
        }
    }

    public class TenantData() : SubjectEditForm<TenantData>("اطلاعات")
    {
        protected override void ViewModel()
        {
            AddTable<TenantAttributeValue>("مقادیر ویژگی‌ها", $"(({nameof(TenantAttributeValue.Attribute)}.{nameof(TenantAttribute.Area)})=={(int)AttributeArea.Tenant}) And (({nameof(TenantAttributeValue.Attribute)}.{nameof(TenantAttribute.IsActive)})==true)");
            AddTable<ForumTopic>("انجمن‌ها");
            AddTable<CustomerTenant>("آخرین مشتریان", $"{nameof(CustomerTenant.IsActive)}==true", 20, nameof(Tenant));

            void AddTable<TTableEntity>(
                string labelName, string? filter = null, int? recordCount = null,
                string? tableIndexFormSubjectId = null, ContainerControl containerControl = ContainerControl.MultiTab,
                string? association = null, bool editable = false, string? enLabelName = null)
                where TTableEntity : IEntity
            {
                AddSubTable<TTableEntity>(nameof(ISubOfTenant.Tenant), labelName,
                    containerControl, filter, recordCount, tableIndexFormSubjectId, association, editable, enLabelName);
            }
        }
    }
}