namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

/// <summary>Neo home-page metadata for Hyper business and booth/store synchronization.</summary>
public sealed class HomePageEntityUiDefinitions : CRUDDefinition<HomePageEntity>
{
    private static readonly List<string> AdminRoles = [HyperRoles.Admin];
    public override List<string>? Roles => AdminRoles;

    public sealed class HomePageDashboard : DashboardDefinition
    {
        public override List<string>? Roles => AdminRoles;
        protected override Form Identify() => DefineDashboard("داشبورد هایپریک و یکسان‌سازی");
        protected override void DataSources()
        {
            AddReport<ExternalIntegrationConnection>();
            AddReport<ExternalProductMapping>();
            AddReport<ExternalOrderMapping>();
            AddReport<IntegrationSyncRun>();
            AddReport<IntegrationWebhookInbox>();
            AddReport<IntegrationOutboxMessage>();
            AddReport<IntegrationAdminSimulation>();
            AddReport<IntegrationTokenRequest>();
            AddReport<IntegrationMerchantAccess>();
        }
    }
}
