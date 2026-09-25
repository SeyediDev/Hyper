# به نام خدا

## آزمون جریان کلان API

- ارسال webhook معتبر باسلام: یک inbox، یک audit و یک job متناظر ایجاد شود.
- ارسال webhook معتبر Hyperyek: همان جریان برای eventهای حسابداری اجرا شود.
- ارسال دوبارهٔ همان `(connection,eventId)`: پاسخ duplicate و بدون job/inbox دوم.
- event ناشناخته: inbox ثبت شود اما job business ساخته نشود و رویداد برای replay باقی بماند.
- درخواست توکن: فقط request id/status برگردد و هیچ access/refresh token در response، log یا dashboard دیده نشود.
- استعلام وضعیت: فقط token status و connection status در scope همان shop/tenant برگردد.
- revoke: توکن غیرفعال و connection غیرفعال شود؛ دسترسی shop دیگر رد شود.
- worker: jobهای Product/Inventory به adapter و jobهای Counterparty/Sale/Purchase به `Hyperyek.Accounting.Api` تحویل داده شوند.

# برنامه تست Integration هایپریک و باسلام

## 1. تست معماری و مرز دامین

## مسیر عمودی فعلی برای مرور و تست واقعی

### A) باسلام → حسابداری

`POST /api/integrations/v1/webhooks/basalam/{connectionKey}` با `X-Event-Type: order.vendor.created`
در `IntegrationWebhookInbox` و `IntegrationScenarioJobs` ثبت می‌شود. Worker، payload را به
`IIntegrationBusinessCommandPort` و در محیط واقعی به `Hyperyek.Accounting.Api` می‌فرستد.
پاسخ حسابداری باید `Applied` یا `Duplicate` باشد؛ خطای موقت retry و خطای قطعی dead-letter می‌شود.

### B) حسابداری → باسلام

`POST /api/integrations/v1/accounting/events/inventory-changed` فقط یک رویداد قراردادی می‌گیرد،
mapping و scope را بررسی می‌کند و پیام `IntegrationOutbox` می‌سازد. Worker آن را با adapter باسلام
به `PublishInventoryAsync` تحویل می‌دهد. تکرار همان `SourceVersion` اثر دوم ایجاد نمی‌کند و نسخه قدیمی رد می‌شود.

تست واقعی این دو مسیر باید با SQL Server و یک fixture HTTP برای باسلام انجام شود؛ حسابداری واقعی
نباید با SQL مستقیم از Integration نوشته شود.

- بررسی dependency graph پروژه‌های Domain، Application، Infrastructure، API و Worker.
- رد هر reference مستقیم Integration به entity، repository یا DbContext دامین کسب‌وکار.
- اطمینان از اینکه پنل فقط Contract/API را مصرف می‌کند.
- بررسی اینکه migrationهای Integration فقط جداول owned-by-Integration را هدف می‌گیرند.
- بررسی no-op بودن migrationهای تاریخی platform برای token و customer mapping؛ ایجاد schema فقط با `ensure-integration-database.sql` یا migration اختصاصی Integration مجاز است.
- بررسی اینکه `HyperSqlServerContext` هیچ DbSet یا مدل فعالی برای IntegrationCustomerMapping، token، webhook، mapping و outbox ندارد و همه‌ی آن‌ها از `HyperIntegrationContext` خوانده می‌شوند.

### آزمون مستقل دامین

این آزمون بدون SQL Server و بدون build وابستگی‌های کامل پنل/Neo-Bpms اجرا می‌شود:

```powershell
dotnet build src/Integration/Hyper.Integration.Domain/Hyper.Integration.Domain.csproj --no-restore
dotnet restore tools/IntegrationDomainChecks/IntegrationDomainChecks.csproj
dotnet build tools/IntegrationDomainChecks/IntegrationDomainChecks.csproj --no-restore
dotnet run --project tools/IntegrationDomainChecks/IntegrationDomainChecks.csproj --no-build
dotnet run --project tools/IntegrationSchemaProvisioner/IntegrationSchemaProvisioner.csproj -- --settings src/AdminPanel/Hyper.AdminPanel.Web/appsettings.json --script docs/schema/ensure-integration-database.sql
```

