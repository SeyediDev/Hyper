> گزارش تاریخی: مرجع جاری سناریوی وب، Backend/docs/BASALAM_OAUTH_WEB.md و Backend/docs/BACKLOG.md است. وضعیت‌های تکمیل و راه‌اندازی این فایل را با مرجع جاری تطبیق دهید.

# Hyper Backend - Development Backlog

## Basalam OAuth2 Integration - دریافت توکن و ذخیره

### ✅ Completed (تکمیل شده)
- [x] Create ExternalOAuthToken domain entity (src/Core/Hyper.Domain/Entities/Integrations/ExternalOAuthToken.cs)
- [x] Create EF configuration & add DbSet to HyperContext
- [x] Implement BasalamOAuthService with PKCE + token encryption
- [x] Implement OAuthCallbackController (POST /api/auth/basalam/login, GET /api/auth/basalam/callback)
- [x] Add DI registrations for BasalamOAuthService, HttpClient, DataProtection
- [x] Add Basalam configuration to appsettings.json
- [x] Implement stateless state management (encrypted state without session)
- [x] Create migration files 
- [x] Build successful - all code compiles
- [x] Documentation: BASALAM_OAUTH_IMPLEMENTATION.md

### 🔴 Blocked (مسدود شده)
- [ ] **EF Core Version Mismatch**: Project uses EF Core 8.0 but .NET 10 requires 10.x
  - Need to upgrade: EntityFrameworkCore.SqlServer, EntityFrameworkCore.Tools
  - Impact: Cannot run `dotnet ef database update` command
  - Workaround: Manually create table or use SQL script

### ⏳ In Progress (درحال انجام)
- [ ] Apply database migration to create ExternalOAuthTokens table
  - Blocked by EF Core version upgrade

### 📋 To Do (باقی مانده)

#### Phase 1: Core OAuth Flow
- [ ] Verify token encryption/decryption works with IDataProtection
- [ ] Test OAuth callback with Basalam sandbox
- [ ] Handle OAuth errors (invalid code, expired state, network errors)
- [ ] Implement token refresh logic (when expiresAtUtc < now)
- [ ] Add refresh token endpoint: POST /api/auth/basalam/refresh

#### Phase 2: Admin UI Integration
- [ ] Create Admin page to select merchant/shop
- [ ] Add "Login with Basalam" button
- [ ] Display connected shops and their token status
- [ ] Wire UI to call POST /api/auth/basalam/login with selected shop params

#### Phase 3: Token Usage
- [ ] Implement service to retrieve stored tokens (DecryptToken)
- [ ] Create adapter to use Basalam API with stored tokens
  - Sync products/inventory from Basalam
  - Sync orders to Basalam
- [ ] Audit logging: log all token gen/refresh/usage
- [ ] Token revocation: add IsActive flag handling

#### Phase 4: Security & Production
- [ ] Move ClientSecret to User Secrets (dev) or KeyVault (production)
- [ ] Configure IDataProtection key persistence
  - File system: ensure keys survives app restart
  - Or: Use Azure KeyVault for distributed apps
- [ ] Register RedirectUri with Basalam Developer Portal
- [ ] Validate SSL/TLS for callback endpoint
- [ ] Add rate limiting to OAuth endpoints

#### Phase 5: Monitoring & Maintenance
- [ ] Add health check for OAuth service
- [ ] Monitor token expiration and auto-refresh
- [ ] Implement token cleanup job (remove old/revoked tokens)
- [ ] Add metrics: successful logins, failed exchanges, token refreshes
- [ ] Error telemetry: log all OAuth failures

### 🐛 Known Issues
1. **EF Core Version Mismatch** (BLOCKING)
   - Environment: .NET 10 + EF Core 8.0 → Type loading error
   - Solution: Upgrade to EF Core 10.x
   - Priority: CRITICAL - blocks database operations

2. **State Encryption Verification Needed**
   - Ensure encrypted_state is URL-safe and properly decoded
   - Test with long state data

3. **Cache Expiration**
   - IMemoryCache only suitable for single-instance dev
   - Production: switch to IDistributedCache (Redis/SQL Server)

### 📝 Documentation Files
- `src/Core/Hyper.Infrastructure/Features/Integrations/BASALAM_OAUTH_IMPLEMENTATION.md`
- `src/Core/Hyper.Domain/Features/Integrations/README.md` (existing)

### 🔗 Related Stories
- **Story 1: Connect Shop to Basalam** - Admin selects shop → initiates OAuth → stores token
- **Story 2: Sync Basalam Products** - Use stored token to fetch products from Basalam
- **Story 3: Sync Orders to Basalam** - Push orders to Basalam using API

### 🏗️ Architecture Decision Records (ADRs)

**ADR-001: Stateless State Management**
- Decision: Use encrypted IDataProtection instead of session
- Rationale: Stateless = easier scaling, no session affinity needed, cleaner code
- Trade-off: Slight crypto overhead vs session memory usage
- Alternative: Redis distributed session (rejected for now due to complexity)

**ADR-002: Separate Tokens Table**
- Decision: Create dedicated ExternalOAuthTokens table instead of CredentialsJson
- Rationale: Better indexing, lifecycle tracking, type safety, audit logging
- Trade-off: More DB schema vs simpler code
- Alternative: Store in ExternalIntegrationConnection.CredentialsJson (rejected)

**ADR-003: IDataProtection for Tokens**
- Decision: Encrypt tokens at rest using ASP.NET Core IDataProtection
- Rationale: Built-in, integrated with app lifecycle, key management
- Trade-off: Keys not portable between instances without setup
- Alternative: HashiCorp Vault (overkill), customer-managed keys (more complex)

### 💡 Future Enhancements
- [ ] Support for other OAuth providers (Digikala, Trez, etc.)
- [ ] Multi-store OAuth (one user connects multiple stores)
- [ ] Delegated token management (shop owner revokes/renews without going through OAuth)
- [ ] Token migration tool (if changing encryption strategy)
- [ ] GraphQL mutation for initiating OAuth flow
- [ ] Webhook support (Basalam notifies of token expiry)

---

**Last Updated**: 2024-09-14  
**Updated By**: Copilot  
**Status**: Awaiting EF Core upgrade to proceed
