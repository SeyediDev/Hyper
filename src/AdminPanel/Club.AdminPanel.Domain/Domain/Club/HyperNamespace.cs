using Neo.Bpms.Domain.Models.Cmmn.Partitions;
using Neo.Domain.Entities.Common;

namespace Hyper.AdminPanel.Domain.Domain.Hyper;

public class HyperNamespace : ModelDefinition<HyperNamespace>
{
    protected override bool Identify()
    {
        return DefineModel(nameof(Domains.Hyper), "پلتفرم باشگاه مشتریان", null, nameof(DomainProvider.Domain));
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
        DefineEntity<HomePageEntity>();
        DefineEntities<IDomainEventEntity>(typeof(Language).Assembly);
        DefineEntities<IDomainEventEntity>(typeof(Point).Assembly);
		DefineEntities<IView>(typeof(Language).Assembly);
		DefineEntities<IView>(typeof(Point).Assembly);
	}

    private void AddArchivePartitionScheme(string name)
    {
        AddPartitionScheme($"Archive_{name}", "pfArchive",
            FileGroupSelectionType.FromList,
            "", name, $"{name}_Archive");
    }
}