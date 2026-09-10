using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Hyper.Channel.Api.Infrastructure.Swagger;

/// <summary>
/// Document Processor برای اضافه کردن Security Schemes به Swagger
/// </summary>
public class SwaggerDocumentProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        var document = context.Document;

        // اضافه کردن Security Schemes
        if (document.Components == null)
        {
            return; // Components باید از قبل initialize شده باشد
        }
        
        // OAuth 2.0 Client Credentials
        if (!document.Components.SecuritySchemes.ContainsKey("OAuth2"))
        {
            document.Components.SecuritySchemes.Add("OAuth2", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.OAuth2,
                Description = "OAuth 2.0 Client Credentials Flow",
                Flows = new OpenApiOAuthFlows
                {
                    ClientCredentials = new OpenApiOAuthFlow
                    {
                        TokenUrl = "/api/v1/auth/token",
                        Scopes = new Dictionary<string, string>
                        {
                            { "channel:write", "دسترسی به ارسال رویدادها" },
                            { "channel:read", "دسترسی به خواندن اطلاعات کانال" }
                        }
                    }
                }
            });
        }

        // Bearer Token (برای استفاده مستقیم از token)
        if (!document.Components.SecuritySchemes.ContainsKey("Bearer"))
        {
            document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT token دریافت شده از endpoint /api/v1/auth/token"
            });
        }

        // اضافه کردن Security به تمام endpoints (به جز auth)
        // monitoring از طریق ApiExplorerSettings(IgnoreApi = true) از Swagger حذف می‌شود
        if (document.Paths != null)
        {
            foreach (var path in document.Paths)
            {
                foreach (var operation in path.Value.Values)
                {
                    // اگر endpoint مربوط به auth نباشد، security را اضافه کن
                    if (!path.Key.Contains("/auth/", StringComparison.OrdinalIgnoreCase))
                    {
                        operation.Security ??= [];
                        
                        // فقط Bearer را اضافه می‌کنیم (OAuth2 برای Client Credentials در Swagger UI کار نمی‌کند)
                        var securityRequirement = new OpenApiSecurityRequirement
                        {
                            ["Bearer"] = Array.Empty<string>()
                        };
                        operation.Security.Add(securityRequirement);
                    }
                }
            }
        }

        // اضافه کردن لینک مانیتورینگ به Swagger UI
        // این کار از طریق HeadContent در Program.cs انجام می‌شود
        // چون NSwag OpenApiInfo Extensions را پشتیبانی نمی‌کند
    }
}

