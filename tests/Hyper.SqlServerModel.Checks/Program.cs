using System.Text.Json;
using Hyper.Domain.Entities.Database;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Neo.Domain.Entities.Base;

var options = new DbContextOptionsBuilder<HyperSqlServerContext>()
    .UseSqlServer("Server=.;Database=Hyperyek;Integrated Security=True;TrustServerCertificate=True").Options;
if (args.Length != 0 && (args.Length != 2 || args[0] is not ("--live" or "--live-local" or "--simulation-sql-local" or "--outbox-sql-local" or "--overview-sql-local")))
    throw new ArgumentException("Unknown check arguments; invoke the compiled DLL with exactly one mode and settings path.");
using var db = new HyperSqlServerContext(options);
var model = db.GetService<IDesignTimeModel>().Model;
using var snapshot = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "hyper-schema.json")));
var data = snapshot.RootElement;
var objects = data.GetProperty("objects").EnumerateArray().ToArray();
var columns = data.GetProperty("columns").EnumerateArray().ToArray();
var indexes = data.GetProperty("indexes").EnumerateArray().ToArray();
int count = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    count++;
}
string Text(JsonElement value, string property) => value.GetProperty(property).GetString()!;
int Int(JsonElement value, string property) => value.GetProperty(property).GetInt32();
bool Flag(JsonElement value, string property) => value.GetProperty(property).GetBoolean();
Check(model.GetEntityTypes().Count() == objects.Length, "Object count mismatch or accidental inheritance mapping.");
foreach (var obj in objects)
{
    var view = Text(obj, "type").Trim() == "V";
    var name = Text(obj, "name");
    var schema = Text(obj, "schema");
    var entity = model.GetEntityTypes().Single(e => view
        ? e.GetViewName() == name && e.GetViewSchema() == schema
        : e.GetTableName() == name && e.GetSchema() == schema);
    var table = view ? StoreObjectIdentifier.View(name, schema) : StoreObjectIdentifier.Table(name, schema);
    var objectColumns = columns.Where(c => Int(c, "object_id") == Int(obj, "object_id")).ToArray();
    Check(typeof(IEntity).IsAssignableFrom(entity.ClrType), $"{name}: Neo IEntity missing");
    Check(entity.GetProperties().Count() == objectColumns.Length, $"{name}: extra or missing columns");
    Check(entity.FindProperty("DomainEvents") is null, $"{name}: domain events mapped to database");
    foreach (var column in objectColumns)
    {
        var property = entity.GetProperties().Single(p => p.GetColumnName(table) == Text(column, "name"));
        Check(property.IsNullable == Flag(column, "is_nullable"), $"{name}.{property.Name}: nullability mismatch");
        var type = Text(column, "sql_type");
        var expectedType = type switch
        {
            "char" or "nchar" or "varchar" or "nvarchar" or "binary" or "varbinary" =>
                $"{type}({(Int(column, "max_length") == -1 ? "max" : (Int(column, "max_length") / (type.StartsWith('n') ? 2 : 1)).ToString())})",
            "decimal" or "numeric" => $"{type}({Int(column, "precision")},{Int(column, "scale")})",
            "datetime2" or "datetimeoffset" or "time" => $"{type}({Int(column, "scale")})",
            _ => type
        };
        Check(property.GetColumnType() == expectedType, $"{name}.{property.Name}: SQL type mismatch");
        Check(property.GetDefaultValueSql() == column.GetProperty("default_sql").GetString(), $"{name}.{property.Name}: default mismatch");
        Check(property.GetComputedColumnSql() == column.GetProperty("computed_sql").GetString(), $"{name}.{property.Name}: computed expression mismatch");
        Check((property.GetValueGenerationStrategy() == SqlServerValueGenerationStrategy.IdentityColumn) == Flag(column, "is_identity"), $"{name}.{property.Name}: identity mismatch");
    }
    var pk = indexes.Where(i => Int(i, "object_id") == Int(obj, "object_id") && Flag(i, "is_primary_key"))
        .OrderBy(i => Int(i, "key_ordinal")).ToArray();
    var expectedKey = pk.Select(i => Text(objectColumns.Single(c => Int(c, "column_id") == Int(i, "column_id")), "name"));
    var actualKey = entity.FindPrimaryKey()?.Properties.Select(p => p.GetColumnName(table)) ?? [];
    Check(actualKey.SequenceEqual(expectedKey), $"{name}: primary key mismatch");
    if (!view) Check(entity.IsTableExcludedFromMigrations(), $"{name}: database-owned table exposed to migrations");
}
Check(model.GetEntityTypes().Sum(e => e.GetForeignKeys().Count()) == data.GetProperty("foreignKeys").EnumerateArray()
    .Select(f => (Int(f, "parent_object_id"), Text(f, "name"))).Distinct().Count(), "Foreign key count mismatch");
var operations = db.GetService<IMigrationsModelDiffer>().GetDifferences(null, model.GetRelationalModel());
Check(operations.Count == 0, "Database-owned context must not generate DDL even without a migration baseline.");
IDomainEventEntity eventRecord = new SqlTblProduct();
eventRecord.AddDomainEvents([new TestEvent(), new TestEvent()]);
Check(eventRecord.DomainEvents.Count == 2, "Neo domain event collection does not record range additions.");
eventRecord.ClearDomainEvents();
Check(eventRecord.DomainEvents.Count == 0, "Neo domain events do not clear.");
Console.WriteLine($"PASS {count} schema assertions; {objects.Length} objects; {columns.Length} columns; migration operations={operations.Count}. No database writes or provider calls.");
IntegrationModelChecks.Run(data, model);
if (args.Length == 2 && args[0] is "--live" or "--live-local")
    await LiveReadChecks.Run(args[1], args[0] == "--live-local");
if (args.Length == 2 && args[0] == "--simulation-sql-local") await SimulationSqlChecks.Run(args[1]);

if (args.Length == 2 && args[0] == "--outbox-sql-local") await OutboxSqlChecks.Run(args[1]);
if (args.Length == 2 && args[0] == "--overview-sql-local") await AdminOverviewSqlChecks.Run(args[1]);

sealed class TestEvent : BaseEvent;

