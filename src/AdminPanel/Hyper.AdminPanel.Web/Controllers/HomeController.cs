using Microsoft.Extensions.Options;
using Neo.Bpms.Domain.Expressions.Model.ExpressionNodes;
using Neo.Bpms.Domain.Expressions.Parsers;
using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Reports;
using Neo.Bpms.Infrastructure.Features.SystemConfigs;
using Neo.Bpms.UI.MVC.Controllers;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Neo.Bpms.UI.MVC.Features;
using Neo.Common.Extensions;
using Neo.Domain.Entities.Base;
using static Neo.Bpms.Domain.Models.Cmmn.AutoCalc;

namespace Hyper.AdminPanel.Web.Controllers;

public class HomeController : DesktopController
{
    private static int _initializationRequested;
    private static Task? _modelMappingInitializationTask;
    public HomeController(ILogger<HomeController> logger,
        DashboardStructRoutines dashboardStructRoutines, DashboardConfigManager dashboardConfigManager,
        FormStructRoutines formStructRoutines, ControllerMethods controllerMethods,
        ReportConfigManager reportConfigManager, 
        FilterConfigBackupRestore filterConfigBackupRestore, 
        FilterManager filterController, IOptions<CmmnSettings> cmmnSettings,
        IServiceScopeFactory serviceScopeFactory) :
        base(dashboardStructRoutines,
            dashboardConfigManager,
            formStructRoutines,
            controllerMethods,  
            reportConfigManager,
            filterConfigBackupRestore,
            filterController, cmmnSettings)
    {
        DashboardStructRoutines = dashboardStructRoutines;
        //EnsureModelMappingInitialized(serviceScopeFactory, logger);
    }

    private static void EnsureModelMappingInitialized(IServiceScopeFactory serviceScopeFactory, ILogger<HomeController> logger)
    {
        if (Volatile.Read(ref _initializationRequested) == 1)
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _initializationRequested, 1, 0) != 0)
        {
            return;
        }

        _modelMappingInitializationTask = Task.Run(() => InitializeModelMappingInBackground(serviceScopeFactory, logger));
    }

    private static void InitializeModelMappingInBackground(IServiceScopeFactory serviceScopeFactory, ILogger<HomeController> logger)
    {
        try
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            IHyperUnitOfWorkCommand scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IHyperUnitOfWorkCommand>();
            UpdateModelMapping(scopedUnitOfWork, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update model mapping using background scope");
            Interlocked.Exchange(ref _initializationRequested, 0);
        }
    }

    private static void UpdateModelMapping(IHyperUnitOfWorkCommand commandHyperUnitOfWork, ILogger<HomeController> logger)
    {
        foreach (Entity entity in ProjectDefinition.Project.Entities.Values)
        {
            if (entity.Provider is not nameof(DomainProvider.Domain))
            {
                entity.Provider = nameof(DomainProvider.Domain);
            }
            if (entity.EntityType is not null && ReflectionTools.IsInBaseInterface<IDomainEventEntity>(entity.EntityType))
            {
                UpdateModelMapping(commandHyperUnitOfWork, logger, entity);
            }
        }
    }

    private static void UpdateModelMapping(IHyperUnitOfWorkCommand commandHyperUnitOfWork, ILogger<HomeController> logger, Entity entity)
    {
        if (entity.NotMapped)
        {
            return;
        }
        EntityTableInfo tableInfo = commandHyperUnitOfWork.GetEntityTableInfo(entity.EntityType)!;
        if (tableInfo == null)
        {
            return;
        }
        if (tableInfo.Schema is not null && entity.Schema is null)
        {
            entity.Schema = tableInfo.Schema;
        }
        entity.DbTableNameMap = tableInfo.TableName;
        Dictionary<string, EntityField> keyFieldsById = entity.KeyFields?
            .Where(keyField => keyField != null)
            .ToDictionary(keyField => keyField.Id) ?? [];

        bool autoCalcsInitialized = false;
        ExpressionTree? autoIncrementFormula = null;

        foreach (EntityFieldColumnInfo pkInfo in tableInfo.PrimaryKeys)
        {
            if (!entity.entityFields.TryGetValue(pkInfo.Id, out EntityField? entityField) || entityField == null)
            {
                logger.LogError("In class {className} Have pk {pk} that not exists in class", entity.Id, pkInfo.Id);
                continue;
            }
            if (!keyFieldsById.TryGetValue(pkInfo.Id, out EntityField? keyField) || keyField == null)
            {
                logger.LogError("In class {className} Have pk {pk} that not map in class", entity.Id, pkInfo.Id);
                continue;
            }
            if (pkInfo.IsIdentity &&
                !entity.AutoCalcs.Calculations.Any(
                    a=> a.GenerationType== AutoCalc.eGenerationType.DBInsert &&
                        a.FieldId== pkInfo.Id
                    ))
            {
                if (!autoCalcsInitialized)
                {
                    entity.InitAutoCalcs();
                    autoCalcsInitialized = true;
                    autoIncrementFormula = Parser.ParseTree("AutoIncrement()");
                }
                entity.AutoCalcs.AddAutoCalc(new AutoCalc
                {
                    FieldId = pkInfo.Id,
                    GenerationType = AutoCalc.eGenerationType.DBInsert,
                    Formula = autoIncrementFormula!,
                    Condition = null,
                    Loaction = AutoCalcLocation.BeforeValidation,
                    IfNull = false,
                    RecalcOnAnyChange = false,
                });
            }
        }
        foreach (EntityField entityField in entity.entityFields.Values)
        {
            if (entityField.NotMapped || entityField.NotMap)
            {
                continue;
            }
            if(!string.IsNullOrEmpty(entityField.GetSetDBFieldNameMap()))
            {
                continue;
            }
            if (tableInfo.Properties.TryGetValue(entityField.Id, out EntityFieldColumnInfo? fieldInfo) && fieldInfo != null)
            {
                if (fieldInfo.Name is not null)
                {
                    entityField.DbFieldName = fieldInfo.Name;
                }
                if (fieldInfo.Comment is not null)
                {
                    entityField.Name = fieldInfo.Comment;
                }
            }
            else
            {
                logger.LogError("In class {className} Have field Id {entityField} that not map in db", entity.Id, entityField.Id);
            }
        }
        foreach (EntityFieldColumnInfo fieldInfo in tableInfo.Properties.Values)
        {
            if (!entity.entityFields.ContainsKey(fieldInfo.Id))
            {
                logger.LogError("In class {className} Have db column {column} that not exists in class ", entity.Id, fieldInfo.Id);
            }
        }
    }

    public DashboardStructRoutines DashboardStructRoutines { get; }
}
