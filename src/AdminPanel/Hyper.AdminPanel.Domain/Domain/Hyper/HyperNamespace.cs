using Neo.Bpms.Domain.Models.Cmmn.Partitions;

namespace Hyper.AdminPanel.Domain.Domain.Hyper;

public class HyperNamespace : ModelDefinition<HyperNamespace>
{
    protected override bool Identify()
    {
        return DefineModel(nameof(Hyper), "پنل مدیریتی هایپریک", null, nameof(DomainProvider.Domain));
    }
    protected override void Partitions()
    {
        AddPartitionFunction("pfArchive", typeof(bool),
            PartitionFunctionType.FixRange,
            PartitionFunctionBoundaryType.Left,
            "0", "1", "0", "1");
        AddArchivePartitionScheme(nameof(DomainSchema.CoreConfig));
        AddArchivePartitionScheme(nameof(DomainSchema.Core));
        AddArchivePartitionScheme(nameof(DomainSchema.CoreLog));
    }

    protected override void Entities()
    {
        // HomePageEntity is Neo's metadata anchor. Business entities are loaded
        // explicitly by their own Hyper definitions; do not scan the copied Club
        // assemblies here because that registers Club tables in the admin model.
        DefineEntity<HomePageEntity>();
        DefineEntity<ExternalIntegrationConnection>();
        DefineEntity<ExternalProductMapping>();
        DefineEntity<ExternalOrderMapping>();
        DefineEntity<IntegrationSyncRun>();
        DefineEntity<IntegrationWebhookInbox>();
        DefineEntity<IntegrationOutboxMessage>();
        DefineEntity<IntegrationAdminSimulation>();
        DefineEntity<IntegrationTokenRequest>();
        DefineEntity<IntegrationMerchantAccess>();
	}

    private void AddArchivePartitionScheme(string name)
    {
        AddPartitionScheme($"Archive_{name}", "pfArchive",
            FileGroupSelectionType.FromList,
            "", name, $"{name}_Archive");
    }
}
