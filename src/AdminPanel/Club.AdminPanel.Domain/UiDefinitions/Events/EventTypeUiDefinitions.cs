using Hyper.Domain.Entities.Channels;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Events;

public partial class EventTypeUiDefinitions : CRUDDefinition<EventType>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-tag";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(EventType.Tenant)
                 , nameof(EventType.Title)
                 , nameof(EventType.Key)
                 , nameof(EventType.AddNewAttributePermission)
                 );
        AddOrderBy(nameof(EventType.Tenant), nameof(EventType.Title));
        AddSubjectColumn<Settings>();
    }
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(EventType.Tenant)
                , nameof(EventType.Title)
                , nameof(EventType.Key)
                , nameof(EventType.AddNewAttributePermission)
                , nameof(EventType.CustomerBehaviorType)
                );
    }

    public class Settings() : SubjectEditForm<Settings>("تنظیمات")
    {
        protected override void ViewModel()
        {
            AddTable<EventChannelValidEvent>("کانال‌های مجاز");
            AddTable<EventTypeValidAttribute>("ویژگی‌های مجاز");
            AddTable<TenantAttribute>("ویژگی‌های خاص این رویداد");

            void AddTable<T>(string title) where T : IEntity
            {
                AddSubTable<T>(nameof(ISubOfEventType.EventType), title);
            }
        }
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
        public class AllEventTypesConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "لیست انواع رویدادها";
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(EventType.Tenant), "اکوسیستم");
                DisplayColumn(nameof(EventType.Title), "عنوان");
                DisplayColumn(nameof(EventType.Key), "کلید");
                OrderBy(nameof(EventType.Title));
            }
        }
        public class MostUsedEventTypesConfig : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "پرکاربردترین انواع رویداد";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventType.Title), "نوع رویداد");
                Count(null, "تعداد استفاده");
                OrderByDesc("COUNT");
            }
        }
    }
}