using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions;
using Neo.Common.Extensions;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class Default : FormDefinition
    {
        protected override Form Identify()
        {
            return DefineForm("صفحه‌ی اول", Form.eFormType.CustomPage);
        }

        protected override void ViewModel()
        {
            AddMultiTab("", "");
            {
                AddHyperMenu();
                AddBPMSMenu();
                EndPart();
            }
        }
    }
    public class ManagerHomePage : Default
    {
        protected override Form Identify()
        {
            return DefineForm("صفحه‌ی اول", Form.eFormType.CustomPage);
        }

        protected override void ViewModel()
        {
            base.ViewModel();
        }
    }

    public partial class Default
    {
        private void AddMenu(string name, string? iconName, string area, string targetAddress,
            string? targetPlace = null, params string[]? paramValues)
        {
            AddMenuWithEnName(null, name, iconName, area, targetAddress, targetPlace, paramValues);
            AddProperty(eControlPropertyId.LinkPathFirstPart, area);
            AddProperty(eControlPropertyId.LinkPathLastPart, targetAddress);
        }

        private void AddMenu<TEntity>(string name, string? iconName = null,
             string? subjectId = null, string? targetPlace = null, params string[]? paramValues)
        {
            var entity = ProjectDefinition.Project.GetEntity<TEntity>();
            if (entity != null)
            {
                // Use entity icon from UIDefinition if iconName is not specified
                var effectiveIcon = iconName ?? entity.Icon;
                AddMenuForFormWithEnName(entity.EnName, name, effectiveIcon, entity.NamespaceId, entity.Id, null,
                    subjectId, targetPlace, paramValues);
            }
        }

        private void AddMenuWithEnName(string? enName, string name, string? iconName = null, string? area = null,
            string? targetAddress = null, string? targetPlace = null, params string[]? paramValues)
        {
            string controlName = $"{area}.{targetAddress}.{targetPlace}";
            if (paramValues?.Length > 0)
            {
                controlName += $".{paramValues[0]}";
            }

            AddControl(eControlTypeId.LinkListItem, controlName, name, enName);
            if (!string.IsNullOrEmpty(iconName))
            {
                AddProperty(eControlPropertyId.IconImage, iconName);
            }

            if (paramValues != null)
            {
                foreach (string paramValue in paramValues)
                {
                    AddProperty(eControlPropertyId.LinkParameter, paramValue);
                }
            }
        }

        private void AddMenuWithEnName<TEntity, TEntityItem>(string? enName, string name, string? iconName = null,
             string? subjectId = null, string? targetPlace = null, params string[]? paramValues)
        {
            var entity = ProjectDefinition.Project.GetEntity<TEntity>();
            // Use entity icon from UIDefinition if iconName is not specified
            var effectiveIcon = iconName ?? entity?.Icon;
            
            if (FindOutParametersByType<TEntity, TEntityItem>(out string namespaceId, out string entityId, out string id, out Type type))
            {
                if (type == typeof(FormDefinition))
                {
                    AddMenuForFormWithEnName(enName, name, effectiveIcon, namespaceId,
                         entityId, id, null, targetPlace, paramValues);
                    return;
                }
                if (type == typeof(ReportDefinition))
                {
                    AddMenuForReport(enName, name, effectiveIcon, namespaceId,
                         entityId, id, null, targetPlace, paramValues);
                    return;
                }
                if (type == typeof(DashboardDefinition))
                {
                    AddMenuForDashboard(enName, name, effectiveIcon, namespaceId,
                         entityId, id, null, targetPlace, paramValues);
                    return;
                }
            }
            if (entity != null)
            {
                AddMenuForFormWithEnName(enName, name, effectiveIcon, entity.NamespaceId, entity.Id, null,
                    subjectId, targetPlace, paramValues);
            }
        }
        private static bool FindOutParametersByType<TEntity, TFormDefinitionItem>(out string namespaceId, out string entityId, out string id, out Type type)
        {
            var entity = ProjectDefinition.Project.GetEntity<TEntity>();
            namespaceId = entity.NamespaceId;
            entityId = entity.Id;
            id = typeof(TFormDefinitionItem).Name;
            type = ReflectionTools.FetchBaseType<TFormDefinitionItem>(typeof(FormDefinition),
                 typeof(ReportDefinition), typeof(DashboardDefinition))!;
            return type != null;
        }
        private void AddMenuForFormWithEnName(string? enName, string name, string? iconName, string namespaceId,
             string entityId, string? formId = null, string? formSubjectId = null,
             string? targetPlace = null, params string[]? paramValues)
        {
            AddMenuForSomeForm(enName, name, iconName, namespaceId, entityId, formId, "Index", formSubjectId, targetPlace, paramValues);
        }
        private void AddMenuForForm(string name, string? iconName, string namespaceId,
             string entityId, string? formId = null, string? formSubjectId = null,
             string? targetPlace = null, params string[]? paramValues)
        {
            AddMenuForSomeForm(null, name, iconName, namespaceId, entityId, formId, "Index", formSubjectId, targetPlace, paramValues);
        }
        private void AddMenuForDashboard(string name, string? iconName, string namespaceId, string entityId, string dashboardId, string? configId = null, string? targetPlace = null, params string[]? paramValues)
        {
            AddMenuForDashboard(null, name, iconName, namespaceId, entityId, dashboardId, configId, targetPlace, paramValues);
        }
        private void AddMenuForDashboard(string? enName, string name, string? iconName, string namespaceId,
            string entityId, string dashboardId, string? configId = null, string? targetPlace = null,
            params string[]? paramValues)
        {
            AddMenuForEntityItem(enName, name, iconName, namespaceId, entityId, configId, targetPlace, paramValues,
                "Dashboard", dashboardId, null);
        }
        private void AddMenuForReport(string name, string? iconName, string namespaceId, string entityId, string reportId, string? configId = null, string? targetPlace = null, params string[]? paramValues)
        {
            AddMenuForReport(null, name, iconName, namespaceId, entityId, reportId, configId, targetPlace, paramValues);
        }
        private void AddMenuForReport(string? enName, string name, string? iconName, string namespaceId, string entityId, string reportId, 
            string? configId = null, string? targetPlace = null, params string[]? paramValues)
        {
            AddMenuForEntityItem(enName, name, iconName, namespaceId, entityId, configId,
                targetPlace, paramValues,
                "Report", reportId, null);
        }

        private void AddMenuForEntityItem(string? enName, string name, string? iconName, string namespaceId,
            string entityId, string? configId, string? targetPlace, string[]? paramValues,
            string pageType, string? entityItemId, string? subjectId)
        {
            AddMenuWithEnName(enName, name, iconName, namespaceId, $"{entityId};{entityItemId}", targetPlace, paramValues);
            AddProperty(eControlPropertyId.PageType, pageType);
            AddProperty(eControlPropertyId.EntityId, entityId);
            AddProperty(eControlPropertyId.EntityItemId, entityItemId);
            if (namespaceId != null)
            {
                AddProperty(eControlPropertyId.NamespaceId, namespaceId);
            }
            if (subjectId != null)
            {
                AddProperty(eControlPropertyId.Subject, subjectId);
            }
            if (configId != null)
            {
                AddProperty(eControlPropertyId.ReportConfigurationId, configId);
            }
        }

        private void AddMenuForSomeForm(string? enName, string name, string? iconName,
             string namespaceId, string entityId, string? formId, string targetAddress,
             string? formSubjectId = null, string? targetPlace = null, string[]? paramValues = null)
        {
            AddMenuForEntityItem(enName, name, iconName, namespaceId, entityId, null,
                targetPlace, paramValues,
                "Form", formId, formSubjectId);
        }
        private void AddMultiTab(string name, string? enName)
        {
            AddControl(eControlTypeId.MultiTab, $"{enName?.Replace(" ", "")}Accordion", name, enName);
            {
                StartSubControls();
            }
        }
        private void AddMultiTabItem(string name, string? enName, ContextualStyle style, string? tooltip = null)
        {
            AddControl(eControlTypeId.MultiTabItem, $"{enName?.Replace(" ", "")}AccordionItem", name, enName);
            AddProperty(eControlPropertyId.ContextualStyle, style);
            AddProperty(eControlPropertyId.Tooltip, tooltip);
            StartSubControls();
        }
        private void AddReportPart(string name, string? enName)
        {
            AddPart(name, enName, "#FFF"); //"#C5E7CB"
        }
        private void AddFormPart(string name, string? enName)
        {
            AddPart(name, enName, "#FFF"); //"#C3E2EE"
        }
        private void AddPart(string name, string? enName, string backgroundColor)
        {
            AddControl(eControlTypeId.LinkList, $"{enName?.Replace(" ", "")}Links", name, enName);
            {
                AddProperty(eControlPropertyId.BackgroundColor, backgroundColor);
                StartSubControls();
            }
        }
        private void AddPartForm<T>(string name, string? subject = null, string? pageSubType = null, string? partName = null, string? entityItemId = null)
        {
            AddPartItem<T>("Form", name, subject, pageSubType, partName, entityItemId);
        }
        private void AddPartReport<T>(string name, string? subject = null, string? partName = null, string? entityItemId = null)
        {
            AddPartItem<T>("Report", name, subject, null, partName, entityItemId);
        }
        private void AddPartDashboard<T>(string name, string? dashboardId = null, string? partName = null)
        {
            AddPartItem<T>("Dashboard", name, null, null, partName, dashboardId);
        }
        private void AddPartItem<T>(string pageType, string name, string? subject, string? pageSubType, string? partName, string? entityItemId = null)
        {
            var entity = ProjectDefinition.Project.GetEntity<T>();
            string namespaceId = entity.NamespaceId;
            string entityId = entity.Id;
            AddControl(eControlTypeId.LinkListItem, $"{partName}:{pageType};{namespaceId};{entityId};{subject};{entityItemId}", name, entityId);
            {
                // Use entity icon from UIDefinition
                if (!string.IsNullOrEmpty(entity.Icon))
                {
                    AddProperty(eControlPropertyId.IconImage, entity.Icon);
                }
                
                if (pageType == "Report")
                {
                    AddProperty(eControlPropertyId.IconClass, ContextualStyle.Success);
                }
                AddProperty(eControlPropertyId.PageType, pageType);
                if (!string.IsNullOrEmpty(pageSubType))
                {
                    AddProperty(eControlPropertyId.PageSubType, pageSubType);
                }

                AddProperty(eControlPropertyId.NamespaceId, namespaceId);
                AddProperty(eControlPropertyId.EntityId, entityId);
                if (entityItemId is not null)
                {
                    AddProperty(eControlPropertyId.EntityItemId, entityItemId);
                }

                if (subject is not null)
                {
                    AddProperty(eControlPropertyId.Subject, subject);
                }
            }
        }
        private void EndPart()
        {
            EndSubControls();
        }
    }
}
