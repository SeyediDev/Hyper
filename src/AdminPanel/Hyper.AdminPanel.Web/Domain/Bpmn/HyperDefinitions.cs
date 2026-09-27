using Hyper.Domain.Entities.Common;
using Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

namespace Hyper.AdminPanel.Domain.Domain.Hyper;

public partial class HyperDefinitions : BpmnDefinitionsDefinition
{
    protected override bool Identify()
    {
        return Identify(nameof(Domains.Hyper));
    }

    protected override void DefineProcess()
    {
    }
}