using System.Reflection;
using Hyper.Domain.Entities.Database;
using Hyper.Domain.Metadata;
using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions;
using Neo.Bpms.Domain.Models.Cmmn;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Domain.Models.Cmmn.Relationship;

namespace Hyper.AdminPanel.Domain.Domain.Hyper;

/// <summary>
/// Rehydrates the legacy Neo.Bpms metadata at the panel boundary.
///
/// Hyper.Domain owns neutral database metadata so workers and integration
/// services cannot acquire a Neo.Bpms dependency. The panel still needs the
/// old code-first model for its CRUD/report definitions, so this adapter
/// translates the neutral attributes into the Neo model after all Hyper
/// entities have been defined.
/// </summary>
internal static class HyperDomainMetadataAdapter
{
    public static void Apply()
    {
        var model = ProjectDefinition.Project.GetModel(nameof(Hyper));
        if (model is null)
        {
            return;
        }

        foreach (var entity in model.GetEntities().Values)
        {
            ApplyEntityMetadata(entity);
        }

        ConfigureView<SqlVwMarketingsubscriptionmonthly>(SqlVwMarketingsubscriptionmonthlyQuery.Sql);
        ConfigureView<SqlVwMarketinggmvmonthly>(SqlVwMarketinggmvmonthlyQuery.Sql);
    }

    private static void ApplyEntityMetadata(Neo.Bpms.Domain.Models.Cmmn.Entities.Entity entity)
    {
        var clrType = entity.EntityType;
        if (clrType is null)
        {
            return;
        }

        // This was previously inherited from SqlServerEntity's [DontSync]
        // attribute. Keep the behavior panel-local after removing that
        // framework attribute from the shared domain assembly.
        if (typeof(SqlServerEntity).IsAssignableFrom(clrType))
        {
            entity.DontSync = true;
        }

        var classMap = clrType.GetCustomAttribute<DbMapAttribute>(inherit: true);
        if (classMap?.DBName is not null)
        {
            entity.DbTableNameMap = classMap.DBName;
            entity.OldDbTableNameMap = classMap.OldDbName;
        }

        foreach (var property in clrType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var field = entity.GetField(property.Name);
            if (field is null)
            {
                continue;
            }

            var fieldMap = property.GetCustomAttribute<DbMapAttribute>(inherit: true);
            var oldFieldMap = property.GetCustomAttribute<OldDbMapAttribute>(inherit: true);
            if (fieldMap is not null)
            {
                field.SetDbFieldNameMap(fieldMap.DBName, fieldMap.OldDbName);
            }
            else if (oldFieldMap is not null)
            {
                field.SetDbFieldNameMap(null, oldFieldMap.OldDbName);
            }

            var associationMaps = property.GetCustomAttributes<AssociationMapAttribute>(inherit: true).ToArray();
            if (associationMaps.Length == 0 || field.AssociationEntity is null)
            {
                continue;
            }

            field.AssociationEntity.Maps = associationMaps
                .Select(map => new EntityRelationMap
                {
                    SourceField = map.MyField,
                    DestField = map.ObjectField,
                })
                .ToList();

            if (associationMaps.Any(map => map.Bitmask))
            {
                field.Flags |= EntityFieldFlags.IsBitMask;
            }
        }
    }

    private static void ConfigureView<TEntity>(string query)
    {
        var entity = ProjectDefinition.Project.GetEntity<TEntity>();
        if (entity is null)
        {
            return;
        }

        entity.ViewSetting = new ViewSetting
        {
            Query = query,
            IsDbQuery = true,
        };
    }
}