اگر اجرای ابزار را به‌صورت دستی انجام می‌دهید، در SSMS ابتدا روی `master` این دستور را اجرا کنید و سپس context را روی `HyperyekIntegration` بگذارید و کل فایل `docs/schema/ensure-integration-database.sql` را اجرا کنید:

```sql
IF DB_ID(N'HyperyekIntegration') IS NULL
    CREATE DATABASE [HyperyekIntegration];
```

برای تأیید ساختار:

```sql
SELECT COUNT(*) AS IntegrationTableCount
FROM sys.tables
WHERE schema_id = SCHEMA_ID(N'dbo')
  AND name IN
  ('ExternalIntegrationConnections','ExternalOAuthTokens','IntegrationCustomerMappings',
   'IntegrationScenarioJobs','ExternalProductMappings','ExternalOrderMappings',
   'IntegrationSyncRuns','IntegrationWebhookInbox','IntegrationOutbox',
   'IntegrationMerchantAccess','IntegrationAdminSimulations','IntegrationTokenRequests',
   'IntegrationEventAudits','InventoryReservationLogs');
```

مقدار مورد انتظار `14` است. اسکریپت schema داده‌ای درج یا حذف نمی‌کند.

باید ruleهای scope و event، تشخیص اختلاف کالا و موجودی، حذف از هر دو سمت، mapping تکراری، floor موجودی، backoff retry و workflow فروش را سبز کند.

## 2. تست قرارداد API

### Webhook

- scopeهای Basalam برای مسیر اصلی باید شامل `vendor.profile.read`، `vendor.parcel.read` و `vendor.product.read` باشند؛ اولی برای تشخیص غرفه و دومی/سومی برای `VENDOR_NEW_ORDER`، `VENDOR_PARCEL_CHANGES` و `PRODUCT_CREATE_CHANGES` لازم‌اند.
- `Basalam:WebhookAuthorization` باید در Secret Store یا environment تنظیم شود؛ مقدار خالی عمداً ثبت وب‌هوک را متوقف می‌کند.

- payload معتبر و signature معتبر: پاسخ `202 Accepted` و ایجاد یک inbox و audit.
- ارسال دوباره همان `(Connection, EventId)`: پاسخ duplicate و بدون رکورد دوم.
- بدنه خالی، JSON خراب، event header نامعتبر و payload بزرگ: رد با `400`.
- signature خراب، timestamp خارج از tolerance و connection غیرفعال: رد و ثبت نتیجه audit.
- provider ناشناخته یا protocol پشتیبانی‌نشده: پاسخ `422`.
- برای باسلام، تا زمان تأیید رسمی signature/payload از مستند provider، تست باید unsupported را بپذیرد و نباید پروتکل HMAC حدسی تولید کند.
- دو درخواست همزمان با event یکسان: فقط یک اثر پایدار.

### مدیریت اتصال و token

- ثبت اتصال با tenant/shop معتبر.
- عدم بازگشت credential یا token در response، log و telemetry.
- refresh موفق، token منقضی، refresh ناموفق و revocation.
- جلوگیری از دسترسی یک shop به اتصال shop دیگر.

### Mapping و sync

