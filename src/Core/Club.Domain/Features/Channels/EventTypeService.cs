namespace Hyper.Domain.Features.Channels;

public interface IEventTypeService
{
    [Telemetry]
    Task<EventType?> GetEventTypeAsync(int tenantId, string key, CancellationToken cancellationToken);
    [Telemetry]
    Task<IEnumerable<EventType>> GetEventTypesAsync(int tenantId, CancellationToken cancellationToken);
}
internal class EventTypeService(
        IQueryRepository<EventType, int> eventTypeQuery
    ) : IEventTypeService
{
    public async Task<EventType?> GetEventTypeAsync(int tenantId, string key, CancellationToken cancellationToken)
    {
        return await eventTypeQuery.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Key == key, cancellationToken);
    }

    public async Task<IEnumerable<EventType>> GetEventTypesAsync(int tenantId, CancellationToken cancellationToken)
    {
        return await eventTypeQuery.GetAllAsync(cancellationToken, x => x.TenantId == tenantId);
    }
}
