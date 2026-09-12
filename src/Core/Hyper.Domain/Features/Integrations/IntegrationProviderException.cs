namespace Hyper.Domain.Features.Integrations;

public sealed class IntegrationProviderException(string code, bool retryable, TimeSpan? retryAfter = null)
    : Exception(code)
{
    public string Code { get; } = code;
    public bool Retryable { get; } = retryable;
    public TimeSpan? RetryAfter { get; } = retryAfter;
}
