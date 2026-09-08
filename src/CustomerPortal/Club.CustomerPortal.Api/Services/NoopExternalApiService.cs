using Neo.Domain.Entities.Integrations;
using Neo.Domain.Features.Integrations;

namespace Hyper.CustomerPortal.Api.Services;

internal sealed class NoopExternalApiService : IExternalApiService
{
    public Task<ExternalApiInvocationResult> InvokeAsync(ExternalApi externalApi, ExternalApiRequest request, CancellationToken cancellationToken = default)
    {
        ExternalApiInvocationResult result = new(200, "{}", new Dictionary<string, string[]>());
        return Task.FromResult(result);
    }
}

