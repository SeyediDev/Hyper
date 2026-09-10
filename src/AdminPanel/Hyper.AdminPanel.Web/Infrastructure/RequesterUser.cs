using System.Security.Claims;
using Microsoft.Extensions.Primitives;
using Neo.Domain.Entities.Common;

namespace Hyper.AdminPanel.Web.Infrastructure;

public class RequesterUser(IHttpContextAccessor httpContextAccessor,
    IQueryRepository<Language, LanguageId> languageRepository) : IRequesterUser
{
    private UserId? _id = null;
    public UserId? Id
    {
        get
        {
            if(_id is null)
            {
                var userPrincipal = httpContextAccessor.HttpContext?.User?.Identities?.FirstOrDefault();
                var value = userPrincipal?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value?.ToString();
                _ = int.TryParse(value, out var id);
                _id = (UserId)id;
            }
            return _id;
        }
        set
        {
            _id = value;
        }
    }
    private string _mobile = null!;
    public string Mobile
    {
        get
        {
            _mobile ??= httpContextAccessor.HttpContext?.User?.FindFirstValue("username")
                ?? httpContextAccessor.HttpContext?.User?.Identity?.Name!;
            return _mobile!;
        }
        set
        {
            _mobile = value;
        }
    }
    private string _appName = null!;
    public string AppName
    {
        get
        {
            _appName ??= httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("x-app-name", out StringValues appName) is true
                ? appName.ToString()
                : "office";
            return _appName;
        }
    }

    private string _lang = null!;
    public string Lang
    {
        get
        {
            _lang ??= httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("x-lang", out StringValues langName) is true
                ? langName.ToString()
                : "en";
            return _lang;
        }
    }
    private LanguageId? _langId = null;
    public async Task<LanguageId> GetLangIdAsync(CancellationToken cancellationToken = default)
    {
        if (_langId == null)
        {
            var lang = await languageRepository.FirstOrDefaultAsync(l => l.Name == Lang, cancellationToken);
            _langId = lang?.Id;
        }
        return _langId ?? new(0);
    }

    private string _correlationId = null!;
    public string CorrelationId
    {
        get
        {
            _correlationId ??= httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues correlationId) is true
                ? correlationId.ToString()!
                : null!;
            return _correlationId;
        }
    }

    private string _tenantId = null!;
    public string TenantId
    {
        get
        {
            if (_tenantId == null)
                _tenantId = httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id")!;
            return _tenantId!;
        }
        set
        {
            _tenantId = value;
        }
    }

    private string _userAgent = null!;
    public string Platform
    {
        get
        {
            _userAgent = httpContextAccessor.HttpContext?.Request.Headers?.UserAgent.ToString()!;
            string platform = "UNKNOWN";
            if (_userAgent is not null)
            {
                if (_userAgent.Contains("Android"))
                    platform = "ANDROID";
                else if (_userAgent.Contains("iPhone") || _userAgent.Contains("iPad"))
                    platform = "IOS";
                else if (_userAgent.Contains("Windows"))
                    platform = "WINDOWS";
                else if (_userAgent.Contains("Macintosh"))
                    platform = "MACOS";
                else if (_userAgent.Contains("Linux"))
                    platform = "LINUX";
            }
            return platform;
        }
    }

    public List<Claim> Claims()
    {
        return httpContextAccessor.HttpContext?.User?.Claims?.ToList()!;
    }
    public Dictionary<string, object> Properties { get; set; } = [];

    private bool? _isUserInRole = null;
    public bool? IsInRole(string role)
    {
        _isUserInRole = httpContextAccessor.HttpContext?.User.IsInRole(role) ?? null;
        return _isUserInRole;
    }
}
