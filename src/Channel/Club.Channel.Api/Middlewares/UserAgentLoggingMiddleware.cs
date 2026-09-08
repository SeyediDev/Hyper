namespace Hyper.Channel.Api.Middlewares;

public class UserAgentLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserAgentLoggingMiddleware> _logger;

    public UserAgentLoggingMiddleware(RequestDelegate next, ILogger<UserAgentLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();

        if (string.IsNullOrWhiteSpace(userAgent))
        {
            userAgent = "DefaultUserAgent/1.0";
            context.Request.Headers["User-Agent"] = userAgent;
        }

        using (_logger.BeginScope(new Dictionary<string, object> { ["UserAgent"] = userAgent }))
        {
            await _next(context);
        }
    }
}