- ایجاد mapping یکتا برای product/variant.
- تشخیص mapping تکراری و mapping متعلق به اتصال دیگر.
- sync کامل، incremental، cancel، retry و dead-letter.
- provider adapter ناشناخته یا credential ناسازگار.
- API مدیریت اتصال: create بدون credential، list بدون credential، enable/disable با tenant/shop mismatch و duplicate connection.
- API mapping: create/list/deactivate با connection scope، duplicate unique key و mapping متعلق به shop دیگر.
- API replay/sync: replay فقط برای inbox همان connection و scope؛ sync برای connection متعلق به shop/tenant درخواست‌کننده.
- authorization: درخواست بدون احراز هویت رد شود و gateway/claims نتواند با header آزاد به shop دیگر دسترسی بگیرد.
- scope authorizer: فقط claim دقیق `integration_scope=shop:{ShopId};tenant:{TenantId}` مجاز است؛ headerهای `X-Shop-Id` و `X-Tenant-Id` صرفاً داده‌ی درخواست‌اند.
- dashboard API: خروجی فقط scope همان shop/tenant باشد و هیچ credential/token در summary یا run برنگردد.

## 3. تست worker و رویداد

- پردازش inbox با lease و recovery پس از انقضای lease.
- ordering بر اساس provider event sequence/OccurredAt.
- retry با backoff، حداکثر تلاش و انتقال به dead-letter.
- replay دستی با correlation جدید و idempotency حفظ‌شده.
- جلوگیری از loop با source/correlation/causation metadata.

## 4. تست سناریوهای کسب‌وکاری

- subscription: ایجاد، تمدید، لغو و تعلیق sync.
- catalog: ایجاد/ویرایش محصول، variation، SKU/barcode و اختلاف قیمت.
- inventory: snapshot معتبر، رزرو، release و جلوگیری از overselling.
- order: فروش غرفه، خرید مشتری، parcel، لغو و مرجوعی.
- review و chat: ثبت، پاسخ، rate limit و retry.
- accounting: نگاشت به قرارداد Hyperyek فقط از طریق port/API و با transaction.
- customer mapping: ثبت mapping در دیتابیس Integration با `ExternalCustomerId` و `PersonId` scalar؛ عدم ایجاد FK یا جدول mapping در دیتابیس Hyperyek.
- customer port: اجرای `IntegrationCustomerRegistration` با fake برای `IIntegrationAccountingPort`، بدون load شدن `HyperSqlServerContext` در Domain/Application.
- shop port: اجرای ownership و simulation با fake برای `IIntegrationPlatformShopPort` و آزمون حفظ tenant/shop isolation.

## 5. تست پنل و شبیه‌سازی

- داشبورد tenant-scoped برای اتصال، mapping، run، inbox، retry و خطا.
- simulation بدون ایجاد مجوز دائمی یا token واقعی.
- simulation اجرایی با همان pipeline واقعی و امکان مشاهده/replay.
- عدم دسترسی مستقیم view/controller به Integration DbContext.

## 6. تست‌های نهایی انتشار

```powershell
pwsh Backend/tools/IntegrationArchitectureChecks.ps1
dotnet build Backend/src/Integration/Hyper.Integration.Contracts/Hyper.Integration.Contracts.csproj --no-restore
dotnet build Backend/src/Integration/Hyper.Integration.Infrastructure/Hyper.Integration.Infrastructure.csproj --no-restore
dotnet build Backend/src/Core/Hyper.Infrastructure/Hyper.Infrastructure.csproj -p:ArtifactsPath=Backend/.artifacts --no-restore
dotnet run --project Backend/tools/ScenarioChecks/ScenarioChecks.csproj --no-restore
dotnet test Backend/Hyper.Backend.sln --no-restore --filter Category=Integration
```

در محیطی که دیتابیس تست در دسترس است، اجرای SQL scenario با transaction rollback انجام شود و هیچ provider واقعی در تست خودکار فراخوانی نشود؛ provider contract test فقط با fixture و fake HTTP اجرا شود.

## معیار قبولی

انتشار زمانی مجاز است که build لایه‌های جدید موفق، تست مرز دامین موفق، تست idempotency و tenant isolation موفق، تست worker و adapterها موفق و تمام موارد امنیتی بدون credential leakage پاس شده باشند.
