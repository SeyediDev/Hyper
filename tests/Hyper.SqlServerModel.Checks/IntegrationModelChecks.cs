using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

internal static class IntegrationModelChecks
{
    public static void Run(JsonElement schema, IModel baseline)
    {
        using var db = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>()
            .UseSqlServer("Server=.;Database=Hyperyek;Integrated Security=True;TrustServerCertificate=True").Options);
        var model = db.GetService<IDesignTimeModel>().Model;
        if (model.GetEntityTypes().Count() != 11) throw new Exception("Integration context includes unrelated entities.");
        var checkedColumns = 0;
        foreach (var entity in model.GetEntityTypes())
        {
            var table = StoreObjectIdentifier.Table(entity.GetTableName()!, entity.GetSchema());
            var expected = baseline.GetEntityTypes().Single(e => e.GetTableName() == table.Name && e.GetSchema() == table.Schema);
            if (entity.GetProperties().Count() != expected.GetProperties().Count()) throw new Exception($"{table.Name}: property count differs");
            foreach (var property in entity.GetProperties())
            {
                var column = property.GetColumnName(table);
                var other = expected.GetProperties().Single(p => p.GetColumnName(table) == column);
                if (property.GetColumnType() != other.GetColumnType() || property.IsNullable != other.IsNullable
                    || property.GetDefaultValueSql() != other.GetDefaultValueSql()
                    || property.GetValueGenerationStrategy() != other.GetValueGenerationStrategy())
                    throw new Exception($"{table.Name}.{column}: integration mapping differs from SQL baseline");
                checkedColumns++;
            }
            var actualConstraints = entity.GetForeignKeys().Select(f => f.GetConstraintName()).Order();
            var expectedConstraints = expected.GetForeignKeys().Select(f => f.GetConstraintName()).Order();
            if (!actualConstraints.SequenceEqual(expectedConstraints)) throw new Exception($"{table.Name}: FK mismatch");
        }
        if (db.GetService<IMigrationsModelDiffer>().GetDifferences(null, model.GetRelationalModel()).Count != 0)
            throw new Exception("Existing integration tables generate baseline DDL.");
        Console.WriteLine($"PASS integration context: 11 tables, {checkedColumns} columns match audited SQL baseline; controller and synchronization production sources compiled.");
    }
}
