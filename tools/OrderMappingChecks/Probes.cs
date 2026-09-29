using System.Collections.Concurrent;

// These probes assert orchestration only. They never invoke the accounting API,
// provider network, native invoice writer or a real inventory reservation store.
internal sealed class CommandProbe : IIntegrationBusinessCommandPort
{
    internal int Calls { get; private set; }
    internal List<IntegrationVendorOrderCommand> Received { get; } = [];
    internal Func<IntegrationVendorOrderCommand, CancellationToken, Task<BusinessCommandResult>> Behavior { get; set; } =
        (_, _) => Task.FromResult(new BusinessCommandResult(BusinessCommandStatus.Applied, "101"));

    public Task<BusinessCommandResult> ApplyVendorOrderAsync(IntegrationVendorOrderCommand command, CancellationToken ct)
    {
        Calls++;
        Received.Add(command);
        return Behavior(command, ct);
    }

    // Deliberately outside the dispatcher's routine provider error vocabulary.
    public Task<BusinessCommandResult> ApplyCounterpartyAsync(IntegrationCounterpartyCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyCustomerOrderAsync(IntegrationCustomerOrderCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> CancelOrderAsync(IntegrationOrderCancellationCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyParcelStatusAsync(IntegrationParcelStatusCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyExternalProductChangedAsync(IntegrationExternalProductChangedCommand command, CancellationToken ct) => throw new UnexpectedDependency();
}

internal sealed class ReservationProbe : IIntegrationInventoryReservation
{
    internal int ReserveCalls { get; private set; }
    internal int CommitCalls { get; private set; }
    internal int ReleaseCalls { get; private set; }
    internal HashSet<string> Held { get; } = [];
    internal HashSet<string> Committed { get; } = [];
    internal ConcurrentQueue<string> Steps { get; } = new();

    public Task ReserveAsync(OwnedIntegrationShop shop, string key,
        IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct)
    {
        ReserveCalls++;
        Steps.Enqueue("reserve");
        if (!Committed.Contains(key)) Held.Add(key);
        return Task.CompletedTask;
    }

    public Task CommitAsync(OwnedIntegrationShop shop, string key, CancellationToken ct)
    {
        CommitCalls++;
        Steps.Enqueue("commit");
        Held.Remove(key);
        Committed.Add(key);
        return Task.CompletedTask;
    }

    public Task ReleaseAsync(OwnedIntegrationShop shop, string key, CancellationToken ct)
    {
        ReleaseCalls++;
        throw new UnexpectedDependency();
    }
}

internal sealed class ForbiddenEngagement : IIntegrationEngagementPort
{
    public Task<EngagementCommandResult> ApplySubscriptionAsync(IntegrationSubscriptionCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<EngagementCommandResult> ApplyReviewAsync(IntegrationReviewCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<EngagementCommandResult> ApplyChatMessageAsync(IntegrationChatMessageCommand command, CancellationToken ct) => throw new UnexpectedDependency();
}
