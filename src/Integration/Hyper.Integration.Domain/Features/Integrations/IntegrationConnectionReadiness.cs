namespace Hyper.Integration.Domain.Features.Integrations;

/// <summary>Checks authorization-independent connection readiness before any provider call.</summary>
public static class IntegrationConnectionReadiness
{
    public static void Validate(ExternalIntegrationConnection connection, DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(connection);
        if (!connection.IsEnabled)
            throw new InvalidOperationException("The integration connection is disabled.");
        if (connection.ShopId <= 0 || string.IsNullOrWhiteSpace(connection.TenantId)
            || string.IsNullOrWhiteSpace(connection.AccountIdentifier))
            throw new InvalidOperationException("The integration connection has incomplete ownership metadata.");
        if (string.IsNullOrWhiteSpace(connection.CredentialsJson))
            throw new InvalidOperationException("Integration credentials are missing.");
        if (connection.ExpiresAtUtc is { } expiry && expiry <= utcNow)
            throw new InvalidOperationException("Integration credentials have expired; renew them before synchronization.");
    }
}

