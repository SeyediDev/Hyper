using Neo.Bpms.Domain.Features.UiDefinitions;

namespace Hyper.AdminPanel.Domain;

public class HyperProjectDefinition : ProjectMetaDefinition
{
    public override ProjectContext Identify()
    {
        return DefineProject(
             customerName: "پلتفرم باشگاه مشتریان",
             projectName: "پلتفرم باشگاه مشتریان",
             projectCode: "580614",
             startDate: "1404/06/05",
             supportStartDate: "",
             fileMethod: "FileSystem",
             hasDesignFeatures: true
        );
    }

    public override void DefineNamespaceNames()
    {
        _ = AddNamespace<CmmnNamespace>();
        _ = AddNamespace<CmmnConfigNamespace>();
        _ = AddNamespace<ProcessModelNamespace>();
        _ = AddNamespace<ProcessDataNamespace>();

        AddNamespace<HyperNamespace>();
    }

    public override void DefineBPMNDefinitions()
    {
        //DefineBpmnDefinitions<HyperDefinitions>();
    }
}
