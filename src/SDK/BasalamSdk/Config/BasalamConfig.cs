using System.Globalization;

namespace Basalam.SDK.Config;

public enum BasalamEnvironment
{
    Production,
    Development
}

public record ServiceConfig
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public string ApiVersion { get; init; } = "v1";
    public string BaseUrl { get; init; } = "https://openapi.basalam.com";
    public string Url
    {
        get => $"{BaseUrl.TrimEnd('/')}/{ApiVersion}/{Path.TrimStart('/')}";
    }
}

public sealed class BasalamConfig
{
    private static readonly Dictionary<BasalamEnvironment, string> BaseUrls = new()
    {
        [BasalamEnvironment.Production] = "https://openapi.basalam.com",
        [BasalamEnvironment.Development] = "https://openapi.basalam.dev"
    };

    private static readonly Dictionary<BasalamEnvironment, string> AuthUrls = new()
    {
        [BasalamEnvironment.Production] = "https://auth.basalam.com/oauth/token",
        [BasalamEnvironment.Development] = "https://auth.basalam.dev/oauth/token"
    };

    private static readonly Dictionary<BasalamEnvironment, string> AuthorizeUrls = new()
    {
        [BasalamEnvironment.Production] = "https://basalam.com/accounts/sso",
        [BasalamEnvironment.Development] = "https://basalam.dev/accounts/sso"
    };

    private static readonly Dictionary<BasalamEnvironment, string> UserAgentUrls = new()
    {
        [BasalamEnvironment.Production] = "basalam-python-sdk",
        [BasalamEnvironment.Development] = "basalam-python-sdk-dev"
    };

    public BasalamConfig()
    {
        Environment = BasalamEnvironment.Production;
        TimeoutSeconds = 30;
    }

    public BasalamEnvironment Environment { get; set; }
    public int TimeoutSeconds { get; set; }
    public string? UserAgent { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? ApiBaseUrl { get; set; }

    public string TokenEndpoint => AuthUrls[Environment];
    public string AuthorizeEndpoint => AuthorizeUrls[Environment];

    public static ServiceConfig GetServiceConfig(string serviceName)
    {
        var services = ServiceConfigs;
        if (services.TryGetValue(serviceName.ToLowerInvariant(), out var config))
            return config;
        throw new ArgumentException($"Unknown service: {serviceName}");
    }

    public static IReadOnlyDictionary<string, ServiceConfig> ServiceConfigs { get; } = new Dictionary<string, ServiceConfig>(StringComparer.OrdinalIgnoreCase)
    {
        ["core"] = new ServiceConfig { Name = "core", Path = "core" },
        ["catalog"] = new ServiceConfig { Name = "catalog", Path = "catalog" },
        ["product"] = new ServiceConfig { Name = "product", Path = "product" },
        ["vendor"] = new ServiceConfig { Name = "vendor", Path = "vendor" },
        ["user"] = new ServiceConfig { Name = "user", Path = "user" },
        ["shipping"] = new ServiceConfig { Name = "shipping", Path = "shipping" },
        ["order"] = new ServiceConfig { Name = "order", Path = "order" },
        ["wallet"] = new ServiceConfig { Name = "wallet", Path = "wallet" },
        ["upload"] = new ServiceConfig { Name = "upload", Path = "upload" },
        ["webhook"] = new ServiceConfig { Name = "webhook", Path = "webhook" },
        ["chat"] = new ServiceConfig { Name = "chat", Path = "chat" },
        ["appstore"] = new ServiceConfig { Name = "appstore", Path = "appstore" },
    };

    public string ResolveServiceUrl(string serviceName)
    {
        var config = GetServiceConfig(serviceName);
        var baseUrl = BaseUrls[Environment];
        return $"{baseUrl.TrimEnd('/')}/{config.ApiVersion}/{config.Path.TrimStart('/')}";
    }

    public string ResolveRequestUrl(string path)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out var absolute)) return absolute.AbsoluteUri;
        var baseUrl = string.IsNullOrWhiteSpace(ApiBaseUrl) ? BaseUrls[Environment] : ApiBaseUrl;
        return $"{baseUrl.TrimEnd('/')}/{path.TrimStart('/')}";
    }

    public string GetUserAgent()
    {
        return UserAgent ?? UserAgentUrls[Environment];
    }
}
