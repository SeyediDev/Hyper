using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
namespace Hyper.Infrastructure.Features.Integrations;
public sealed class IntegrationSynchronizationService(HyperContextCommand db,IEnumerable<IExternalIntegrationAdapter> adapters) : IIntegrationSynchronizationService
{
 public async Task<long> SynchronizeAsync(long connectionId,CancellationToken cancellationToken=default)
 {
  var connection=await db.ExternalIntegrationConnections.SingleAsync(x=>x.Id==connectionId && x.IsEnabled,cancellationToken);
  var adapter=adapters.SingleOrDefault(x=>x.Provider==connection.Provider) ?? throw new InvalidOperationException($"No adapter registered for {connection.Provider}");
  var run=new IntegrationSyncRun{ConnectionId=connectionId}; db.IntegrationSyncRuns.Add(run); await db.SaveChangesAsync(cancellationToken);
  try { var items=await adapter.ReadCatalogAsync(connection,cancellationToken); run.ItemsRead=items.Count; foreach(var item in items){var mapping=await db.ExternalProductMappings.SingleOrDefaultAsync(x=>x.ConnectionId==connectionId && x.ExternalProductId==item.ExternalProductId && x.ExternalVariantId==item.VariantId,cancellationToken); if(mapping is null){mapping=new ExternalProductMapping{ConnectionId=connectionId,ShopId=connection.ShopId,ExternalProductId=item.ExternalProductId,ExternalVariantId=item.VariantId,ExternalSku=item.Sku};db.ExternalProductMappings.Add(mapping);} mapping.LastExternalPrice=item.Price; mapping.LastExternalInventory=item.Inventory; mapping.LastSyncAtUtc=DateTime.UtcNow; run.ItemsWritten++;} await adapter.PublishInventoryAsync(connection,await db.ExternalProductMappings.Where(x=>x.ConnectionId==connectionId && x.IsActive).ToListAsync(cancellationToken),cancellationToken); run.Status=1; connection.LastSyncAtUtc=DateTime.UtcNow; } catch(Exception ex){run.Status=2;run.Error=ex.ToString();connection.LastError=ex.Message;throw;} finally {run.FinishedAtUtc=DateTime.UtcNow;await db.SaveChangesAsync(cancellationToken);} return run.Id;
 }
}
