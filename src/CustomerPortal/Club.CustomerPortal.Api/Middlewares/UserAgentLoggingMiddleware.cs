namespace Hyper.CustomerPortal.Api.Middlewares;

public class UserAgentLoggingMiddleware(RequestDelegate next, ILogger<UserAgentLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var userAgent = context.Request.Headers["User-Agent"].ToString();
        if (!string.IsNullOrWhiteSpace(userAgent))
        {
            logger.LogInformation("User-Agent: {UserAgent}", userAgent);
        }

        await next(context);
    }
}

