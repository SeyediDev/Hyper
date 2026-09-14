> گزارش تاریخی: مرجع جاری سناریوی وب، Backend/docs/BASALAM_OAUTH_WEB.md و Backend/docs/BACKLOG.md است. وضعیت‌های تکمیل و راه‌اندازی این فایل را با مرجع جاری تطبیق دهید.

# 📖 Basalam OAuth Implementation - Quick Start Guide

## شروع سریع برای توسعه‌دهندگان دیگر

### ✅ آنچه تمام شده است
- ✓ OAuth2 service layer (BasalamOAuthService)
- ✓ API endpoints (login + callback)
- ✓ Database entity & configuration
- ✓ Encryption setup (IDataProtection)
- ✓ Stateless state management
- ✓ Build is successful

### ⏹️ اند این جا متوقف شده است
**مسئله:** EF Core version mismatch (8.0 vs 10.0)  
**تأثیر:** نمی‌تواند migration را اعمال کند  
**حل:** Upgrade EntityFrameworkCore packages

---

## 🚀 شروع در نشست بعدی

### Step 1: مشکل EF Core را حل کنید
```bash
# پیدا کنید: کجا package versions مدیریت می‌شوند
# احتمالاً: Directory.Packages.props یا .csproj files

# Upgrade to EF Core 10.x
# vim src/Core/Hyper.Infrastructure/Hyper.Infrastructure.csproj
# تغییر دهید:
#   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
#   <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
```

### Step 2: Migration را اعمال کنید
```bash
cd E:\SJVS\Projects\Hyper\Backend

dotnet ef database update \
  --project src/Core/Hyper.Infrastructure \
  --startup-project src/AdminPanel/Hyper.AdminPanel.Web \
  --context HyperContextCommand
```

### Step 3: دیتابیس را تأیید کنید
```sql
-- باید جدول وجود داشته باشد (خالی)
SELECT Count(*) as TokenCount FROM dbo.ExternalOAuthTokens;
```

### Step 4: Basalam Credentials را تنظیم کنید
```json
// src/AdminPanel/Hyper.AdminPanel.Web/appsettings.json
{
  "Basalam": {
	"ClientId": "YOUR_ACTUAL_CLIENT_ID",
	"ClientSecret": "YOUR_ACTUAL_SECRET",
	"AuthorizationEndpoint": "https://auth.basalam.com/oauth/authorize",
	"TokenEndpoint": "https://auth.basalam.com/oauth/token",
	"RedirectUri": "http://localhost:5001/api/auth/basalam/callback",
	"Scopes": "inventory.read orders.read products.read"
  }
}
```

### Step 5: OAuth Flow را تست کنید
```bash
# شروع اپلیکیشن
dotnet run --project src/AdminPanel/Hyper.AdminPanel.Web

# آزمایش Initiate (نیاز نیست DB موجود باشد)
curl -X POST "http://localhost:5001/api/auth/basalam/login?shopId=1&connectionId=1&tenantId=test"

# Response:
# {"redirectUrl":"https://auth.basalam.com/oauth/authorize?..."}

# در مرورگر: این URL را باز کنید → اجازه دهید → redirect
```

---

## 📂 فایلهای کلیدی

```
src/Core/Hyper.Domain/Entities/Integrations/
  └─ ExternalOAuthToken.cs         ← Entity for storing tokens

src/Core/Hyper.Infrastructure/Features/Integrations/
  ├─ BasalamOAuthService.cs        ← OAuth logic (PKCE + encryption)
  ├─ ExternalOAuthTokenConfiguration.cs
  └─ BASALAM_OAUTH_IMPLEMENTATION.md  ← Full documentation

src/AdminPanel/Hyper.AdminPanel.Web/
  ├─ Controllers/OAuthCallbackController.cs  ← API endpoints
  └─ appsettings.json              ← Basalam configuration

Root:
  ├─ BACKLOG.md                    ← Prioritized tasks
  ├─ OAUTH_SESSION_SUMMARY.md      ← Session handoff
  └─ CHANGES_SUMMARY.md            ← What changed
```

---

## 🎯 اگر گیر کردید

### مسئله: `dotnet ef` command کار نمی‌کند
**حل:** 
```bash
# نصب tool
dotnet tool install --global dotnet-ef --version 10.0.0

# یا اپ‌دیت کنید
dotnet tool update --global dotnet-ef
```

### مسئله: Migration برای SQL Server 2019+ نیست
**حل:** Check EF configuration - باید از SqlServer provider استفاده شود:
```csharp
options.UseSqlServer(connectionString);
```

### مسئله: Token encryption خراب است
**حل:** IDataProtection keys باید persistent باشند:
```csharp
// Program.cs
services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo(@"C:\DataProtection-Keys"));
```

### مسئله: State decryption fail می‌شود
**حل:** Check `encrypted_state` parameter:
- باید Base64 encoded باشد
- باید از HTTP request query string آمده باشد
- می‌تواند خیلی طول داشته باشد؟ Check URL length limits

---

## 🔗 لینک‌های مرجع

| Topic | Link |
|-------|------|
| OAuth2 Authorization Code Flow | https://tools.ietf.org/html/rfc6749 |
| PKCE (RFC 7636) | https://tools.ietf.org/html/rfc7636 |
| ASP.NET Core Data Protection | https://docs.microsoft.com/aspnet/core/security/data-protection/ |
| Entity Framework Core Migrations | https://docs.microsoft.com/ef/core/managing-schemas/migrations/ |
| Basalam API Docs | https://api.basalam.com/docs (if available) |

---

## ✅ Checklist برای تکمیل

- [ ] EF Core upgraded to 10.x
- [ ] Migration executed successfully
- [ ] Database table verified
- [ ] Basalam credentials obtained (sandbox)
- [ ] RedirectUri registered in Basalam Console
- [ ] POST /api/auth/basalam/login endpoint works
- [ ] Redirect to Basalam auth server works
- [ ] Callback received and token stored
- [ ] Token encryption verified (gibberish in DB)
- [ ] UI button added to initiate login
- [ ] End-to-end test successful

---

## 💡 نکات طلایی

1. **بدون Session:** هیچ Session استفاده نمی‌شود - state encrypted است در URL
2. **PKCE:** بسیار مهم برای security - نه تنها سبک‌تر بلکه standard است
3. **Token Encryption:** Tokens در DB رمزگذاری شده‌اند - فقط RAM و HTTPS
4. **Stateless:** بهتر scale می‌کند - نیاز به affinity نیست
5. **IMemoryCache:** برای dev خواب - production باید Redis یا SQL Server استفاده کند

---

## 🎓 اگر می‌خواهید توضیح عمیق‌تر

- Read: `BASALAM_OAUTH_IMPLEMENTATION.md` (60 دقیقه)
- Read: `BACKLOG.md` section "ADRs" (20 دقیقه)
- Code walkthrough: `BasalamOAuthService.cs` (30 دقیقه)
- Code walkthrough: `OAuthCallbackController.cs` (15 دقیقه)

---

**Last Updated:** 2024-09-14  
**Format:** Farsi + English (mixed for clarity)  
**Status:** Ready to proceed after EF Core fix
