using Neo.Bpms.Domain.Features.UiDefinitions;

namespace Hyper.AdminPanel.Domain;

public class HyperProjectDefinition : ProjectMetaDefinition
{
    public override ProjectContext Identify()
    {
        return DefineProject(
             customerName: "پنل مدیریت هایپریک",
             projectName: "پنل مدیریت هایپریک",
             projectCode: "580614",
             startDate: "1405/06/15",
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
