using Hyper.Domain.Entities.Channels;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Events;

public partial class EventChannelUiDefinitions : CRUDDefinition<EventChannel>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-plug";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(EventChannel.Tenant),
                        nameof(EventChannel.Title),
                        nameof(EventChannel.Key));
        form.AddOrderBy(nameof(EventChannel.Tenant));
        form.AddOrderBy(nameof(EventChannel.Title));
        AddSubjectColumn<EventChannelRelations>();
        AddSubjectColumn<Attributes>();
    }
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(EventChannel.Tenant),
                       nameof(EventChannel.Title),
                       nameof(EventChannel.Key));
    }

    public class EventChannelRelations : SubjectEditForm2<EventChannelRelations>
    {
        public override string Name => "قواعد و محدودیت‌ها";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddSubTable(nameof(EventChannelValidEvent), nameof(EventChannelValidEvent.EventChannel), "Sub",
                "نوع رویدادهای مجاز", null, true, ContainerControl.MultiTab);
            AddSubTable(nameof(EventChannelValidIp), nameof(EventChannelValidIp.EventChannel), "Sub",
                "آدرس‌های مجاز", null, true, ContainerControl.MultiTab);
        }
    }

    public class Attributes : SubjectEditForm2<Attributes>
    {
        public override string Name => "ویژگی‌ها";
        protected override void ViewModel()
        {
            AddSubTable<TenantAttribute>(nameof(TenantAttribute.Channel), "ویژگی‌ها", ContainerControl.None);
        }
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        /// <summary>
        /// گزارش لیست کانال‌های رویداد
        /// </summary>
        public class AllEventChannelsConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "لیست کانال‌های رویداد";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(EventChannel.Tenant), "اکوسیستم");
                DisplayColumn(nameof(EventChannel.Title), "عنوان");
                DisplayColumn(nameof(EventChannel.Key), "کلید");
                OrderBy(nameof(EventChannel.Title));
            }
        }

        /// <summary>
        /// گزارش پرکاربردترین کانال‌ها
        /// </summary>
        public class MostUsedChannelsConfig : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "پرکاربردترین کانال‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventChannel.Title), "کانال");
                Count(null, "تعداد استفاده");
                OrderByDesc("COUNT");
            }
        }
    }
}

