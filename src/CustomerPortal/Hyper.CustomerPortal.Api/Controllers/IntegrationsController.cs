using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "integrations")]
[ApiController]
[Authorize]
public sealed class IntegrationsController(
    HyperIntegrationContext db,
    IIntegrationSynchronizationService sync,
    IIntegrationOutbox outbox,
    Microsoft.Extensions.Options.IOptions<Hyper.Infrastructure.Features.Integrations.IntegrationInventoryCaptureOptions> inventorySource,
    IIntegrationWebhookVerifier webhookVerifier,
    IIntegrationActor actor,
    IIntegrationShopAccess shopAccess) : ControllerBase
{
    [HttpGet("providers")]
    public IActionResult Providers() => Ok(Enum.GetValues<IntegrationProvider>()
        .Select(x => new { Value = (byte)x, Name = x.ToString() }));

    [HttpGet("connections")]
    public async Task<IActionResult> Connections(CancellationToken ct)
    {
        if (actor.Identity is not { } owner) return Forbid();
        var shops = await shopAccess.GetOwnedShopsAsync(owner, ct);
        return Ok(await db.ExternalIntegrationConnections.AsNoTracking()
            .Where(IntegrationConnectionScope.For(shops))
            .Select(x => new { x.Id, x.ShopId, x.TenantId, x.Provider, x.DisplayName,
                x.AccountIdentifier, x.CredentialType, x.IsEnabled, x.LastSyncAtUtc, x.LastError })
            .ToListAsync(ct));
    }

    [HttpPost("connections")]
    [RequestSizeLimit(131072)]
    public async Task<IActionResult> Register([FromBody] RegisterConnectionRequest request, CancellationToken ct)
    {
        if (actor.Identity is not { } owner) return Forbid();
        var shops = await shopAccess.GetOwnedShopsAsync(owner, ct);
        var shop = shops.SingleOrDefault(x => x.ShopId == request.ShopId);
        if (shop is null) return NotFound();
        // The explicit account grant and shop metadata supplies tenant scope. Request data cannot grant access.
        if (request.TenantId is not null && !string.Equals(request.TenantId, shop.TenantId, StringComparison.Ordinal))
            return BadRequest("Tenant does not match the selected shop.");
        if (!Enum.IsDefined(request.Provider) || !Enum.IsDefined(request.CredentialType))
            return BadRequest("Unsupported provider or credential type.");
        if (string.IsNullOrWhiteSpace(request.AccountIdentifier) || request.AccountIdentifier.Length > 200
            || string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200
            || string.IsNullOrWhiteSpace(request.CredentialsJson) || request.CredentialsJson.Length > 65536)
            return BadRequest("Account, display name and credentials are required and must fit their size limits.");
        try
        {
            using var credentials = JsonDocument.Parse(request.CredentialsJson);
            if (credentials.RootElement.ValueKind != JsonValueKind.Object)
                return BadRequest("Credentials must be a JSON object.");
        }
        catch (JsonException) { return BadRequest("Credentials must be a JSON object."); }

        if (await db.ExternalIntegrationConnections.AnyAsync(x => x.ShopId == shop.ShopId
            && x.Provider == request.Provider && x.AccountIdentifier == request.AccountIdentifier, ct))
            return Conflict("Connection already exists.");
        var connection = new ExternalIntegrationConnection
        {
            ShopId = shop.ShopId, TenantId = shop.TenantId, Provider = request.Provider,
            DisplayName = request.DisplayName, AccountIdentifier = request.AccountIdentifier,
            CredentialType = request.CredentialType, CredentialsJson = request.CredentialsJson
        };
        db.ExternalIntegrationConnections.Add(connection);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException error) when (error.InnerException is Microsoft.Data.SqlClient.SqlException sql
            && sql.Number is 2601 or 2627)
        {
            db.ChangeTracker.Clear();
            return Conflict("Connection already exists.");
        }
        return CreatedAtAction(nameof(Connections), new { }, new
        {
            connection.Id, connection.ShopId, connection.TenantId, connection.Provider,
            connection.DisplayName, connection.AccountIdentifier, connection.IsEnabled
        });
    }

    [HttpPost("connections/{id:long}/disable")]
    public async Task<IActionResult> Disable(long id, CancellationToken ct)
    {
        var connection = await FindOwnedConnection(id, ct);
        if (connection is null) return NotFound();
        connection.IsEnabled = false;
        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpGet("connections/{id:long}/mappings")]
    public async Task<IActionResult> Mappings(long id, CancellationToken ct)
    {
        var connection = await FindOwnedConnection(id, ct);
        if (connection is null) return NotFound();
        return Ok(await db.ExternalProductMappings.AsNoTracking()
            .Where(x => x.ConnectionId == id && x.ShopId == connection.ShopId).ToListAsync(ct));
    }

    [HttpGet("connections/{id:long}/runs")]
    public async Task<IActionResult> Runs(long id, CancellationToken ct)
    {
        if (await FindOwnedConnection(id, ct) is null) return NotFound();
        return Ok(await db.IntegrationSyncRuns.AsNoTracking().Where(x => x.ConnectionId == id)
            .OrderByDescending(x => x.StartedAtUtc).Take(100).ToListAsync(ct));
    }

    [HttpPost("connections/{id:long}/inventory")]
    public async Task<IActionResult> QueueInventory(long id, [FromBody] InventoryRequest request, CancellationToken ct)
    {
        if (await FindOwnedConnection(id, ct) is null) return NotFound();
        if (inventorySource.Value.AccountingStockSourceVerified)
            return Conflict("Inventory is managed by SQL capture; submit the accounting change to Hyper and let capture publish it.");
        try
        {
            var messageId = await outbox.EnqueueInventoryAsync(id, request.MappingId, request.SourceVersion, request.Quantity, ct);
            return Accepted(new { MessageId = messageId });
        }
        catch (ArgumentException) { return BadRequest("A positive source version and nonnegative absolute quantity are required."); }
        catch (NotSupportedException) { return Problem(statusCode: 501, title: "Provider strategy is not available."); }
        catch (InvalidOperationException) { return Conflict("Connection/mapping is unavailable or the source version conflicts with an existing update."); }
    }

    [HttpGet("connections/{id:long}/outbox")]
    public async Task<IActionResult> Outbox(long id, [FromQuery] byte? status, CancellationToken ct)
    {
        if (await FindOwnedConnection(id, ct) is null) return NotFound();
        var query = db.IntegrationOutbox.AsNoTracking().Where(x => x.ConnectionId == id);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        return Ok(await query.OrderByDescending(x => x.Id).Take(200)
            .Select(x => new { x.Id, x.MappingId, x.SourceVersion, x.Operation, x.Status, x.Attempts,
                x.CreatedAtUtc, x.NextAttemptAtUtc, x.CompletedAtUtc, x.LastError }).ToListAsync(ct));
    }

    public sealed record InventoryRequest(long MappingId, long SourceVersion, decimal Quantity);

    [HttpPost("connections/{id:long}/outbox/{messageId:long}/retry")]
    public async Task<IActionResult> RetryOutbox(long id, long messageId, CancellationToken ct)
    {
        if (await FindOwnedConnection(id, ct) is null) return NotFound();
        try
        {
            return await outbox.RetryAsync(id, messageId, ct) ? Accepted(new { MessageId = messageId })
                : Conflict("Only the latest failed version of an active mapping can be retried.");
        }
        catch (InvalidOperationException) { return Conflict("Connection is not ready for retry."); }
    }

    [HttpGet("connections/{id:long}/webhooks")]
    public async Task<IActionResult> Webhooks(long id, [FromQuery] byte? status, CancellationToken ct)
    {
        if (await FindOwnedConnection(id, ct) is null) return NotFound();
        var query = db.IntegrationWebhookInbox.AsNoTracking().Where(x => x.ConnectionId == id);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        return Ok(await query.OrderByDescending(x => x.ReceivedAtUtc).Take(200)
            .Select(x => new { x.Id, x.ExternalEventId, x.EventType, x.ReceivedAtUtc,
                x.ProcessedAtUtc, x.Status, x.Error }).ToListAsync(ct));
    }

    [HttpPost("webhooks/{webhookId:long}/replay")]
    public async Task<IActionResult> Replay(long webhookId, CancellationToken ct)
    {
        if (actor.Identity is not { } owner) return NotFound();
        var shops = await shopAccess.GetOwnedShopsAsync(owner, ct);
        if (shops.Count == 0) return NotFound();
        var allowedConnections = db.ExternalIntegrationConnections.Where(IntegrationConnectionScope.For(shops));
        var inbox = await (from message in db.IntegrationWebhookInbox
                           join connection in allowedConnections on message.ConnectionId equals connection.Id
                           where message.Id == webhookId
                           select message).SingleOrDefaultAsync(ct);
        if (inbox is null) return NotFound();
        if (inbox.Status != 2) return Conflict("Only failed webhook events can be replayed.");
        inbox.Status = 0;
        inbox.Error = null;
        inbox.ProcessedAtUtc = null;
        await db.SaveChangesAsync(ct);
        return Accepted(new { WebhookId = inbox.Id, Queued = true });
    }

    [HttpPost("connections/{id:long}/sync")]
    public async Task<IActionResult> Sync(long id, CancellationToken ct)
    {
        if (await FindOwnedConnection(id, ct) is null) return NotFound();
        try { return Ok(new { RunId = await sync.SynchronizeAsync(id, ct) }); }
        catch (NotSupportedException)
        {
            return Problem(statusCode: StatusCodes.Status501NotImplemented,
                title: "The selected platform and credential strategy is not available.");
        }
    }

    private async Task<ExternalIntegrationConnection?> FindOwnedConnection(long id, CancellationToken ct)
    {
        if (actor.Identity is not { } owner) return null;
        var shops = await shopAccess.GetOwnedShopsAsync(owner, ct);
        if (shops.Count == 0) return null;
        return await db.ExternalIntegrationConnections.Where(IntegrationConnectionScope.For(shops))
            .SingleOrDefaultAsync(x => x.Id == id, ct);
    }

 [AllowAnonymous]
 [HttpPost("webhooks/{id:long}")]
 [RequestSizeLimit(1048576)]
 public async Task<IActionResult> Webhook(long id, CancellationToken ct)
 {
     var connection = await db.ExternalIntegrationConnections.AsNoTracking()
         .SingleOrDefaultAsync(x => x.Id == id && x.IsEnabled, ct);
     if (connection is null) return NotFound();
     using var buffer = new MemoryStream();
     var chunk = new byte[8192];
     int read;
     while ((read = await Request.Body.ReadAsync(chunk.AsMemory(), ct)) != 0)
     {
         if (buffer.Length + read > 1048576) return StatusCode(StatusCodes.Status413PayloadTooLarge);
         buffer.Write(chunk, 0, read);
     }
     var raw = buffer.ToArray();
     var eventId = Request.Headers["X-Event-Id"].ToString();
     var eventType = Request.Headers["X-Event-Type"].ToString();
     var verification = webhookVerifier.Verify(connection, new IntegrationWebhookRequest(raw,
         eventId, eventType, Request.Headers["X-Timestamp"].ToString(),
         Request.Headers["X-Signature"].ToString()), DateTimeOffset.UtcNow);
     if (verification == WebhookValidationResult.Unsupported)
         return StatusCode(StatusCodes.Status501NotImplemented);
     if (verification != WebhookValidationResult.Valid) return Unauthorized();
     try
     {
         using var json = JsonDocument.Parse(raw);
         if (json.RootElement.ValueKind != JsonValueKind.Object) return BadRequest("Expected a JSON object.");
     }
     catch (JsonException) { return BadRequest("Invalid JSON."); }
     if (await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x =>
         x.ConnectionId == id && x.ExternalEventId == eventId, ct)) return Ok(new { duplicate = true });
     var receivedAt = DateTime.UtcNow;
     db.IntegrationEventAudits.Add(new IntegrationEventAudit
     {
         ConnectionId = id, Direction = "Inbound", EventType = eventType,
         ExternalEventId = eventId, PayloadHash = Convert.ToHexString(SHA256.HashData(raw)),
         SignatureValid = true, CorrelationId = HttpContext.TraceIdentifier, ReceivedAtUtc = receivedAt
     });
     db.IntegrationWebhookInbox.Add(new IntegrationWebhookInbox
     {
         ConnectionId = id, ExternalEventId = eventId, EventType = eventType,
         PayloadJson = Encoding.UTF8.GetString(raw), ReceivedAtUtc = receivedAt
     });
     try { await db.SaveChangesAsync(ct); }
     catch (DbUpdateException error) when (error.InnerException is Microsoft.Data.SqlClient.SqlException sql
         && sql.Number is 2601 or 2627)
     {
         // The database unique key, not a pre-read, arbitrates simultaneous duplicate deliveries.
         db.ChangeTracker.Clear();
         if (await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x =>
             x.ConnectionId == id && x.ExternalEventId == eventId, ct)) return Ok(new { duplicate = true });
         throw;
     }
     return Accepted();
 }

    public sealed record RegisterConnectionRequest(int ShopId, string? TenantId, IntegrationProvider Provider,
        string DisplayName, string AccountIdentifier, IntegrationCredentialType CredentialType, string CredentialsJson);
}
