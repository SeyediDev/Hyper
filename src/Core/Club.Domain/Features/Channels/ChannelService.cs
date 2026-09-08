using Microsoft.AspNetCore.Http;
using Neo.Domain.Features.Client;

namespace Hyper.Domain.Features.Channels;

/// <summary>
/// Interface برای سرویس مدیریت کانال‌ها
/// </summary>
public interface IChannelService
{
    /// <summary>
    /// دریافت کانال با کلید
    /// </summary>
    Task<EventChannelDto?> GetChannelByKeyAsync(string channelKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت کانال با شناسه کلاینت OAuth
    /// </summary>
    Task<EventChannelDto?> GetChannelByClientIdAsync(string clientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لیست کانال‌ها
    /// </summary>
    Task<List<EventChannelDto>> GetChannelsAsync(int tenantId = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لیست اکوسیستم‌ها و کانال‌ها به صورت یکجا
    /// </summary>
    Task<TenantsAndChannelsResponseDto> GetTenantsAndChannelsAsync(CancellationToken cancellationToken = default);
    string? GetClientIdFromToken(HttpContext httpContext);
    Task<EventChannelDto?> GetChannelFromTokenAsync(HttpContext httpContext, CancellationToken cancellationToken = default);
    Task<EventChannelDto?> GetChannelFromUserAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO برای اطلاعات کانال با اطلاعات Tenant
/// </summary>
public sealed record EventChannelDto
{
    public int Id { get; init; }
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public int TenantId { get; init; }
    public string TenantTitle { get; init; } = null!;
    public string? ClientId { get; init; }
}

/// <summary>
/// DTO برای اطلاعات ساده کانال (بدون TenantTitle)
/// </summary>
public sealed record EventChannelSimpleDto
{
    public int Id { get; init; }
    public string Key { get; init; } = null!;
    public string Title { get; init; } = null!;
    public int TenantId { get; init; }
}

/// <summary>
/// DTO برای اطلاعات Tenant
/// </summary>
public sealed record TenantDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}

/// <summary>
/// DTO برای پاسخ شامل لیست اکوسیستم‌ها و کانال‌ها
/// </summary>
public sealed record TenantsAndChannelsResponseDto
{
    public List<TenantDto> Tenants { get; init; } = [];
    public List<EventChannelSimpleDto> Channels { get; init; } = [];
}

/// <summary>
/// سرویس مدیریت کانال‌ها
/// </summary>
public sealed class ChannelService(
    IQueryRepository<EventChannel, int> eventChannelRepository,
    IQueryRepository<Tenant, int> tenantRepository,
    IRequesterUser user,
    ILogger<ChannelService> logger
    ) : IChannelService
{
    public async Task<EventChannelDto?> GetChannelByKeyAsync(
        string channelKey,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting channel by key {ChannelKey}", channelKey);

        var channel = await eventChannelRepository.FirstOrDefaultWithIncludeAsync(
            x => x.Tenant,
            x => x.Key == channelKey && !x.IsDeleted,
            cancellationToken);

        if (channel == null)
        {
            logger.LogWarning("Channel with key {ChannelKey} not found", channelKey);
            return null;
        }

        logger.LogDebug("Successfully retrieved channel {ChannelKey} with tenant {TenantId}", channelKey, channel.TenantId);

        return new EventChannelDto
        {
            Id = channel.Id,
            Key = channel.Key,
            Title = channel.Title,
            TenantId = channel.TenantId,
            TenantTitle = channel.Tenant?.Title ?? await GetTenantTitleAsync(channel.TenantId, cancellationToken),
            ClientId = channel.ClientId
        };
    }

    public async Task<EventChannelDto?> GetChannelByClientIdAsync(
        string clientId,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting channel by client ID {ClientId}", clientId);

        var channel = await eventChannelRepository.FirstOrDefaultWithIncludeAsync(
            x => x.Tenant,
            x => x.ClientId == clientId && !x.IsDeleted,
            cancellationToken);

        if (channel == null)
        {
            logger.LogWarning("Channel with client ID {ClientId} not found", clientId);
            return null;
        }

        logger.LogDebug("Successfully retrieved channel {ChannelKey} for client ID {ClientId}", channel.Key, clientId);

        return new EventChannelDto
        {
            Id = channel.Id,
            Key = channel.Key,
            Title = channel.Title,
            TenantId = channel.TenantId,
            TenantTitle = channel.Tenant?.Title ?? await GetTenantTitleAsync(channel.TenantId, cancellationToken),
            ClientId = channel.ClientId
        };
    }

    public async Task<List<EventChannelDto>> GetChannelsAsync(
        int tenantId = 0,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting channels. TenantId: {TenantId}", tenantId == 0 ? "all" : tenantId);

        var query = tenantId > 0
            ? eventChannelRepository.Query().Where(x => x.TenantId == tenantId && !x.IsDeleted)
            : eventChannelRepository.Query().Where(x => !x.IsDeleted);
        
        query = query.Include(x => x.Tenant);

        var eventChannels = await query.ToListAsync(cancellationToken);

        var result = new List<EventChannelDto>();
        foreach (var ec in eventChannels)
        {
            result.Add(new EventChannelDto
            {
                Id = ec.Id,
                Key = ec.Key,
                Title = ec.Title,
                TenantId = ec.TenantId,
                TenantTitle = ec.Tenant?.Title ?? await GetTenantTitleAsync(ec.TenantId, cancellationToken),
                ClientId = ec.ClientId,
            });
        }

        logger.LogDebug("Successfully retrieved {Count} channels", result.Count);

        return result;
    }

    public async Task<TenantsAndChannelsResponseDto> GetTenantsAndChannelsAsync(
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting tenants and channels");

        // دریافت همه کانال‌ها
        var eventChannels = await eventChannelRepository.Query()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Tenant)
            .ToListAsync(cancellationToken);

        // دریافت همه Tenantها
        var tenants = await tenantRepository.Query()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        // استخراج اکوسیستم‌های منحصر به فرد بر اساس TenantTitle (Group By TenantTitle)
        var tenantMap = BuildUniqueTenantsMap(tenants);

        // ساخت لیست کانال‌ها (بدون TenantTitle)
        var channels = eventChannels
            .Select(ec => new EventChannelSimpleDto
            {
                Id = ec.Id,
                Key = ec.Key,
                Title = ec.Title,
                TenantId = ec.TenantId
            })
            .OrderBy(c => c.Title)
            .ToList();

        var result = new TenantsAndChannelsResponseDto
        {
            Tenants = tenantMap.Values.OrderBy(t => t.Name).ToList(),
            Channels = channels
        };

        logger.LogDebug("Successfully retrieved {TenantCount} tenants and {ChannelCount} channels", 
            result.Tenants.Count, result.Channels.Count);

        return result;
    }

    private const string ClientIdClaim = "client_id";
    private const string NameIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
    /// <summary>
    /// استخراج clientId از claim توکن
    /// </summary>
    public string? GetClientIdFromToken(HttpContext httpContext)
    {
        // اول از claim "client_id" استخراج کن
        var clientIdClaim = httpContext.User?.FindFirst(ClientIdClaim)?.Value;
        if (!string.IsNullOrWhiteSpace(clientIdClaim))
        {
            return clientIdClaim;
        }

        // Fallback به claim "nameid" (NameIdentifier)
        var nameIdClaim = httpContext.User?.FindFirst(NameIdClaim)?.Value;
        if (!string.IsNullOrWhiteSpace(nameIdClaim))
        {
            return nameIdClaim;
        }

        return null;
    }

    public async Task<EventChannelDto?> GetChannelFromTokenAsync(HttpContext httpContext, CancellationToken cancellationToken = default)
    {
        string? clientId = GetClientIdFromToken(httpContext);
        if (string.IsNullOrWhiteSpace(clientId))
            return null;
        return await GetChannelByClientIdAsync(clientId, cancellationToken);
    }

    public async Task<EventChannelDto?> GetChannelFromUserAsync(CancellationToken cancellationToken = default)
    {
        string? clientId =
            user.Claims()?.FirstOrDefault(x=>x.Type==ClientIdClaim)?.Value ??
            user.Claims()?.FirstOrDefault(x => x.Type == NameIdClaim)?.Value;
        if (string.IsNullOrWhiteSpace(clientId))
            return null;
        return await GetChannelByClientIdAsync(clientId, cancellationToken);
    }

    private static Dictionary<string, TenantDto> BuildUniqueTenantsMap(List<Tenant> tenants)
    {
        var tenantMap = new Dictionary<string, TenantDto>();
        foreach (var tenant in tenants)
        {
            if (!string.IsNullOrWhiteSpace(tenant.Title) && !tenantMap.ContainsKey(tenant.Title))
            {
                tenantMap[tenant.Title] = new TenantDto
                {
                    Id = tenant.Id,
                    Name = tenant.Title
                };
            }
        }
        return tenantMap;
    }

    private async Task<string> GetTenantTitleAsync(int tenantId, CancellationToken cancellationToken)
    {
        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        return tenant?.Title ?? $"Tenant {tenantId}";
    }
}

