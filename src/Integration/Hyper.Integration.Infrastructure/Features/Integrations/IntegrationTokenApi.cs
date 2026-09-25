using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Microsoft.EntityFrameworkCore;
using DomainProvider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;
using DomainCredential = Hyper.Integration.Domain.Entities.Integrations.IntegrationCredentialType;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationTokenApi(
    HyperIntegrationContext db,
    Hyper.Integration.Domain.Features.Integrations.IAdminMerchantSimulationService simulations) : IIntegrationTokenApi
{
    public async Task<IntegrationTokenRequestStatus?> RequestAsync(string adminId,
        IntegrationTokenRequestCommand command, CancellationToken ct)
    {
        var request = await simulations.RequestTokenAsync(adminId, command.SimulationId, command.ShopId,
            (DomainProvider)(byte)command.Provider, (DomainCredential)command.CredentialType, ct);
        return request is null ? null : new(request.Id, request.Status, request.RequestedAtUtc);
    }

    public async Task<IReadOnlyList<IntegrationTokenStatus>> ListAsync(int shopId, string tenantId, CancellationToken ct) =>
        await (from token in db.ExternalOAuthTokens.AsNoTracking()
               join connection in db.ExternalIntegrationConnections.AsNoTracking()
                   on token.ConnectionId equals connection.Id
               where token.ShopId == shopId && token.TenantId == tenantId
                   && connection.ShopId == shopId && connection.TenantId == tenantId
               select new IntegrationTokenStatus(connection.Id, (Hyper.Integration.Contracts.IntegrationProvider)(byte)token.Provider,
                   token.IsActive, connection.IsEnabled, token.ExpiresAtUtc)).ToListAsync(ct);

    public async Task<bool> RevokeAsync(int shopId, string tenantId, long connectionId, CancellationToken ct)
    {
        var changed = await db.ExternalOAuthTokens.Where(x => x.ConnectionId == connectionId
                && x.ShopId == shopId && x.TenantId == tenantId)
            .ExecuteUpdateAsync(x => x.SetProperty(t => t.IsActive, false), ct);
        if (changed == 0) return false;
        await db.ExternalIntegrationConnections.Where(x => x.Id == connectionId
                && x.ShopId == shopId && x.TenantId == tenantId)
            .ExecuteUpdateAsync(x => x.SetProperty(c => c.IsEnabled, false), ct);
        return true;
    }
}
