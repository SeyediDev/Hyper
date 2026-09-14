> گزارش تاریخی: مرجع جاری سناریوی وب، Backend/docs/BASALAM_OAUTH_WEB.md و Backend/docs/BACKLOG.md است. وضعیت‌های تکمیل و راه‌اندازی این فایل را با مرجع جاری تطبیق دهید.

# Basalam OAuth2 Token Management

## خلاصه

پیاده‌سازی OAuth2 Authorization Code Flow + PKCE برای لاگین و دریافت توکن Basalam. توکن‌ها در جدول `ExternalOAuthTokens` رمزگذاری شده ذخیره می‌شوند.

## معماری

### Entities
- **ExternalOAuthToken** (`src/Core/Hyper.Domain/Entities/Integrations/ExternalOAuthToken.cs`)
  - ذخیره‌سازی access_token و refresh_token رمزگذاری شده
  - شامل metadata: connectionId, shopId, tenantId, provider، issued/expires timestamps
  - فیلدهای رمزنگاری شده: `AccessToken`، `RefreshToken`

### Services
- **BasalamOAuthService** (`src/Core/Hyper.Infrastructure/Features/Integrations/BasalamOAuthService.cs`)
  - `CreateAuthorizationUrl(state, out codeVerifier)` - ایجاد auth URL + PKCE pair
  - `ExchangeCodeForTokenAsync(code, state)` - تبادل code برای access_token
  - `CreateTokenEntity(...)` - ایجاد و رمزگذاری entity برای ذخیره
  - `EncryptToken() / DecryptToken()` - استفاده از IDataProtection

### Controller
- **OAuthCallbackController** (`src/AdminPanel/Hyper.AdminPanel.Web/Controllers/OAuthCallbackController.cs`)
  - `POST /api/auth/basalam/login` - شروع process
  - `GET /api/auth/basalam/callback` - دریافت callback از Basalam

### Stateless State Management
- shopId، connectionId، tenantId در JSON رمزگذاری شده **encrypt** می‌شوند
- هیچ Session استفاده نمی‌شود - state خود معلومات را حمل می‌کند
- رمز‌گشایی بر اساس IDataProtectionProvider در callback

## Flow

```
1. LoginInitiate
   POST /api/auth/basalam/login?shopId=1&connectionId=1&tenantId="tenant1"
   ↓
   state = UUID
   codeVerifier = random(128 bytes base64)
   stateData = {shopId, connectionId, tenantId}
   encryptedState = IDataProtection.Protect(JSON(stateData))
   authUrl = https://auth.basalam.com/oauth/authorize?
			client_id=...&redirect_uri=...&state=UUID&
			code_challenge=SHA256(codeVerifier)&encrypted_state=...
   ↓
   Response: {redirectUrl: "https://auth.basalam.com/..."}

2. User Authorizes at Basalam
   ↓

3. Callback
   GET /api/auth/basalam/callback?code=AUTH_CODE&state=UUID&encrypted_state=...
   ↓
   codeVerifier = cache.Get("basalam_cv_" + state)
   stateData = IDataProtection.Unprotect(encrypted_state)
   tokenResponse = POST to token endpoint with code + codeVerifier
   token = Create ExternalOAuthToken(encrypted accessToken/refreshToken)
   DbContext.ExternalOAuthTokens.Add(token)
   SaveChanges()
   ↓
   Response: {message: "توکن با موفقیت ذخیره شد", shopId, connectionId}
```

## Configuration

### appsettings.json
```json
{
  "Basalam": {
	"ClientId": "your-client-id",
	"ClientSecret": "your-client-secret",
	"AuthorizationEndpoint": "https://auth.basalam.com/oauth/authorize",
	"TokenEndpoint": "https://auth.basalam.com/oauth/token",
	"RedirectUri": "http://localhost:5001/api/auth/basalam/callback",
	"Scopes": "inventory.read orders.read products.read"
  }
}
```

**نکاتی مهم:**
- ClientSecret را به User Secrets یا KeyVault منتقل کنید (production)
- RedirectUri باید در Basalam Developer Console ثبت شده باشد
- Scopes را بر اساس نیاز تغییر دهید

### DI Registration
```csharp
// src/Core/Hyper.Infrastructure/DependencyInjection.cs
services.Configure<BasalamOAuthSettings>(configuration.GetSection("Basalam"));
services.AddHttpClient<BasalamOAuthService>()
	.ConfigureHttpClient(client =>
	{
		client.DefaultRequestHeaders.Add("User-Agent", "HyperIntegration/1.0");
	});
services.AddDataProtection();
```

## Database

