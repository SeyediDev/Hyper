namespace Hyper.Domain.Features.Integrations;

public interface IIntegrationOutbox
{
    Task<long> EnqueueInventoryAsync(long connectionId, long mappingId, long sourceVersion, decimal quantity, CancellationToken ct);
    Task<bool> ProcessNextAsync(CancellationToken ct);
    Task<bool> RetryAsync(long connectionId, long messageId, CancellationToken ct);
}

public static class IntegrationRetryPolicy
{
    // Initial delivery plus the five documented retry intervals.
    public const int MaxAttempts = 6;
    private static readonly int[] RetryMinutes = [1, 5, 15, 60, 1440];
    public static TimeSpan Delay(int attempts, TimeSpan? providerDelay = null)
    {
        var seconds = 60d * RetryMinutes[Math.Clamp(attempts - 1, 0, RetryMinutes.Length - 1)];
        // Honor a provider's Retry-After; do not retry earlier than it requested.
        if (providerDelay is { } delay) seconds = Math.Max(seconds, delay.TotalSeconds);
        return TimeSpan.FromSeconds(Math.Max(1, seconds));
    }
}
