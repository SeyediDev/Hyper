namespace Hyper.AdminPanel.Domain.UiDefinitions.Tenants;

public class TenantAttributeValueUiDefinitions : SubCRUDDefinition<TenantAttributeValue>
{
    public override string? Icon => "fa fa-sliders";
    public override string SubjectId => "Sub";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(TenantAttributeValue.CustomerTenant)
                      , nameof(TenantAttributeValue.Attribute)
                      , nameof(TenantAttributeValue.Value)
                      );
    }
    
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(TenantAttributeValue.CustomerTenant)
                      , nameof(TenantAttributeValue.Attribute)
                      , nameof(TenantAttributeValue.Value)
                      );
    }

    public override void SubViewModel()
    {
        form.AddAllFields(Neo.Bpms.Domain.Models.Cmmn.UI.Forms.FormField.Type.Field);
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        /// <summary>
        /// تقویم روزانه بر اساس TenantAttributeValue
        /// </summary>
        public class DailyCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "تقویم روزانه ویژگی‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(TenantAttributeValue.EventDate), "تاریخ");
                Count(null, "تعداد رویدادها");
            }
        }
    }
}
