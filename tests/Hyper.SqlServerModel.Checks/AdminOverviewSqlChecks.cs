using System.Text.Json;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

internal static class AdminOverviewSqlChecks
{
    public static async Task Run(string settingsPath)
    {
        using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(settingsPath),
            new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
        var sql = new SqlConnectionStringBuilder(settings.RootElement.GetProperty("ConnectionStrings").GetProperty("Domain").GetString());
        if (sql.InitialCatalog != "Hyperyek" || sql.DataSource is not ("." or "localhost" or "(local)"))
            throw new Exception("Overview checks require local Hyperyek.");
        sql.DataSource = "lpc:.";
        sql.Encrypt = SqlConnectionEncryptOption.Optional;
        await using var accounting = new HyperSqlServerContext(new DbContextOptionsBuilder<HyperSqlServerContext>().UseSqlServer(sql.ConnectionString).Options);
        await using var integrations = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(sql.ConnectionString).Options);
        var query = new AdminOverviewQuery(accounting, integrations);
        var service = new AdminMerchantSimulationService(accounting, integrations);
        var shop = (await service.SearchShopsAsync(null, default)).First();
        var rawShop = await accounting.TblShops.SingleAsync(x => x.Shopid == shop.ShopId);
        var tenant = IntegrationConnectionScope.CanonicalTenant(shop.ShopId, rawShop.TenantId);
        foreach (var days in new[] { 7, 30 })
        {
            var overview = await query.GetAsync(days, null, null, default);
            if (overview.Shops != await accounting.TblShops.CountAsync()
                || overview.Products != await accounting.TblProducts.CountAsync()
                || overview.People != await accounting.TblPersons.CountAsync()) throw new Exception("Global accounting totals mismatch.");
            if (overview.Trend.Count != days || overview.Trend.Sum(x => x.Invoices) != overview.Invoices
                || overview.Trend.Sum(x => x.SuccessfulRuns) != overview.SuccessfulRuns
                || overview.Trend.Sum(x => x.FailedRuns) != overview.FailedRuns) throw new Exception("Trend totals mismatch.");
        }
        var baseline = await query.GetAsync(7, shop.ShopId, tenant, default);
        if (baseline.Shops != 1) throw new Exception("Selected shop scope mismatch.");
        long id;
        await using (var tx = await integrations.Database.BeginTransactionAsync())
        {
            var connection = new ExternalIntegrationConnection { ShopId = shop.ShopId, TenantId = tenant,
                Provider = IntegrationProvider.Custom, DisplayName = "overview-fixture", AccountIdentifier = Guid.NewGuid().ToString("N"),
                CredentialType = IntegrationCredentialType.ApiKey, CredentialsJson = "{}" };
            var foreign = new ExternalIntegrationConnection { ShopId = shop.ShopId, TenantId = "other-" + Guid.NewGuid().ToString("N")[..12],
                Provider = IntegrationProvider.Custom, DisplayName = "foreign-fixture", AccountIdentifier = Guid.NewGuid().ToString("N"),
                CredentialType = IntegrationCredentialType.ApiKey, CredentialsJson = "{}" };
            integrations.ExternalIntegrationConnections.AddRange(connection, foreign);
            await integrations.SaveChangesAsync();
            id = connection.Id;
            for (var i = 0; i < 3; i++) integrations.IntegrationSyncRuns.Add(new() { ConnectionId = connection.Id, Status = 1 });
            for (var i = 0; i < 4; i++) integrations.IntegrationSyncRuns.Add(new() { ConnectionId = foreign.Id, Status = 2 });
            integrations.IntegrationSyncRuns.Add(new() { ConnectionId = connection.Id, Status = 1, StartedAtUtc = DateTime.UtcNow.Date.AddDays(-40) });
            await integrations.SaveChangesAsync();
            var scoped = await query.GetAsync(7, shop.ShopId, tenant, default);
            if (scoped.Connections != baseline.Connections + 1 || scoped.SuccessfulRuns != baseline.SuccessfulRuns + 3
                || scoped.FailedRuns != baseline.FailedRuns || scoped.Products != baseline.Products
                || scoped.People != baseline.People || scoped.Invoices != baseline.Invoices)
                throw new Exception("Tenant leakage, period filtering or fan-out error.");
            if (scoped.RecentRuns.Any(x => x.ConnectionName == "foreign-fixture")) throw new Exception("Foreign recent run exposed.");
            await tx.RollbackAsync();
        }
        integrations.ChangeTracker.Clear();
        if (await integrations.ExternalIntegrationConnections.AnyAsync(x => x.Id == id)) throw new Exception("Overview fixture rollback failed.");
        var empty = await query.GetAsync(7, shop.ShopId, "absent-" + Guid.NewGuid().ToString("N"), default);
        if (empty.Shops != 0 || empty.Products != 0 || empty.People != 0 || empty.Invoices != 0 || empty.Connections != 0)
            throw new Exception("Wrong tenant sees real accounting data.");
        Console.WriteLine("PASS overview SQL: 7/30-day totals and trends, selected shop, tenant isolation, independent counts, old-run exclusion and rollback. No legacy writes or provider calls.");
    }
}
