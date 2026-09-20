using System.Data;
using Hyper.Domain.Entities.Integrations;
using Hyper.SDK.Auth;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

// Uses the existing Neo command context; no additional OAuth DbContext.
public sealed class BasalamOAuthStore(HyperContextCommand db, BasalamOAuthService oauth)
{
    public async Task<TokenInfo?> GetTokenAsync(ExternalIntegrationConnection connection, CancellationToken ct)
    {
        var row = await db.ExternalOAuthTokens.SingleOrDefaultAsync(x =>
            x.ConnectionId == connection.Id && x.ShopId == connection.ShopId &&
            x.TenantId == connection.TenantId && x.Provider == IntegrationProvider.Basalam && x.IsActive, ct);
        if (row is null || string.IsNullOrWhiteSpace(row.AccessToken)) return null;
        if (row.ExpiresAtUtc <= DateTime.UtcNow.AddMinutes(1)) return null;
        var access = oauth.DecryptToken(row.AccessToken);
        var refresh = string.IsNullOrWhiteSpace(row.RefreshToken) ? null : oauth.DecryptToken(row.RefreshToken);
        return new TokenInfo { AccessToken = access, TokenType = row.TokenType, RefreshToken = refresh, ExpiresAt = row.ExpiresAtUtc ?? DateTime.UtcNow.AddMinutes(10) };
    }
    public async Task<bool> TryClaimAsync(BasalamAuthorizationState state, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        return await db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE r SET Status = 1
            FROM dbo.IntegrationTokenRequests r
            INNER JOIN dbo.IntegrationAdminSimulations s ON s.Id = r.SimulationId
            WHERE r.Id = {state.RequestId} AND r.SimulationId = {state.SimulationId}
              AND r.Status = 0 AND r.Provider = 1 AND r.CredentialType = 1
              AND s.AdminUserId = {state.AdminId} AND s.EndedAtUtc IS NULL AND s.ExpiresAtUtc > {now}", ct) == 1;
    }

    public async Task SetOutcomeAsync(Guid requestId, byte status, CancellationToken ct) =>
        await db.Set<IntegrationTokenRequest>().Where(x => x.Id == requestId && x.Status == 1)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, status), ct);

    public async Task SaveAsync(BasalamAuthorizationState state, IntegrationAdminSimulation selected,
        BasalamVendor vendor, BasalamTokenResponse response, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        // Keep the simulation row locked until commit; Clear/Select cannot reassign the callback.
        var now = DateTime.UtcNow;
        var valid = await db.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE dbo.IntegrationAdminSimulations SET ShopName = ShopName
            WHERE Id = {state.SimulationId} AND AdminUserId = {state.AdminId}
              AND ShopId = {selected.ShopId} AND TenantId = {selected.TenantId}
              AND EndedAtUtc IS NULL AND ExpiresAtUtc > {now}", ct);
        if (valid != 1) throw new InvalidOperationException("زمینه مغازه تغییر کرده یا منقضی شده است.");
        var request = await db.Set<IntegrationTokenRequest>().SingleAsync(x => x.Id == state.RequestId, ct);
        if (request.Status != 1) throw new InvalidOperationException("این درخواست قبلاً پردازش شده است.");
        var connection = await db.ExternalIntegrationConnections.SingleOrDefaultAsync(x =>
            x.ShopId == selected.ShopId && x.Provider == IntegrationProvider.Basalam && x.AccountIdentifier == vendor.Id, ct);
        if (connection is not null && connection.TenantId != selected.TenantId)
            throw new InvalidOperationException("زمینه اتصال با مغازه تطابق ندارد.");
        if (connection is null)
        {
            connection = new ExternalIntegrationConnection
            {
                ShopId = selected.ShopId, TenantId = selected.TenantId, Provider = IntegrationProvider.Basalam,
                DisplayName = vendor.Title, AccountIdentifier = vendor.Id, CredentialType = IntegrationCredentialType.OAuth2,
                CredentialsJson = "{}", IsEnabled = false
            };
            db.ExternalIntegrationConnections.Add(connection);
            await db.SaveChangesAsync(ct);
        }
        var previous = await db.ExternalOAuthTokens.SingleOrDefaultAsync(x =>
            x.ShopId == selected.ShopId && x.Provider == IntegrationProvider.Basalam, ct);
        if (previous is not null && (previous.TenantId != selected.TenantId || previous.ConnectionId != connection.Id))
            throw new InvalidOperationException("برای این مغازه توکن غرفه دیگری ثبت شده است؛ ابتدا اتصال قبلی را تعیین تکلیف کنید.");
        var token = oauth.CreateTokenEntity(connection.Id, selected.ShopId, selected.TenantId, IntegrationProvider.Basalam, response);
        if (previous is null) db.ExternalOAuthTokens.Add(token);
        else
        {
            token.Id = previous.Id;
            token.UpdatedAtUtc = DateTime.UtcNow;
            db.Entry(previous).CurrentValues.SetValues(token);
        }
        // Token acquisition alone must not activate accounting/inventory jobs.
        connection.IsEnabled = false;
        request.Status = 2;
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public Task<BasalamTokenStatus?> GetStatusAsync(int shopId, string tenantId, CancellationToken ct) =>
        (from t in db.ExternalOAuthTokens.AsNoTracking()
         join c in db.ExternalIntegrationConnections.AsNoTracking() on t.ConnectionId equals c.Id
         where t.ShopId == shopId && t.TenantId == tenantId && t.Provider == IntegrationProvider.Basalam
            && c.ShopId == shopId && c.TenantId == tenantId
         select new BasalamTokenStatus(c.AccountIdentifier, c.DisplayName, t.IssuedAtUtc, t.ExpiresAtUtc, t.IsActive))
        .SingleOrDefaultAsync(ct);
}
public sealed record BasalamTokenStatus(string VendorId, string VendorName, DateTime IssuedAtUtc, DateTime? ExpiresAtUtc, bool IsActive);
