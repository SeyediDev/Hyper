namespace Hyper.Domain.Entities.Database;

/// <summary>
/// Neo-compatible base for database-owned records. Key shape belongs to each table;
/// composite keys and keyless views must not acquire an artificial Id column.
/// </summary>
[DataProvider("Domain")]
public abstract class SqlServerEntity : IEntity, IDomainEventEntity
{
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void AddDomainEvents(IEnumerable<BaseEvent> domainEvents) => _domainEvents.AddRange(domainEvents);
    public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>Typed Neo identity only for tables whose actual single primary key maps to Id.</summary>
public abstract class SqlServerEntity<TKey> : SqlServerEntity, IEntity<TKey>
{
    public TKey Id { get; set; } = default!;
}
