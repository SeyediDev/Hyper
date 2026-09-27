using Neo.Bpms.Domain.Models.Service.Internal;

namespace Hyper.AdminPanel.Domain.Domain.Bpmn;

public class HyperOperationsInterface : InternalServiceGroupDefinition
{
    public HyperOperationsInterface() : base(nameof(HyperOperationsInterface))
    {
    }


    public override void AddOperations()
    {
        //AddOperation(new CalcBillingOperation());
    }
}
