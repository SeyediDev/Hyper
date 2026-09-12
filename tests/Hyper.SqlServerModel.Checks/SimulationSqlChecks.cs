using System.Text.Json;
using Hyper.Domain.Entities.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

internal static class SimulationSqlChecks
{
    public static async Task Run(string settingsPath)
    {
        using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(settingsPath),
            new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
        var builder = new SqlConnectionStringBuilder(settings.RootElement.GetProperty("ConnectionStrings").GetProperty("Domain").GetString());
        if (builder.InitialCatalog != "Hyperyek" || builder.DataSource is not ("." or "(local)" or "localhost"))
            throw new Exception("This read/rollback fixture requires local Hyperyek.");
        builder.DataSource = "lpc:.";
        builder.Encrypt = SqlConnectionEncryptOption.Optional;
        await using var accounting = new HyperSqlServerContext(new DbContextOptionsBuilder<HyperSqlServerContext>().UseSqlServer(builder.ConnectionString).Options);
        await using var integrations = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(builder.ConnectionString).Options);
        var service = new AdminMerchantSimulationService(accounting, integrations);
        var shops = await service.SearchShopsAsync(null, default);
        if (shops.Count == 0) throw new Exception("A real shop is required for this fixture.");
        var admin = "test-admin-" + Guid.NewGuid().ToString("N");
        Guid selectionId;
        await using (var transaction = await integrations.Database.BeginTransactionAsync())
        {
            var selection = await service.SelectAsync(admin, shops[0].ShopId, default) ?? throw new Exception("Selection failed.");
            selectionId = selection.Id;
            await CheckDashboard(integrations, selection);
            if (await service.GetAsync("other-admin", selection.Id, default) is not null) throw new Exception("Cross-admin access.");
            if (await service.RequestTokenAsync(admin, selection.Id, -1, IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2, default) is not null)
                throw new Exception("Forged shop accepted.");
            var request = await service.RequestTokenAsync(admin, selection.Id, selection.ShopId, IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2, default)
                ?? throw new Exception("Token simulation not recorded.");
            if (request.Status != 0 || request.SimulationId != selection.Id) throw new Exception("Simulation falsely reports issued token.");
            await service.EndAsync(admin, selection.Id, default);
            if (await service.RequestTokenAsync(admin, selection.Id, selection.ShopId, IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2, default) is not null)
                throw new Exception("Ended selection accepted.");
            await transaction.RollbackAsync();
        }
        integrations.ChangeTracker.Clear();
        if (await integrations.IntegrationAdminSimulations.AnyAsync(x => x.Id == selectionId)
            || await integrations.IntegrationTokenRequests.AnyAsync(x => x.SimulationId == selectionId))
            throw new Exception("Test transaction did not roll back.");
        Console.WriteLine("PASS SQL simulation: real shop read, admin isolation, forged-shop denial, prepared request, ended-context denial, rollback verified. Only new integration tables were written inside the rolled-back transaction.");
    }

    private static async Task CheckDashboard(HyperIntegrationContext db, IntegrationAdminSimulation selection)
    {
        var query = new IntegrationDashboardQuery(db);
        var baseline = await query.GetAsync(selection.ShopId, selection.TenantId, default);
        var connection = new ExternalIntegrationConnection
        {
            ShopId = selection.ShopId, TenantId = selection.TenantId, Provider = IntegrationProvider.Custom,
            DisplayName = "dashboard-test", AccountIdentifier = Guid.NewGuid().ToString("N"),
            CredentialType = IntegrationCredentialType.ApiKey, CredentialsJson = "{}"
        };
        var foreign = new ExternalIntegrationConnection
        {
            ShopId = selection.ShopId, TenantId = "other-" + Guid.NewGuid().ToString("N")[..12], Provider = IntegrationProvider.Custom,
            DisplayName = "other-scope", AccountIdentifier = Guid.NewGuid().ToString("N"),
            CredentialType = IntegrationCredentialType.ApiKey, CredentialsJson = "{}"
        };
        db.ExternalIntegrationConnections.AddRange(connection, foreign);
        await db.SaveChangesAsync();
        for (var i = 0; i < 2; i++) db.ExternalProductMappings.Add(new()
        {
            ConnectionId = connection.Id, ShopId = selection.ShopId, HyperProductId = 1,
            ExternalProductId = "fixture-" + i, IsActive = true
        });
        for (byte status = 0; status < 4; status++) db.IntegrationSyncRuns.Add(new()
        {
            ConnectionId = connection.Id, Status = status
        });
        db.IntegrationSyncRuns.Add(new() { ConnectionId = foreign.Id, Status = 1 });
        for (var i = 0; i < 4; i++) db.IntegrationWebhookInbox.Add(new()
        {
            ConnectionId = connection.Id, ExternalEventId = "fixture-" + i, EventType = "fixture",
            PayloadJson = "{}", Status = i < 2 ? (byte)0 : (byte)(i - 1)
        });
        await db.SaveChangesAsync();
        var actual = await query.GetAsync(selection.ShopId, selection.TenantId, default);
        if (actual.Connections != baseline.Connections + 1 || actual.MappedProducts != baseline.MappedProducts + 2
            || actual.EnabledConnections != baseline.EnabledConnections + 1
            || actual.WebhookCount(0) != baseline.WebhookCount(0) + 2
            || actual.WebhookCount(2) != baseline.WebhookCount(2) + 1
            || Enumerable.Range(0, 4).Any(s => actual.RunCount((byte)s) != baseline.RunCount((byte)s) + 1)
            || actual.RecentRuns.Any(r => r.ConnectionName == foreign.DisplayName))
            throw new Exception("Dashboard fan-out or cross-tenant leakage.");
        Console.WriteLine("PASS SQL dashboard: independent mapping/run/webhook counts and tenant isolation; fixtures roll back.");
    }
}
