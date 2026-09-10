using Hyper.Domain.Entities.Channels;
using Hyper.Domain.Entities.Events.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions : CRUDDefinition<HomePageEntity>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void Forms()
    {
        //DefineForm<ManagerHomePage>();
        //DefineForm<Default>();
    }

    public partial class HomePageDashboard : DashboardDefinition
    {
        public override List<string>? Roles => DefaultRoles;

        protected override Form Identify()
        {
            return DefineDashboard("داشبورد اصلی هایپریک");
        }

        protected override void Filters()
        {
        }

        protected override void DataSources()
        {
            base.DataSources();
            AddReport<Customer>();
            AddReport<CustomerTenant>();
            AddReport<CustomerTransaction>();
            AddReport<RewardAsset>();
            AddReport<Reward>();
            AddReport<Product>();
            AddReport<ProductFitAnalysis>();
            AddReport<CustomerSegment>();
            AddReport<EventType>();
            AddReport<EventChannel>();
            AddReport<EventLog>();
            AddReport<Promotion>();
            AddReport<Point>();
            AddReport<Survey>();
        }
    }
}
