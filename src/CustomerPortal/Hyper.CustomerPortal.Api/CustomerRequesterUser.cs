using System.Security.Claims;
using Hyper.CustomerPortal.Application.Interfaces;
using Microsoft.Extensions.Primitives;
using Neo.Common.Extensions;
using Neo.Domain.Entities.Common;
using Neo.Domain.Features.Client;

namespace Hyper.CustomerPortal.Api;

public class CustomerRequesterUser(IHttpContextAccessor httpContextAccessor)
    : IRequesterUser, ICustomerRequesterUser
{
    private UserId? _id = null;
    public UserId? Id
    {
        get
        {
            if (_id is null)
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

    private string? _customerId = null;
    public int CustomerId
    {
        get
        {
            if (_customerId == null && httpContextAccessor.HttpContext?.User?.FindFirstValue("customerId") != null)
            {
                _customerId = httpContextAccessor.HttpContext.User.FindFirstValue("customerId");
            }
            return _customerId?.ToNullableInt32()??0;
        }
        set
        {
            _customerId = value.ToString();
        }
    }

    private string? _mobile = null;
    public string? Mobile
    {
        get
        {
            _mobile ??= httpContextAccessor.HttpContext?.User?.FindFirstValue("mobile")
                ?? httpContextAccessor.HttpContext?.User?.FindFirstValue("username");
            return _mobile;
        }
        set
        {
            _mobile = value;
        }
    }

    private string? _appName = null;
    public string? AppName
    {
        get
        {
            _appName ??= httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("x-app-name", out StringValues appName) is true
                ? appName.ToString()
                : "CustomerPortal";
            return _appName;
        }
    }

    private string? _lang = null;
    public string? Lang
    {
        get
        {
            _lang ??= httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("x-lang", out StringValues langName) is true
                ? langName.ToString()
                : "fa";
            return _lang;
        }
    }

    private LanguageId? _langId = null;
    public Task<LanguageId> GetLangIdAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_langId ??= new(1)); // Default to Persian
    }

    private string? _correlationId = null;
    public string? CorrelationId
    {
        get
        {
            _correlationId ??= httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("X-Correlation-ID", out StringValues correlationId) is true
                ? correlationId.ToString()
                : null;
            return _correlationId;
        }
    }

    public int? TenantId = null;
    /*private string? _tenantId = null;
    public string? TenantId
    {
        get
        {
            _tenantId ??= httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id");
            return _tenantId;
        }
        set
        {
            _tenantId = value;
        }
    }*/

    private string? _userAgent = null;
    public string Platform
    {
        get
        {
            _userAgent = httpContextAccessor.HttpContext?.Request.Headers?.UserAgent.ToString();
            string platform = "WEB";
            if (_userAgent is not null)
            {
                if (_userAgent.Contains("Android"))
                    platform = "ANDROID";
                else if (_userAgent.Contains("iPhone") || _userAgent.Contains("iPad"))
                    platform = "IOS";
            }
            return platform;
        }
    }

    public Dictionary<string, object> Properties { get; set; } = [];
    public List<Claim> Claims()
    {
        return httpContextAccessor.HttpContext?.User?.Claims?.ToList() ?? [];
    }

    private bool? _isUserInRole = null;
    public bool? IsInRole(string role)
    {
        _isUserInRole = httpContextAccessor.HttpContext?.User.IsInRole(role) ?? null;
        return _isUserInRole;
    }

    //TODO
    int? ICustomerRequesterUser.TenantId { get { Properties.TryGetValue("TenantId", out var v); return v?.ToInt(); } }
}