### Table: ExternalOAuthTokens
```sql
CREATE TABLE [dbo].[ExternalOAuthTokens] (
	[Id] bigint PRIMARY KEY IDENTITY(1,1),
	[ConnectionId] bigint NOT NULL,
	[ShopId] int NOT NULL,
	[TenantId] nvarchar(450) NOT NULL,
	[Provider] nvarchar(50) NOT NULL,
	[AccessToken] nvarchar(MAX) NOT NULL (encrypted),
	[RefreshToken] nvarchar(MAX) NULL (encrypted),
	[TokenType] nvarchar(50) NOT NULL,
	[ExpiresAtUtc] datetime2 NULL,
	[Scopes] nvarchar(MAX) NULL,
	[IssuedAtUtc] datetime2 DEFAULT SYSUTCDATETIME(),
	[UpdatedAtUtc] datetime2 DEFAULT SYSUTCDATETIME(),
	[IsActive] bit DEFAULT 1,
	[RawTokenResponse] nvarchar(MAX) NULL
);

CREATE UNIQUE INDEX UX_ExternalOAuthTokens_ShopId_Provider 
	ON [dbo].[ExternalOAuthTokens] (ShopId, Provider);
CREATE INDEX IX_ExternalOAuthTokens_TenantId 
	ON [dbo].[ExternalOAuthTokens] (TenantId);
CREATE INDEX IX_ExternalOAuthTokens_ConnectionId 
	ON [dbo].[ExternalOAuthTokens] (ConnectionId);
CREATE INDEX IX_ExternalOAuthTokens_ExpiresAtUtc 
	ON [dbo].[ExternalOAuthTokens] (ExpiresAtUtc);
```

### EF Configuration
```csharp
// src/Core/Hyper.Infrastructure/Data/Configurations/ExternalOAuthTokenConfiguration.cs
entity.ToTable("ExternalOAuthTokens", "dbo");
entity.HasKey(e => e.Id);
entity.Property(e => e.IssuedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
entity.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
entity.Property(e => e.IsActive).HasDefaultValue(true);
```

## Migration

### فایلات Migration
- `src/Core/Hyper.Infrastructure/Migrations/20260914_AddExternalOAuthToken.cs`
- `src/Core/Hyper.Infrastructure/Migrations/HyperContextCommandModelSnapshot.cs`

### اعمال Migration
```bash
dotnet ef database update --project src/Core/Hyper.Infrastructure \
	--startup-project src/AdminPanel/Hyper.AdminPanel.Web \
	--context HyperContextCommand
```

**⚠️ مسئله فعلی:** EF Core version mismatch (8.0 vs 10.0)
- نیاز به upgrade packages: EntityFrameworkCore.SqlServer، EntityFrameworkCore.Tools
- برای test کردن می‌توان manually table ایجاد کرد یا Seed کردن اضافی

## Security

### IDataProtection
- مقدار tokens در دیتابیس رمزگذاری می‌شوند
- Keys به صورت پیش‌فرض در `C:\ProgramData\ASP.NET\DataProtection-Keys` ذخیره می‌شوند
- **Production:** اطمینان حاصل کنید keys persistent هستند (file system یا Azure KeyVault)

### PKCE (Proof Key for Code Exchange)
- code_verifier: 128 بایت random base64
- code_challenge: SHA256(code_verifier)
- code_challenge_method: S256
- cache expiration: 10 دقیقه

### State Encryption
- state parameter: UUID
- encrypted_state: JSON(shopId, connectionId, tenantId) با IDataProtection.Protect()
- رمزگشایی در callback برای اطمینان integrity

## Testing

### Local Testing  
```bash
# Start application
dotnet run --project src/AdminPanel/Hyper.AdminPanel.Web

# Initiate login
curl -X POST "http://localhost:5001/api/auth/basalam/login?shopId=1&connectionId=1&tenantId=test"

# Result: {redirectUrl: "https://auth.basalam.com/..."}
# Copy URL and open in browser → authorize → redirect to callback
```

### Ngrok for Public Redirect (Optional)
```bash
ngrok http 5001
# Update RedirectUri in appsettings.json:
# "RedirectUri": "https://YOUR_NGROK_URL/api/auth/basalam/callback"
```

## Next Steps

1. ✅ Encrypt/Decrypt implementation
2. ✅ Stateless state management
3. ✅ OAuth flow endpoints
4. ⏳ EF Core version upgrade (8.0 → 10.0)
5. ⏳ Database migration execution
6. ⏳ Token refresh logic (when expiresAtUtc reached)
7. ⏳ Integration with Admin UI (لاگین button برای انتخاب شده merchant)
8. ⏳ Error handling و retry logic
9. ⏳ Audit logging برای token operations

## Files Modified/Created

- `src/Core/Hyper.Domain/Entities/Integrations/ExternalOAuthToken.cs` (NEW)
- `src/Core/Hyper.Infrastructure/Data/Configurations/ExternalOAuthTokenConfiguration.cs` (NEW)
- `src/Core/Hyper.Infrastructure/Data/Repository/Hyper/HyperContext.cs` (MODIFIED - added DbSet)
- `src/Core/Hyper.Infrastructure/Features/Integrations/BasalamOAuthService.cs` (NEW)
- `src/AdminPanel/Hyper.AdminPanel.Web/Controllers/OAuthCallbackController.cs` (NEW)
- `src/AdminPanel/Hyper.AdminPanel.Web/appsettings.json` (MODIFIED - added Basalam config)
- `src/Core/Hyper.Infrastructure/DependencyInjection.cs` (MODIFIED - added DI registrations)
- `src/Core/Hyper.Infrastructure/Migrations/20260914_AddExternalOAuthToken.cs` (NEW)
- `src/Core/Hyper.Infrastructure/Migrations/HyperContextCommandModelSnapshot.cs` (NEW)

## References

- [OAuth 2.0 Authorization Code Flow](https://tools.ietf.org/html/rfc6749#section-1.3.1)
- [PKCE (RFC 7636)](https://tools.ietf.org/html/rfc7636)
- [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/)
- [Basalam API Documentation](https://api.basalam.com/docs) (if available)
