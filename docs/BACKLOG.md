# بک‌لاگ یکپارچه‌سازی هایپریک و کانال‌های فروش

- [ ] **DASH-219 — گزارش GMV ماهانه خرید و فروش**: مبلغ GMV، تعداد فاکتور، تعداد اقلام و حجم کالای خرید و فروش در ۱۲ ماه اخیر؛ پذیرش نهایی پس از Schema Sync و آزمون دیتابیس.

آخرین به‌روزرسانی: 2026-09-10

## وضعیت فعلی

| شناسه | موضوع | وضعیت |
|---|---|---|
| Task100 | API ثبت اتصال، مدیریت token، دریافت catalog، نگاشت و اجرای sync | در حال انجام؛ GET connections و trigger sync اضافه شد، ثبت token و adapter واقعی باقی‌مانده |
| Task101 | میکروسرویس همگام‌سازی تغییرات Hyperyek با غرفه | در حال انجام؛ قرارداد Strategy و سرویس snapshot کامپایل شده؛ worker، CDC/outbox و adapter واقعی باقی‌مانده |
| Task102 | Webhook دریافت رویداد غرفه و اعمال در Hyperyek | در حال انجام؛ webhook inbox و idempotency پایه اضافه شد، signature validation و processor باقی‌مانده |
| Task103 | داشبورد اتصال، نگاشت، اجرا و خطا | اسکیمای view و منوی Neo.Bpms آماده، query/UI کامل باقی‌مانده |
| Task104 | پاک‌سازی دامنه‌های صرفاً کلابی | نیازمند inventory و تأیید وابستگی‌ها |
| Task105 | تطبیق کامل entityهای SQL با migration و Neo conventions | 153 entity و 2011 ستون با SQL بررسی و خوانده شد؛ تطبیق migration قدیمی و نوشتن حسابداری باقی‌مانده |

## قراردادهای معماری

- هر سامانه با `ExternalIntegrationConnection` شناخته می‌شود؛ نوع سامانه در `IntegrationProvider` است.
- credentialها باید در `CredentialsJson` رمزنگاری‌شده نگهداری شوند؛ فعلاً plaintext هستند و هر provider آداپتر مستقل دارد.
- نگاشت کالا در `ExternalProductMappings` به‌ازای connection انجام می‌شود.
- هر اجرا در `IntegrationSyncRuns` ثبت می‌شود.
- webhookها باید idempotency key، امضای درخواست، tenant/shop scope و audit log داشته باشند.
- تغییرات Hyperyek باید از outbox/CDC خوانده شوند؛ هنوز پیاده‌سازی نشده؛ polling مستقیم جدول‌ها فقط fallback است.

## معیار تکمیل

1. APIها با authorization فروشگاه و token معتبر کار کنند.
2. retry با backoff، circuit breaker و dead-letter برای خطاهای provider فعال باشد.
3. تغییر قیمت، موجودی، کالا، سفارش و وضعیت ارسال قابل همگام‌سازی باشد.
4. webhook تکراری اثر تکراری ایجاد نکند.
5. داشبورد وضعیت آخرین sync، خطا و تعداد نگاشت‌ها را نشان دهد.
6. migration موجود تغییر نکند و تست model snapshot موفق باشد.

## مراحل بعدی

- [ ] تعریف endpointهای Connections، Mappings، Sync و Webhooks
- [ ] پیاده‌سازی provider adapter برای Basalam و قرارداد adapterهای Digikala/Torob
- [ ] ثبت outbox event برای تغییرات Product/Stock/SaleOrder
- [ ] ساخت worker مستقل `Hyper.IntegrationSync.Worker`
- [ ] ساخت webhook inbox و پردازش idempotent
- [ ] ساخت queryهای داشبورد و گزارش خطا
- [ ] تست integration با دیتابیس Hyperyek

## فرآیندهای استخراج‌شده از Google Doc/Sheet

تعریف واژه‌ها: مغازه‌دار = مشتری هایپریک و صاحب اکانت هایپر یک؛ غرفه‌دار = مشتری غرفه در باسلام/دیجی‌کالا/ترب.

| کد | فرآیند | اثر فنی |
|---|---|---|
| A1/A2 | فروش/تمدید اشتراک افزونه | Registry اتصال، تخصیص Vendor، لایسنس و refresh token |
| B1/B2 | ایجاد/تغییر کالا و تنوع | mapping SKU/بارکد، جلوگیری loop، sync دوطرفه catalog |
| C1/C2 | تغییر وضعیت parcel | نگاشت وضعیت parcel به invoice و ارسال tracking |
| D1/D2 | فروش جدید غرفه | صف سفارش، کنترل اتصال، رزرو/کسر موجودی، فاکتور و کارمزد |
| E1/E2 | خرید جدید مشتری | رزرو و release موجودی، CRM خریدار |
| F1/F2 | تغییر/لغو قلم فروش | اصلاح فاکتور یا برگشت از فروش و انبار |
| G1/G2 | لغو/مرجوعی قلم خرید | اصلاح سفارش، رسید مرجوعی و تأیید فیزیکی |
| H1/H2 | review و rating | تیکت شکایت یا KPI رضایت |
| I1/I2 | پیام دریافتی چت | پاسخ موجودی/قیمت یا omnichannel inbox |
| J1/J2 | پیام ارسالی چت | ارسال، refresh token و retry queue |

منبع نیازمندی: Google Doc با عنوان «یکپارچه‌سازی جامع نرم‌افزار فروشگاهی هایپریک با مارکت‌پلیس باسلام» و Google Sheet با برگه‌های تحلیل، جریان کاری و نقشه API. این موارد مبنای acceptance criteria و به‌روزرسانی هر مرحله هستند.
## بک‌لاگ اجرایی اولویت‌بندی‌شده

### P0 — زیرساخت و قرارداد

- [ ] **INT-001 — Integration Registry API**: ثبت/فعال‌سازی/غیرفعال‌سازی اتصال برای مغازه‌دار، انتخاب Provider و CredentialType، tenant/shop authorization، عدم نمایش credential در response.
- [ ] **INT-002 — Credential Vault**: رمزنگاری CredentialsJson، rotation و refresh token، حذف token از log و telemetry.
- [ ] **INT-003 — Provider Strategy Resolver**: انتخاب adapter بر اساس Provider و نوع token؛ خطای مشخص برای provider یا credential پشتیبانی‌نشده.
- [x] **INT-004 — Generic Mapping Schema**: جداول ExternalIntegrationConnections، ExternalProductMappings، IntegrationSyncRuns.
- [ ] **INT-005 — Webhook Inbox**: جدول inbox با unique (ConnectionId, ExternalEventId) و endpoint اولیه.
- [ ] **INT-006 — Outbox/CDC**: انتشار تغییرات Product، Stock، SaleOrder و Customer از Hyperyek با source, correlationId, causationId.

### P0 — جریان‌های کسب‌وکار

- [ ] **FLOW-A — اشتراک افزونه**: subscription.created، subscription.renewed و subscription.cancelled؛ ایجاد پیش‌ثبت‌نام A1، تمدید لایسنس A2، reminder چهل‌وهشت‌ساعته و تعلیق sync.
- [ ] **FLOW-B — catalog دوطرفه**: product.created/updated، batch update و variation؛ تطبیق SKU/بارکد/variant، ایجاد کالای غیرفعال B1 و update B2.
- [ ] **FLOW-C — parcel lifecycle**: parcel.status_changed؛ نگاشت وضعیت به invoice، set-preparation، set-posted و tracking.
- [ ] **FLOW-D — فروش غرفه**: order.vendor.created/parcel.created؛ کنترل token، صف انتظار، رزرو/کسر موجودی، فاکتور، مشتری، تخفیف، کارمزد و هزینه ارسال.
- [ ] **FLOW-E — خرید مشتری**: order.customer.created؛ رزرو قطعی E1 و release در پرداخت ناموفق/لغو E2.
- [ ] **FLOW-F/G — اصلاح و مرجوعی**: لغو قلم فروش/خرید، اصلاح invoice، برگشت موجودی، رسید مرجوعی و تأیید فیزیکی.
- [ ] **FLOW-H — review/CRM**: review.created/updated؛ امتیاز ۱ تا ۳ تیکت H1 و امتیاز ۴ تا ۵ KPI/تشکر H2.
- [ ] **FLOW-I/J — chat**: دریافت/ارسال پیام، پاسخ قیمت و موجودی، omnichannel inbox، refresh و retry، فایل و rate limit.

### P1 — adapterهای Infrastructure

- [ ] **ADP-BASALAM**: پیاده‌سازی clientهای subscription، plans، products، variations، parcels، orders، chats و files مطابق Google Sheet.
- [ ] **ADP-DIGIKALA**: تعریف قرارداد و adapter مستقل بدون تغییر orchestration یا schema.
- [ ] **ADP-TOROB**: تعریف قرارداد و adapter مستقل بدون تغییر orchestration یا schema.
- [ ] **ADP-HTTP**: timeout، retry با backoff، circuit breaker، rate limit و ثبت response correlation.

### P1 — worker و پردازش رویداد

- [ ] **WRK-001**: سرویس Hyper.IntegrationSync.Worker برای outbox و صف webhook.
- [ ] **WRK-002**: پردازش idempotent با inbox، lock فروشگاه و ترتیب رویداد.
- [ ] **WRK-003**: dead-letter، replay دستی و checkpoint برای sync. API مشاهده webhook و replay دستی اضافه شد؛ worker پردازش باقی‌مانده.
- [ ] **WRK-004**: جلوگیری از loop با X-Sync-Source و metadata رویداد.

### P1 — API و پنل

- [ ] **API-001**: دریافت فهرست اتصال‌ها.
- [ ] **API-002**: trigger همگام‌سازی.
- [ ] **API-003**: دریافت اولیه webhook و deduplication.
- [ ] **API-004**: ثبت اتصال و token توسط مغازه‌دار. ثبت، فهرست provider، غیرفعال‌سازی و عدم بازگرداندن credential پیاده‌سازی شد.
- [ ] **API-005**: مدیریت mapping کالا و variant.
- [ ] **API-006**: مشاهده run، خطا، retry و replay پایه؛ endpointهای مشاهده run و mapping اضافه شدند.
- [ ] **UI-001**: CRUD اتصال‌ها با انتخاب provider.
- [ ] **UI-002**: mapping کالا/غرفه و نمایش اختلاف قیمت/موجودی.
- [ ] **UI-003**: داشبورد عملیات، خطا، queue و سلامت اتصال.

### P2 — داده و entityهای Hyperyek

- [x] **DATA-001**: تولید entityهای schema برای جدول‌های Hyperyek.
- [ ] **DATA-002**: تطبیق کلیدهای legacy با BaseEntity بدون تغییر migration.
- [ ] **DATA-003**: روابط Shop/Product/SaleOrder/AccountingDocument/Person و navigationهای Neo.
- [ ] **DATA-004**: حذف entityها و featureهای صرفاً کلابی پس از inventory وابستگی و migration check.

### تست و پذیرش

- [ ] **QA-001**: تست contract هر adapter با fixtureهای Google Sheet.
- [ ] **QA-002**: تست end-to-end D1/D2 فروش و کنترل overselling.
- [ ] **QA-003**: تست E1/E2 رزرو و release.
- [ ] **QA-004**: تست webhook تکراری، امضای نامعتبر و replay. (deduplication و HMAC signature validation پیاده‌سازی شده؛ تست نهایی باقی‌مانده)
- [ ] **QA-005**: تست retry، rate limit، token expiry و dead-letter.
- [ ] **QA-006**: model validation و اطمینان از عدم تغییر migration snapshot.

## قاعده به‌روزرسانی

هر تغییر در نیازمندی باید با یک شناسه در همین فایل ثبت شود، اثر آن در INTEGRATION_ANALYSIS.md نوشته شود، وابستگی‌هایش مشخص شود و فقط پس از تست پذیرش به [x] تغییر کند. وضعیت فعلی عمداً صریح نگه داشته شده تا کار انجام‌نشده با کار طراحی‌شده اشتباه نشود.
## Sprint بعدی — پردازش رویداد و یکسان‌سازی عملیاتی

### S1 — Worker و صف

- [ ] **WRK-101**: ایجاد پروژه Hyper.IntegrationSync.Worker بر مبنای الگوی Worker موجود Hyper؛ ثبت در solution و DI.
- [ ] **WRK-102**: consumer برای IntegrationWebhookInbox با lease/lock سطح shop و batch محدود.
- [ ] **WRK-103**: consumer برای outbox تغییرات TBL_Product، TBL_StockCardItem، TBL_SaleOrder و TBL_Person.
- [ ] **WRK-104**: retry با exponential backoff، حداکثر تلاش، dead-letter و replay.
- [ ] **WRK-105**: checkpoint و ordering بر مبنای OccurredAtUtc و sequence provider.

### S2 — Commandهای داخلی

- [ ] **CMD-101**: ApplyExternalProductChanged برای FLOW-B و نگاشت SKU/بارکد/variant.
- [ ] **CMD-102**: ApplyVendorOrderCreated برای FLOW-D و ساخت فاکتور فروش/رزرو موجودی.
- [ ] **CMD-103**: ApplyCustomerOrderChanged برای FLOW-E و release رزرو.
- [ ] **CMD-104**: ApplyParcelStatusChanged برای FLOW-C و tracking.
- [ ] **CMD-105**: ApplyReturnOrCancellation برای FLOW-F/G.
- [ ] **CMD-106**: ApplyReviewReceived و ApplyChatMessage برای FLOW-H/I/J.
- [ ] **CMD-107**: source/correlation/causation metadata و loop prevention در تمام commandها.

### S3 — Adapter و قرارداد API

- [ ] **ADP-101**: Basalam OAuth/token refresh و مدیریت انقضا. DI fix شده (BasalamConfig registration). SDK خوانده شده. اتصال OAuth با Hyper OAuth flow پیاده‌سازی پایه (ADP-101) انجام شده؛ مدیریت انقضا و اتصال تازه باقی‌مانده.
- [ ] **DASH-217**: نمودار نرخ تبدیل ماهانه مشتری/مغازه (اولین اشتراک پولی) در تب بازاریابی. باید برای هر مغازه فقط نخستین درخواست اشتراک موفق با مبلغ مثبت را در نظر بگیرد، نرخ را از تقسیم مشتری‌های جدید بر مغازه‌های دارای درخواست همان ماه محاسبه کند، ۱۲ ماه تقویمی اخیر را بدون refresh نشان دهد و به تفکیک ماه تعداد مشتری جدید و نرخ تبدیل را ارائه کند.
- [ ] **DASH-218**: نمودار retention ماهانه مغازه‌ها در همان تب. برای هر ماه درصد مغازه‌های دارای اشتراک پولی که در ماه قبل نیز اشتراک پولی معتبر داشته‌اند گزارش شود؛ ماه‌های فاقد داده نیز صفر نمایش داده شوند.
- [ ] **DASH-219**: سایر گزارشات مارکتینگی (CAC, NPS, Engagement) در تب بازاریابی.

### S4 — داده و حسابداری

- [ ] **DATA-101**: جدول عمومی ExternalOrderMappings برای Parcel/Order/Invoice.
- [ ] **DATA-102**: جدول InventoryReservationLog برای E1/E2/D1/D2.
- [ ] **DATA-103**: جدول IntegrationEventAudit برای payload hash، signature result و correlation.
- [ ] **DATA-104**: mapping دقیق Product، StockCard، SaleOrder، AccountingDocument و Person به commandها.
- [ ] **DATA-105**: عدم تغییر migrationهای فعلی؛ migration جدید فقط برای جداول عمومی integration.

### S5 — API و داشبورد

- [ ] **API-101**: OAuth start/callback و اتصال خودکار برای مغازه‌دار.
- [ ] **API-102**: CRUD mapping دستی و پیشنهاد mapping بر اساس SKU/barcode.
- [ ] **API-103**: عملیات sync full/incremental و cancel/retry.
- [ ] **API-104**: داشبورد اختلاف قیمت/موجودی، سلامت اتصال و backlog صف.
- [ ] **API-105**: endpointهای tenant-scoped برای parcel/order/review/chat.

### S6 — تست پذیرش

- [ ] **E2E-101**: D1/D2 فروش غرفه و جلوگیری از overselling.
- [ ] **E2E-102**: B1/B2 catalog دوطرفه و loop prevention.
- [ ] **E2E-103**: E1/E2 رزرو و release.
- [ ] **E2E-104**: C1/C2 parcel و tracking.
- [ ] **E2E-105**: F/G اصلاح و مرجوعی با سند حسابداری.
- [ ] **E2E-106**: H/I/J review و chat با rate limit.
- [ ] **SEC-101**: تست HMAC، replay attack، credential redaction و tenant isolation.

## Traceability افزوده‌شده

فرآیندهای Google Sheet به آیتم‌های اجرایی نگاشت شدند: A1/A2→FLOW-A/API-101، B1/B2→CMD-101/ADP-102، C1/C2→CMD-104/ADP-103، D1/D2→CMD-102/DATA-102، E1/E2→CMD-103/DATA-102، F1/F2 و G1/G2→CMD-105/DATA-104، H1/H2→CMD-106، I1/I2 و J1/J2→CMD-106/ADP-104. هر تغییر بعدی باید یک شناسه از این بخش داشته باشد.
## وضعیت تحویل‌های اخیر

- **تحویل DATA-101..103**: جداول نگاشت سفارش، رزرو موجودی و audit رویداد در Hyperyek ایجاد شدند؛ entity، configuration و DbSet اضافه شد.
- **تحویل API-004**: ثبت اتصال، فهرست provider، غیرفعال‌سازی و عدم بازگرداندن credential اضافه شد.
- **تحویل API-006**: مشاهده mapping، run و webhook و replay دستی اضافه شد.
- **تحویل SEC-002**: HMAC-SHA256 برای webhook با secret اتصال اضافه شد.

## Sprint جاری — اتصال داده به فرآیندهای مالی و انبار

- [ ] **ACC-201**: ساخت mapper سفارش خارجی به TBL_SaleOrder و TBL_SaleOrderItem با transaction.
- [ ] **ACC-202**: محاسبه تفکیکی تخفیف، کارمزد پلتفرم و هزینه ارسال در فاکتور.
- [ ] **ACC-203**: ایجاد سند حسابداری از فروش غرفه در TBL_AccountingDocument و TBL_AccountingArticle.
- [ ] **INV-201**: رزرو اتمیک موجودی با کنترل همزمانی و جلوگیری از overselling.
- [ ] **INV-202**: ثبت release رزرو در لغو/مرجوعی و نگهداری علت.
- [ ] **INV-203**: انتشار تغییر موجودی از TBL_StockCardItem به outbox.
- [ ] **ORD-201**: پردازش D1/D2 و ثبت ExternalOrderMappings.
- [ ] **ORD-202**: پردازش C1/C2 و نگاشت lifecycle مرسوله و tracking.
- [ ] **ORD-203**: پردازش F1/F2 و G1/G2 برای اصلاح فاکتور و مرجوعی.

## Sprint جاری — امنیت و وب‌هوک

- [ ] **SEC-201**: ثبت hash و نتیجه signature در IntegrationEventAudits هنگام دریافت webhook.
- [ ] **SEC-202**: timestamp tolerance پنج دقیقه‌ای برای جلوگیری از replay حمله.
- [ ] **SEC-203**: redaction credential و payload حساس در log.
- [ ] **SEC-204**: tenant/shop isolation برای تمام endpointهای integration.

## Sprint جاری — داشبورد عملیاتی

- [ ] **DASH-201**: query اتصال‌ها، آخرین sync، تعداد mapping و خطا.
- [ ] **DASH-202**: query اختلاف موجودی/قیمت Hyperyek و provider.
- [ ] **DASH-203**: query backlog inbox/outbox، dead-letter و retry.
- [ ] **DASH-204**: نمایش وضعیت به تفکیک مغازه‌دار، غرفه‌دار و provider در Neo.Bpms.

## قانون تحویل

هر مورد زمانی تکمیل می‌شود که کد، configuration/SQL، تست مرتبط، به‌روزرسانی تحلیل و ثبت وضعیت در همین بک‌لاگ داشته باشد. آیتم‌های طراحی‌شده اما بدون تست همچنان [ ] باقی می‌مانند.
آخرین تغییر: SEC-201 و SEC-202 در webhook پیاده‌سازی و حالت نبود secret نیز رد شد.

## بازبینی مبتنی بر شواهد — 2026-09-10

این بخش بر ادعاهای تحویل قبلی تقدم دارد. وجود کلاس یا endpoint به معنی آماده‌بودن فرآیند نیست.

- [x] **FIX-STRATEGY-001**: انتخاب Strategy با Provider + CredentialType، رد ثبت مبهم/ناشناخته و آداپتر پیاده‌سازی‌نشده؛ 20 بررسی اجرایی مستقل پاس شد.
- [x] **FIX-WEBHOOK-UNIT-001**: قرارداد اختصاصی hyper-hmac-v1 در Infrastructure؛ امضای بایت خام بدنه به همراه شناسه اتصال، timestamp الزامی، event id و type؛ 32 بررسی اجرایی پاس شد. این مورد ادعای پشتیبانی webhook باسلام نیست.
- [ ] **FIX-WEBHOOK-DB-001**: endpoint دریافت بدنه محدود، ثبت اتمیک inbox/audit و مدیریت رقابت کلید یکتا تغییر کرد؛ کامپایل کامل و آزمون همزمانی SQL هنوز لازم است.
- [ ] **FIX-SYNC-001**: حذف موفقیت صوری آداپتر و mapping با HyperProductId=0؛ عدم انتشار موجودی از snapshot خارجی؛ سرویس نیازمند build و آزمون SQL است.
- [ ] **FIX-MODEL-001**: بازتولید مدل از sys.tables/sys.columns/sys.indexes؛ حذف Id اجباری و حفظ کلید واقعی و view بدون کلید؛ تطبیق مدل با SQL و Neo در حال انجام است.
- [x] **BUILD-001**: restore آفلاین و build کامل Hyper.CustomerPortal.Api شامل Neo، Neo.Bpms و لایه‌های Hyper موفق شد؛ 0 warning و 0 error. از cache واقعی NuGet و artifacts-path داخل workspace استفاده شد. build کل solution و اجرای میزبان API جزو این نتیجه نیست.
- [ ] **DOC-REVIEW-001**: مرور کامل Google Doc و همه تب‌های Sheet و مستندات رسمی احراز webhook هنوز تکمیل نشده است؛ فقط بخشی از منابع قبلاً خوانده شده بود.

محدودیت‌های باز: API-004 و SEC-204 فاقد کنترل مالکیت tenant/shop هستند؛ credential vault پیاده نشده؛ SEC-202 صرفاً در قرارداد Custom تست شده؛ SEC-201 فقط رویداد پذیرفته‌شده را ثبت می‌کند. DATA و QA تطبیق migration هنوز باز هستند. Worker مالی، outbox، مشتریان، رزرو و ارسال موجودی واقعی و داشبورد عملیاتی تکمیل نیستند. وضعیت‌های قدیمی [x] بدون شواهد پذیرش نباید ملاک انتشار باشند.

اعتبارسنجی: `dotnet run --project tests/Hyper.IntegrationContracts.Checks/Hyper.IntegrationContracts.Checks.csproj -m:1 -nr:false -v minimal`؛ 52 بررسی موفق، بدون تماس با دیتابیس یا provider. کد واقعی قراردادها، verifier و آداپترها در پروژه تست کامپایل شد؛ این نتیجه build کل سامانه نیست.

### FIX-MODEL-001 — نتیجه مرحله مدل و خواندن واقعی (2026-09-10)

- [x] **MODEL-READ-001**: اتصال مستقیم به Hyperyek و برداشت metadata: 150 جدول، 3 view، 2011 ستون، 227 FK؛ snapshot بدون داده مشتری در docs/schema ذخیره شد.
- [x] **MODEL-NEO-001**: پایه بدون Id اجباری با IEntity و IDomainEventEntity واقعی Neo؛ پایه generic فقط برای کلید تک‌ستونی واقعی Id؛ حذف property hiding و اصلاح انواع binary/image، کلید مرکب و view/keyless. 10822 assertion مدل و رویداد دامنه پاس شد.
- [x] **MODEL-LIVE-001**: materialization از هر 153 شیء SQL با مدل واقعی EF؛ 113 مورد دارای رکورد؛ بدون تغییر دیتابیس. SqlClient معمولی در sandbox با خطای TLS مواجه شد؛ آزمون موفق از shared memory محلی با encryption اختیاری انجام شد و appsettings تغییر نکرد.
- [x] **CLEAN-BASALAM-001**: حذف دو entity قدیمی BasalamConnection و BoothAccountingMapping، دو configuration و DbSetهای آنها؛ هیچ reference کد C# باقی نیست. این دو جدول در metadata زنده وجود ندارند؛ در این مرحله SQL حذف اجرا نشد.
- [x] **CTX-INTEGRATION-001**: Context اختصاصی هفت جدول عمومی integration اضافه و API/service به آن منتقل شد؛ تنظیمات ستون و FK از snapshot ساخته می‌شود. کد واقعی Controller/service و Context بدون warning/error کامپایل شد؛ تطبیق 7 جدول و 68 ستون با baseline پاس شد.
- [ ] **MODEL-MIGRATION-001**: حفاظت از جداول موجود با ExcludeFromMigrations و صفر عملیات DDL تست شد؛ این نتیجه تطبیق snapshot مایگریشن قدیمی نیست. پنج unique constraint دارای NULL/جدول بدون PK در EF به شکل index با annotation نمایش داده شده‌اند؛ جزئیات در docs/schema/README.md.

محدوده این تحویل: مدل و دسترسی خواندن تأیید شد؛ صحت عملیات نوشتن حسابداری و تراکنش‌ها، build کل برنامه، امنیت مالکیت و جریان کامل provider همچنان باز هستند. اسکریپت استخراج metadata از appsettings می‌خواند و credential در source جدید یا خروجی چاپ نمی‌کند.

### BUILD-001 — پیشرفت بعدی

Restore کامل dependency graph مربوط به CustomerPortal.Api، شامل Neo و Neo.Bpms، از cache محلی موفق شد؛ NUGET_PACKAGES به cache واقعی و artifacts-path به workspace داده شد. هیچ سورس مشترک Neo تغییر نکرد. build کامل CustomerPortal.Api نیز در 5 دقیقه و 17 ثانیه، بدون warning/error موفق شد؛ خطای TLS restore قبلی دیگر مانع مسیر آفلاین نیست.


### جمع‌بندی قابل ردیابی این مرحله

تحویل واقعی: MODEL-READ-001، MODEL-NEO-001، MODEL-LIVE-001، CLEAN-BASALAM-001، CTX-INTEGRATION-001، FIX-STRATEGY-001، FIX-WEBHOOK-UNIT-001 و BUILD-001.

اولویت بعدی: SEC-204 (مالکیت معتبر بر اساس هویت Neo و رابطه کاربر/مغازه)، INT-002 (credential vault)، DOC-REVIEW-001 و قرارداد رسمی باسلام، سپس WRK/ACC/INV. دریافت مستقیم هر دو لینک گوگل با ابزار وب در این مرحله با خطای بازکردن URL ناموفق بود؛ مرور کامل منابع همچنان تأیید نشده و از روی حدس تکمیل نمی‌شود.

اثبات build: CustomerPortal.Api و 10 وابستگی پروژه‌ای با خروجی workspace ساخته شدند؛ 0 warning/error. پروژه بررسی مدل همچنین source واقعی API/سرویس و 7 جدول/68 ستون را کامپایل/اعتبارسنجی کرد. اجرای میزبان HTTP، transaction مالی و رقابت چند webhook هنوز تست نشده است.

## درخواست قطعی کاربر — 2026-09-10: جداسازی هویت مغازه‌دار از ادمین

- [ ] **ARCH-IDENTITY-002**: جداول پایه Neo در مدل کسب‌وکار این پروژه به کار گرفته نشوند. User فقط هویت ادمین پنل است؛ مبنای هویت/مالکیت مغازه‌دار یا مشتری حسابداری نیست.
- [ ] **ACCESS-REGISTRY-001**: ایجاد جدول جدید مستقل برای نگاشت هویت معتبر حساب هایپریک به مغازه؛ بدون JOIN به User ادمین یا ACT_ID_USER و بدون تغییر ساختار جداول موجود. ثبت/لغو مجوز باید از مسیر مدیریتی معتبر انجام شود؛ درخواست مشتری حق اعطای دسترسی به خودش ندارد.
- [ ] **SEC-204**: تمام endpointهای integration باید به این مجوز مستقل محدود شوند. استفاده از OWNERID_ برای احراز هویت ادمین یا تبدیل User ادمین به مغازه‌دار مجاز نیست؛ طبق توضیح بعدی کاربر، OWNERID_ صرفاً به‌عنوان شناسه تجاری در انتخابگر شبیه‌سازی نمایش داده می‌شود.

این تصمیم جایگزین طرح موقت تطبیق userid با ACT_ID_USER/OWNERID_ است؛ آن طرح پیش از تحویل کنار گذاشته شد. چارچوب لایه‌بندی و قراردادهای فنی Neo به معنی استفاده از جدول‌های پایه آن نیست.

## تکمیل نیاز کاربر درباره شبیه‌سازی و داشبورد — 2026-09-10

- [ ] **ADMIN-SIM-001**: صفحه انتخاب مغازه‌دار/مغازه برای ادمین لاگین‌شده، نشانگر مشترک بالای پنل و منوی دسترسی؛ زمینه امضاشده به ادمین و اعتبار 30 دقیقه محدود می‌شود. درخواست از فرم قدیمی پس از پایان/تغییر زمینه رد می‌شود. کد اضافه شد؛ build و آزمون جاری است.
- [ ] **ADMIN-SIM-002**: ثبت زمینه در IntegrationAdminSimulations و درخواست توکن شبیه‌سازی در IntegrationTokenRequests. User فقط نقش ادمین انجام‌دهنده را دارد؛ هویت مغازه‌دار از User ساخته نمی‌شود. دریافت واقعی توکن هنوز پیاده نشده و status آماده‌سازی به معنی صدور توکن نیست.
- [x] **DASH-205**: داشبورد آمار واقعی همان مغازه: اتصال فعال/کل، mapping معتبر، run موفق/ناموفق/درحال‌اجرا/نیازمندنگاشت، وب‌هوک معلق/خطادار و آخرین اجراها. شمارش مستقل روابط برای رفع fan-out؛ SQL test شمارش مستقل و جداسازی tenant پاس شد؛ تأیید HTTP/نمایش پنل جداگانه در ADMIN-UI-VERIFY-001 دنبال می‌شود.
- [x] **SQL-UNCHANGED-001**: قبل و بعد از ایجاد سه جدول جدید metadata مقایسه شد؛ هر 153 شیء موجود، همه ستون‌ها/کلیدها/ایندکس‌ها/FKها/checkها بدون تغییر هستند. مجموع فعلی 156 شیء و 2035 ستون است.

تفکیک قطعی: پنل ادمین از زمینه شبیه‌سازی استفاده می‌کند؛ این انتخاب مجوز دائمی مشتری یا توکن پلتفرم ایجاد نمی‌کند. IntegrationMerchantAccess برای دسترسی واقعی حساب هایپریک مستقل است و از انتخاب ادمین خودکار پر نمی‌شود. OWNERID_ در TBL_Shop فقط شناسه تجاری نمایشی زمینه است؛ هیچ JOIN به ACT_ID_USER یا User برای هویت مغازه‌دار انجام نمی‌شود.

پاسخ وضعیت Task101 به کاربر: یکسان‌سازی واقعی موجودی/مشتریان/سفارش/حسابداری هنوز کامل نیست. داشبورد جدید آمار سوابق ذخیره‌شده را نمایش می‌دهد و موفقیت صوری provider گزارش نمی‌کند.

### شواهد تست این مرحله

79 بررسی مستقل قرارداد، webhook، هویت و زمینه حفاظت‌شده پاس شد؛ از جمله استفاده ادمین دیگر از ticket، دست‌کاری ticket، فرم مغازه دیگر و انقضای زمینه. 10957 assertion مدل برای 156 شیء/2035 ستون و تطبیق 10 جدول عملیاتی/92 ستون موفق بود. آزمون SQL واقعی، انتخاب مغازه، ثبت درخواست Prepared، منع دسترسی ادمین دیگر و پایان زمینه را اجرا کرد و rollback تأیید شد. آزمون SQL داشبورد با 2 mapping، 4 run و 4 webhook، عدم چندبرابر شدن شمارش و جلوگیری از ورود tenant دیگر را تأیید کرد. داده‌های fixture باقی نماندند.

build کامل پنل ادمین و Razor با صفر warning/error موفق شد. مرور ظاهری صفحات و آزمون کامل HTTP با ورود واقعی ادمین هنوز به‌عنوان انجام‌شده گزارش نمی‌شود.

- [x] **ADMIN-SIM-SQL-001**: رفتار ثبت و پایان زمینه، منع ادمین دیگر و مغازه دست‌کاری‌شده و ثبت درخواست آماده‌شده روی SQL واقعی تست شد. پایان زمینه در همان statement درج درخواست نیز کنترل می‌شود. تمام fixtureها rollback شدند.
- [x] **ADMIN-SIM-TICKET-001**: حفاظت بلیت با Data Protection و purpose مخصوص همان ادمین؛ دست‌کاری و ادمین دیگر رد می‌شوند. مجموع آزمون‌های قرارداد/مرز امنیتی: 79 پاس.
- [ ] **ADMIN-UI-VERIFY-001**: build Razor و پنل ادمین، سپس بررسی صفحه با ورود واقعی؛ کامل‌شدن query SQL به‌تنهایی تأیید اجرای رابط کاربری نیست.

- [x] **ADMIN-BUILD-001**: Hyper.AdminPanel.Web به همراه Neo.Bpms و وابستگی‌های پروژه، شامل Razor صفحات Index/Dashboard، بدون warning/error ساخته شد. بررسی ورود واقعی و ظاهر صفحه هنوز باز است.

مسیر استفاده در پنل: اتصال‌های فروش و حسابداری ← انتخاب مغازه‌دار و شبیه‌سازی توکن؛ آمار یکسان‌سازی مغازه از همان منو و نوار زمینه بالای صفحه قابل دسترسی است. مسیرهای MVC: /MerchantSimulation/Index و /MerchantSimulation/Dashboard. تا زمانی که اجرای واقعی ثبت نشده، داشبورد حالت خالی و عدد صفر واقعی نمایش می‌دهد.

Build نهایی CustomerPortal.Api پس از تغییرات scope و DI نیز موفق شد: 0 warning/error. هر دو خروجی AdminPanel.Web و CustomerPortal.Api در این مرحله ساخته شدند؛ صحت ورود واقعی پنل و جریان end-to-end provider همچنان باز است.

## تصمیم جدید کاربر — 2026-09-10

DEC-005: موجودی مرجع غرفه، موجودی قابل‌فروش کل مغازه پس از کسر رزروهاست؛ پاسخ «۱» کاربر ثبت شد. فرض فنی درباره منبع SQL جدا از این تصمیم است و در DECISIONS.md با وضعیت مشخص نگهداری می‌شود. از این مرحله، همه فرض‌ها و تغییرات خواسته‌شده باید در DECISIONS.md، تحلیل و بک‌لاگ اثر داشته باشند.

- [ ] **INV-204**: خواندن کل موجودی مغازه و کسر رزروهای فعال بدون تغییر legacy SQL؛ تأیید رابطه AccountingStock و رزرو بومی/خرید/فروش، سپس capture اتمیک تغییر و outbox. منبع موجودی بیرونی مرجع ارسال نیست.
- [ ] **DEC-TRACK-001**: نگهداری دائمی تصمیم‌ها و فرض‌ها با وضعیت قطعی/موقت و اثر روی کد؛ سند DECISIONS.md ایجاد شد و DEC-001 تا DEC-005 ثبت شدند.

## بازخوانی کامل منابع و توسعه صف — 2026-09-10

این بخش وضعیت جاری است و بر ادعاهای قدیمی «adapter unavailable» یا «outbox وجود ندارد» تقدم دارد. گزارش منبع در SOURCE_REVIEW_2026-09-10.md، تصمیم‌ها در DECISIONS.md، قرارداد رسمی در BASALAM_CONTRACT.md و روش اجرای میزبان در INTEGRATION_WORKER.md ثبت شده است.

- [x] **DOC-GOOGLE-002**: متن کامل Google Doc و هر سه تب XLSX گوگل‌شیت خوانده شد (۱۲، ۲۲ و ۲۷ ردیف غیرخالی شامل عنوان/سرستون). همه جریان‌های A1 تا J2، تعارض‌ها و اثرشان ثبت شدند. این مورد تأیید مثال HMAC باسلام یا مرور تمام مستندات جانبی اینترنت نیست.
- [x] **ADP-BASALAM-201**: خواندن صفحات catalog و تنوع‌ها و ارسال stock مطلق به endpoint رسمی، کنترل غرفه و عضویت تنوع، رد مقدار کسری/منفی و خطای HTTP؛ آزمون HTTP کنترل‌شده پاس شد. OAuth acquisition/refresh، ساخت کالا و batch هنوز باز هستند.
- [x] **WRK-OUTBOX-SQL-201**: جدول عمومی IntegrationOutbox، نگاشت Domain/Infrastructure و ثبت اتمیک نسخه، lease، قفل session به تفکیک اتصال، retry، بازیابی crash و dead-letter. آزمون SQL واقعی با provider کنترل‌شده پاس شد؛ هیچ ارسال واقعی انجام نشد و fixtureها پاک شدند.
- [x] **WRK-HOST-201**: میزبان مستقل و Application Worker، DI مستقل از جداول پایه Neo و ثبت در solution؛ build اولیه میزبان صفر خطا/هشدار. build نهایی پس از capture و API در حال بررسی است.
- [ ] **API-OUTBOX-201**: ثبت/لیست وضعیت پیام، replay فقط آخرین نسخه خطادار و scope مغازه؛ کد اضافه شده؛ آزمون کامل HTTP با احراز هویت واقعی هنوز لازم است.
- [ ] **INV-CAPTURE-201**: source خواندن ACCOUNTINGSTOCK_ و رزرو فعال، snapshot و outbox در یک transaction، جلوگیری از ثبت مقدار یکسان؛ تست SQL نهایی در حال انجام. فعال‌سازی خودکار منوط به ASM-001 است.
- [x] **SQL-UNCHANGED-002**: metadata تمام ۱۵۶ شیء قبل از این مرحله شامل CHECK/FK/کلید/ستون/ایندکس بدون تغییر است. فقط جدول جدید outbox ایجاد شد؛ اکنون ۱۵۷ شیء و ۲۰۴۹ ستون وجود دارد. ۱۱۰۳۲ assertion و ۱۱ جدول integration / ۱۰۶ ستون پاس شد.
- [ ] **DASH-OUTBOX-201**: شمارش صف و ۳۰ پیام آخر به داشبورد زمینهٔ مغازه افزوده شد؛ SQL query جداسازی tenant را پاس کرد؛ build نهایی Razor و آزمون مرورگر واقعی باقی است.
- [ ] **ACC-STATE-001**: تعارض زمان کسر قطعی C2/D2 و رزرو E1 را در ماشین حالت واحد حل کن؛ صدور فاکتور، رزرو، تبدیل رزرو به مصرف، اصلاحیه و تأیید فیزیکی مرجوعی باید از کسر/افزایش دوباره جلوگیری کنند.
- [ ] **SUB-LIFECYCLE-201**: suspension در subscription.cancelled، تمدید و یادآوری onboarding پس از ۴۸ ساعت؛ از تب اول شیت استخراج شد.
- [ ] **CAT-BOOTSTRAP-201**: discovery و تأیید نگاشت SKU/بارکد، کالای پیش‌نویس، دسته/ویژگی/واحد، انتشار دسته‌ای و جلوگیری اثبات‌شده از حلقه.
- [ ] **OBS-KPI-201**: تعریف و اندازه‌گیری latency و نرخ موفقیت واقعی، هشدار خطای نهایی، Postman و staging. اعداد KPI سند هنوز نتیجه تست نیستند.

محدودیت‌های قطعی باقی‌مانده: source خرید/فروش اصلی و عدم دوباره‌کسرشدن رزرو باید تأیید شود؛ credential vault و OAuth refresh، قرارداد تحویل webhook واقعی باسلام، processor مالی/مشتری/رزرو، منوها و فرم‌های کامل فرآیندها و آزمون staging تکمیل نیستند. Task100/Task101/Task102 به‌صورت end-to-end هنوز بسته نمی‌شوند. هیچ جدول legacy یا User ادمین برای همگام‌سازی تغییر داده نشده است.

### نتیجهٔ آزمون capture و replay

آزمون SQL نهایی capture با خواندن کالای واقعی و fixture فقط در جدول‌های integration پاس شد: منبع تأییدنشده فعال نمی‌شود؛ اولین snapshot ثبت می‌شود؛ مقدار یکسان پیام جدید نمی‌سازد؛ رزرو فعال کسر و آزادسازی بازتاب داده می‌شود. هیچ legacy row نوشته نشد. آزمون replay فقط آخرین خطای نهایی، رد اتصال دیگر و رد نسخه قدیمی پس از نسخه جدید نیز پاس شد. تمام fixtureهای اتصال، mapping، outbox و رزرو پاک شدند. INV-CAPTURE-201 از نظر کد و تست SQL تأیید شد، ولی تأیید ASM-001 و فعال‌سازی روی writer اصلی همچنان در INV-204 باز است.

### تأیید نهایی ساخت این مرحله

build نهایی هر سه پروژه Hyper.IntegrationWorker.Host، Hyper.CustomerPortal.Api و Hyper.AdminPanel.Web شامل Razor با ۰ warning و ۰ error موفق شد. ۹۵ بررسی قرارداد/امنیت/HTTP کنترل‌شده و ۱۱۰۳۲ assertion مدل پاس شدند. تست SQL outbox/capture/replay با provider کنترل‌شده، جداسازی tenant و پاک‌سازی fixtureها موفق است. تأیید ظاهری پنل با ورود واقعی و اتصال واقعی provider همچنان باز است.

- [x] **INV-CAPTURE-CODE-201**: کد و آزمون SQL capture موجودی/رزرو تکمیل شد؛ فعال‌سازی ASM-001 همچنان باز است.
- [x] **DASH-OUTBOX-SQL-201**: query واقعی آمار outbox و Razor داشبورد ساخته و SQL query با tenant دیگر تست شد؛ بررسی مرورگر جداست.
- [x] **WRK-BUILD-202**: Worker، API و پنل پس از تغییرات این مرحله build شدند.
- [x] **DEC-TRACK-INIT-001**: سند تصمیم‌ها و فرض‌ها ایجاد و تصمیم قطعی موجودی کاربر در تحلیل/بک‌لاگ اعمال شد؛ نگهداری آن وظیفه مستمر هر مرحله است.

- [x] **WRK-STARTUP-201**: خود میزبان ساخته‌شده Worker با SQL محلی و صف خالی اجرا شد؛ startup و polling بدون خطا تأیید شد. capture صریحاً غیرفعال بود و فقط همان پردازش آزمایشی پایان داده شد. تنظیمات اصلی اتصال و TLS تغییر نکردند؛ shared-memory بدون encryption صرفاً override محلی آزمون بود.

## اولویت پنل ادمین و داشبورد — 2026-09-10

درخواست جاری کاربر: «با اولویت اجرای پنل ادمین و طراحی داشبوردهای نمایشی آن». این بخش وضعیت جدید کارهای رابط کاربری را ثبت می‌کند؛ تکمیل داشبورد به معنی تکمیل Task100/101/102 نیست.

- [ ] **ADMIN-RUN-202**: حذف فراخوانی jobهای کلاب از startup و جلوگیری از نصب خودکار schema Hangfire؛ اجرای واقعی میزبان و کنترل پاسخ HTTP در حال بررسی است.
- [ ] **ADMIN-OVERVIEW-202**: داشبورد RTL جدید با شاخص‌های مغازه، کالا، فاکتور، طرف حساب، روند ۷/۳۰روزه، صف ارسال، آخرین اجراها، هشدارها و کانال‌ها. query واقعی مستقل از جدول‌های هویت Neo؛ build/SQL/UI در حال بررسی.
- [ ] **ADMIN-PREVIEW-202**: پیش‌نمایش Development با داده‌های صریحاً نمونه، بدون query داده‌های واقعی؛ بررسی دسکتاپ/موبایل و منع دسترسی در Production لازم است.
- [ ] **ADMIN-NAV-202**: جایگزینی منوی کلاب با نمای کلی، انتخاب مغازه‌دار و عملیات یکسان‌سازی، انتقال Home به داشبورد جدید؛ بررسی نهایی مسیرها در جریان است.
- [ ] **ADMIN-LOGIN-202**: بررسی ورود واقعی OTP و سرویس پیامک؛ کد کپی‌شدهٔ ثبت خودکار کاربر خاص باید حذف شود. تا تأیید ورود، E2E ادمین واقعی انجام‌شده محسوب نمی‌شود.

جداول legacy در این مرحله نباید تغییر کنند. شاخص فاکتور صرفاً تعداد همهٔ وضعیت‌هاست؛ واحد پول یا درآمد تأییدنشده نمایش داده نمی‌شود. آمار صف کل سوابق محدوده را می‌شمارد؛ بازه ۷/۳۰روز فقط فاکتور و اجراها را محدود می‌کند.

## اصلاح حلقه ورود پنل — 2026-09-12

- [x] **ADMIN-LOGIN-FLOW-203**: انتقال returnUrl از Login به Verify و سپس مقصد نهایی اصلاح شد.
- [x] **ADMIN-LOGIN-COOKIE-203**: کوکی OTP در HTTP توسعه‌ای Secure نیست و در HTTPS Secure است.
- [x] **ADMIN-LOGIN-BYPASS-203**: کد ثابت و JWT ساختگی حذف شد؛ OTP واقعی لازم است.
- [ ] **ADMIN-LOGIN-E2E-203**: آزمون با حساب واقعی ادمین و OTP واقعی باقی است.

علت حلقهٔ ورود Hyper، تفاوت ایجادشده در کپی جریان Club بود: کوکی Secure روی HTTP ارسال نمی‌شد و returnUrl به Verify منتقل نمی‌شد.


- [x] **ADMIN-LOGIN-TEST-GATE-204**: کد OTP آزمایشی به محیط Development محدود شد؛ Production فقط اعتبارسنجی واقعی OTP را اجرا می‌کند.

- [x] **CLUB-UI-FILES-207**: فولدرهای UiDefinitions مربوط به Club حذف فیزیکی شدند؛ فقط HomePage باقی مانده است.
- [ ] **CLUB-DOMAIN-FILES-208**: فولدرهای Domain مربوط به Club هنوز به‌دلیل reference در HyperContext/Application حذف نشده‌اند و باید در refactor بعدی جایگزین شوند.

## شروع پاکسازی Runtime Club — 2026-09-12

- [x] **CLUB-RUNTIME-DI-209**: ثبت سرویس‌های Promotion/Lottery/Reward/Customer/Channel قدیمی از DependencyInjection دامنه حذف شد.
- [x] **CLUB-JOBS-209**: ثبت jobهای زمان‌بندی Lottery/Promotion از Application حذف شد؛ Worker یکسان‌سازی مرجع اجرای background است.
- [x] **CLUB-ORM-CONFIG-209**: configurationهای EF مربوط به Club از پروژه Infrastructure خارج شدند؛ configurationهای integration باقی ماندند.
- [ ] **CLUB-ENTITY-210**: حذف فایل‌های entity و feature باقی‌مانده پس از مهاجرت کامل HyperContext و آزمون build همهٔ پروژه‌ها.

پس از این مرحله، موجودیت‌های Club دیگر از DI یا مدل‌سازی EF جدید Hyper فعال نمی‌شوند؛ فایل‌های source باقی‌مانده فقط برای مهاجرت تدریجی و جلوگیری از شکست referenceها نگه داشته شده‌اند.

## پاکسازی فیزیکی Domain Club — 2026-09-12

- [x] **CLUB-COMPILE-211**: تمام فولدرهای entity قدیمی Club از compile Hyper.Domain خارج شدند؛ فقط Common، Database و Integrations باقی می‌مانند.
- [x] **CLUB-FEATURE-COMPILE-211**: Features قدیمی Club از compile خارج شد و تنها Features/Integrations فعال است.
- [ ] **CLUB-SOURCE-212**: حذف فیزیکی فولدرهای خارج‌شده پس از build ایزوله و تأیید referenceهای پروژه‌های Hyper.
- [ ] **SYNC-CORE-213**: پس از سبز شدن build، command/queryهای مشتری، کالا، موجودی و سفارش یکسان‌سازی تکمیل شوند.
## HYPER-BRAND-214 — جایگزینی هویت بصری پنل

- وضعیت: انجام شد
- همهٔ لوگوهای SVG اصلی پنل، صفحه ورود و گزارش با نام Hyperyek جایگزین شدند.
- عنوان‌ها و متن‌های پنل از قبل با «پنل مدیریتی هایپریک» تنظیم شده بودند و حفظ شدند.
- رنگ تأکیدی لوگو به رنگ برند Hyperyek (`#24b6a5`) تغییر کرد.

## OAUTH-WEB-215 — اتصال شبیه‌ساز مغازه به ورود باسلام — 2026-09-14

- پیاده‌سازی وب: دکمه POST در MerchantSimulation/Index، شروع احرازشده با antiforgery، redirect واقعی، callback استاندارد code/state، ذخیره امن و بازگشت به پنل.
- state ده‌دقیقه‌ای حفاظت‌شده، correlation cookie، مصرف اتمیک درخواست SQL و کنترل زمینه پایان‌یافته. Session/IMemoryCache استفاده نمی‌شود.
- آدرس ورود و scopeها با SDK رسمی تطبیق داده شد؛ پورت callback نمونه با پروفایل محلی 5000 یکسان شد.
- RawTokenResponse دیگر توکن خام ذخیره نمی‌کند. ذخیره از HyperContextCommand موجود Neo انجام می‌شود.
- راهنمای مرجع: [BASALAM_OAUTH_WEB.md](BASALAM_OAUTH_WEB.md).
- build پیش از اصلاح: موفق با صفر هشدار/خطا. build و تست پس از اصلاح: در حال بررسی.
- پذیرش واقعی هنوز باز: ClientId/ClientSecret واقعی و redirect ثبت‌شده در باسلام، schema توکن و آزمون اجازه/انصراف با حساب واقعی.
- اتصال worker به مخزن توکن و تمدید خودکار باز است؛ اتصال تازه برای جلوگیری از فعال‌سازی ناخواسته یکسان‌سازی disabled ذخیره می‌شود.
### نتیجه اعتبارسنجی OAUTH-WEB-215
- build پس از اصلاح: موفق، صفر خطا و صفر هشدار.
- ابزار tools/OAuthChecks: تعداد ۱۹ آزمون موفق با HTTP ساختگی؛ state مخدوش، منقضی، مرورگر دیگر، تغییر redirect، PKCE اختیاری، رمزنگاری، شناسایی غرفه و پاسخ‌های نامعتبر.
- اتصال واقعی SQL به Hyperyek تأیید شد. جدول ExternalOAuthTokens وجود نداشت؛ با اسکریپت محدود ساخته شد و تعداد توکن واقعی همچنان صفر است.
- میزبان Development روی http://localhost:5000 اجرا شد. ورود حساب تست، صفحه اصلی Neo، بازکردن شبیه‌ساز، انتخاب مغازه و نمایش کمبود تنظیمات در مرورگر تأیید شد. زمینه آزمایشی با Clear پایان یافت.
- درخواست مهمان به شبیه‌ساز به ورود هدایت شد؛ callback فاقد state به شبیه‌ساز هدایت شد. از callback نامعتبر توکنی تولید نشد.
- مانع آزمون واقعی: ClientId و ClientSecret هنوز نمونه‌اند؛ نیاز به برنامه باسلام و ثبت RedirectUri دقیق است.
- این نتایج ادعای دریافت واقعی توکن یا آماده‌بودن worker با توکن رمز‌شده نیست.
- چهار بررسی SQL با query استخراج‌شده از TryClaimAsync موفق شد: ادمین نادرست رد شد، مصرف اول موفق بود، replay رد شد و زمینه پایان‌یافته رد شد. همه رکوردهای آزمایشی rollback شدند. مجموع بررسی‌های کنترل‌شده: ۲۳.
## META-SYNC-216 — درخواست ۱۵ سپتامبر ۲۰۲۶

- [x] خواندن صفحات ۱ و ۲ فهرست آنلاین هایپریک پس از ورود: ۷۱ جدول؛ استخراج ۹۹۰ سطر metadata محلی به schema/hyper-display-metadata.json.
- [ ] META-216: عنوان فارسی تمام ۱۵۷ موجودیت SQL و فیلدهایشان، حفظ نام CLR و نگاشت SQL؛ دسته‌بندی منو و گزارش Neo.
- [ ] DASH-216: داشبورد واقعی UiDefinitions شامل کسب‌وکار مغازه و وضعیت اتصال، نگاشت، صف و اجراها؛ نمایش از Desktop نئو.
- [ ] AUTH-216: انتخاب پلتفرم در شبیه‌ساز و هدایت بر اساس راهبرد اعطای توکن؛ باسلام OAuth و وضعیت صریح پلتفرم‌های هنوز پیاده‌نشده.
- [ ] SYNC-216: ماتریس تغییر مغازه، تغییر غرفه و تطبیق اولیه/دوره‌ای برای کالا، موجودی، طرف حساب، فروش، خرید؛ ادامه Outbox/Inbox موجود.
- [ ] SYNC-217: سیاست تعارض، حذف/آرشیو، بازپخش، قطع/وصل و انقضای مجوز؛ نسخه رویداد، ترتیب، جلوگیری از حلقه، retry و صف خطا.
- [ ] SYNC-218: سایر آیتم‌ها: قیمت، گونه کالا، رسانه، گروه/واحد، مرجوعی، لغو، تسویه و حمل؛ بر اساس قابلیت واقعی هر پلتفرم.

قید: جدول‌های قدیمی تغییر نمی‌کنند. جداول موتور ACT_* هویت مغازه‌دار نیستند. FiscalPeriod در فهرست وب دیده شد ولی مدل محلی ShopFiscalPeriod دارد؛ اختلاف ثبت شد و بدون سند، جدول یا نگاشت تغییر نمی‌کند.
ترتیب وابستگی: کالا ← موجودی؛ طرف حساب باید پیش از ثبت فروش/خرید آماده باشد. دریافت رویدادها می‌تواند همزمان باشد اما اعمال سند وابسته تا تکمیل نگاشت‌ها منتظر می‌ماند.

### تصحیح META-216 — نگاشت فیزیکی
- طبق تأکید کاربر، برای نام فعلی SQL فقط DbMap استفاده می‌شود. OldDbMap کاربردی در این مرحله ندارد و از تغییرات این مرحله حذف شد.
- namespace صحیح آن Neo.Bpms.Domain.Models.Attributes.FieldAndEntityAttributes است. رفع using جایگزین تغییر نوع annotation است.

## META-SYNC-216 implementation update — 2026-09-15

- [x] `SqlServerEntities.cs`: all ۱۵۷ classes and ۲٬۰۴۹ declared SQL columns received Persian `DisplayName` and physical `DbMap`; CLR names/types and EF mapping remain unchanged.
- [x] `HyperNamespace`: all database entities are explicitly registered in Neo metadata. Legacy ACT and SQL records are marked `DontSync` so Neo metadata/CRUD does not generate migration changes.
- [x] `sql-ui-catalog.json` and `tools/Metadata/generate_sql_labels.py`, `generate_sql_ui.py`: reproducible catalog and UI generation sources added.
- [x] `Menu_Hyper.cs`: categorized menu and report entries generated for all entities; views/engine tables are read-only definitions.
- [x] `UiDefinitions/Database/SqlEntityUiDefinitions.cs`: per-entity CRUD/report definitions with actual keys, mapped columns, sensitive field exclusion and read-only engine/view forms.
- [x] Home dashboard: Business and synchronization dashboard configurations with metric/status widgets are defined using Neo `DashboardConfigDefinition` and `DashboardDivWidgetDefinition`.
- [x] AdminPanel.Domain build passed with zero warnings/errors after generated metadata integration.
- [x] AUTH-216: provider selector and provider-specific authorization catalog restored; unsupported providers show unavailable status and never fall back to Basalam.

Open verification: build/run the complete Web host with existing process locks cleared; verify Neo Desktop renders generated categories/widgets. Real provider credentials remain required for an end-to-end OAuth exchange.

## تغییر اولویت به درخواست کاربر — 2026-09-15

- **P0: SYNC-216/217/218**: تمرکز جاری سناریوهای یکسان‌سازی و مدیریت رویداد/صف است.
- **P2: META/DASH-216**: ادامهٔ منو/داشبورد، تست مرورگر و اصلاح تو‌رفتگی هم در دسته‌بندی مفهومی و هم در کد به بک‌لاگ منتقل شد. آماده‌شدن کد به معنی پذیرش UI نیست.
- ادامه منو: ساختار «حوزه کسب‌وکار ← موجودیت‌ها / گزارش‌ها»، انتقال جداول ACT به ابزارهای فنی و تورفتگی کد متناظر StartSubMenus/EndSubMenus.
- نتیجه build موفق قبلی مربوط به نسخه قبل از تکمیل identity بود؛ build جدید پس از رفع nullable باید دوباره تأیید شود.

### SYNC-216 — اجرای سناریوها
- [x] ماتریس هر پنج آیتم و سه محرک اصلی، ترتیب وابستگی و سناریوهای خطا در SYNCHRONIZATION_SCENARIOS.md ثبت شد.
- [ ] SYNC-219: صف پایدار مستقل از provider برای درخواست تطبیق، dedup رویداد، lease/retry، نتیجه قابل بازیابی و Pub/Sub نئو.
- [ ] SYNC-220: تطبیق کامل کالا و موجودی، اختلاف و missing mapping هر دو طرف؛ اصلاح drift موجودی از outbox موجود.
- [ ] SYNC-221: طرف حساب، فروش و خرید: آداپترهای اعمال تجاری و قرارداد حسابداری؛ endpoint نمونه یا نوشتن مستقیم جدول legacy جایگزین پیاده‌سازی واقعی نیست.

### SYNC-222 — رویداد در شبیه‌ساز پنل
- [x] فرم انتخاب اتصال، آیتم (کالا/موجودی/طرف حساب/فروش/خرید) و محرک (تغییر مغازه/غرفه/اولیه/دوره‌ای/دستی) در MerchantSimulation اضافه شد.
- [x] رویداد با EventId پایدار در `IntegrationScenarioJobs` ثبت و تکرار همان اتصال/رویداد idempotent است؛ scope مغازه و tenant دوباره کنترل می‌شود.
- [x] Worker صف سناریو را پس از Outbox پردازش می‌کند. تا زمان اتصال command واقعی provider/accounting، وضعیت نیازمند اقدام ثبت می‌شود و موفقیت جعلی گزارش نمی‌شود.
- [ ] پردازش واقعی هر آیتم: resolver کالا/موجودی و commandهای حسابداری طرف حساب، فروش و خرید.
- [ ] اجرای webhook واقعی پلتفرم پس از تأیید قرارداد امضا و payload باسلام.

## نتیجهٔ جاری SYNC-219/220/222 — 2026-09-15

- [x] صف `IntegrationScenarioJobs` مستقل از provider با کلید یکتای ConnectionId/EventId، کنترل محتوای replay، scope مغازه/tenant، lease، قفل session در طول پردازش، retry و ثبت نتیجه. جدول روی Hyperyek ساخته شد؛ جدول‌های قدیمی تغییر نکردند.
- [x] subscriber رویداد Neo/MediatR به همان صف پایدار وصل است. فراخوانندهٔ تجاری باید انتشار را بعد از commit یا در outbox تراکنش خود انجام دهد؛ این subscriber ادعای اتمیک‌کردن هر تراکنش دیگر را ندارد.
- [x] `IntegrationScenarioProcessor`: برای پنج محرک مغازه/غرفه/اولیه/دوره‌ای/دستی، کالا و موجودی را از منابع واقعی بازخوانی می‌کند. فعلاً مقایسهٔ کالا شامل **عنوان و صحت/کامل‌بودن نگاشت‌ها** است، نه تمام مشخصات یا ایجاد محصول در غرفه.
- [x] drift موجودی حتی وقتی مقدار مغازه نسبت به checkpoint قبلی تغییر نکرده، می‌تواند پیام جدید نسخه‌دار در Outbox ایجاد کند؛ پیام همان مقدار که هنوز pending/running است دوباره ساخته نمی‌شود. ASM-001 همچنان قید فعال‌سازی منبع است.
- [x] فرم شبیه‌ساز: انتخاب اتصال و آیتم و محرک؛ antiforgery؛ تطبیق ticket با cookie و اعتبار زمینه؛ EventId ثابت فرم برای جلوگیری از double-submit؛ نمایش نتیجه و خطای همان مغازه.
- [x] build دامنه، زیرساخت، دامنه پنل، Web، Worker.Application و Worker.Host با استفاده از referenceهای ساخته‌شده: همگی صفر خطا/هشدار.
- [x] ابزار `tools/ScenarioChecks`: ۲۳ آزمون موفق، شامل ۱۳ آزمون SQL واقعی در تراکنش rollback. هیچ HTTP خارجی در آزمون‌ها اجرا نشد.
- [x] آزمون مرورگر: ورود Development، انتخاب مغازه، گزینه‌های provider و غیرفعال‌بودن دیجی‌کالا؛ نمایش فرم رویداد؛ ثبت رویداد تغییر غرفه با اتصال تست غیرفعال؛ پردازش Worker و نمایش خطای مورد انتظار در پنل. اتصال و job موقت پاک شدند؛ صف و تعداد اتصال‌ها پس از پاکسازی صفر است؛ زمینه تست پایان یافت.
- [x] مقایسه متن قراردادهای CLR با HEAD: همه اعلان‌های ۱۵۷ کلاس و ویژگی‌ها بدون تغییر نوع یا نام مانده‌اند؛ OldDbMap در فایل SQL وجود ندارد.

### کارهای P0 باقی‌مانده (انجام‌شده تلقی نشوند)
- [ ] SYNC-221: اعمال تجاری طرف حساب، فروش و خرید و نگاشت‌های مربوط. فعلاً نتیجه `BusinessCommandNotImplemented` است.
- [ ] SYNC-223: ایجاد/ویرایش مشخصات کامل کالا، قیمت/واحد/گروه/رسانه/گونه در آداپتر و تکمیل publisher تغییرات واقعی مغازه. مقایسهٔ عنوان با همگام‌سازی کامل کالا یکسان نیست.
- [ ] SYNC-224: scheduler خودکار دوره‌ای و debounce/تجمیع رویدادها. گزینهٔ «دوره‌ای» فعلی در شبیه‌ساز یک اجرای دستیِ آن محرک است، نه زمان‌بندی خودکار.
- [ ] SYNC-225: قرارداد و احراز تحویل webhook واقعی باسلام و پردازش payloadهای تجاری. فرم فعلی شبیه‌سازی کنترل‌شده است.
- [ ] SEC-226: اتصال Worker به vault توکن OAuth و refresh؛ ClientId/Secret واقعی و تأیید ASM-001 هنوز لازم‌اند. هیچ اتصال واقعی ساخته یا فعال نشد.
- [ ] QA-227: آزمون رقابت همزمان چند process و آزمون واقعی drift موجودی در staging؛ آزمون SQL lease/retry جایگزین این پذیرش نیست.

### P2: UI معوق طبق درخواست کاربر
- [x] **OAUTH-229**: علت نمایش آدرس محلی مستند شد: `/api/auth/basalam/login` نقطهٔ شروع داخلی OAuth است و باید با `302` به SSO باسلام برود. `RedirectUri` محیط Development با پروفایل IIS Express روی `https://localhost:44301/api/auth/basalam/callback` همسان شد؛ ClientId/ClientSecret از تنظیم پایه خوانده می‌شوند.
- [x] **OAUTH-230**: ورود مستقیم یا بازشدن GET روی endpoint داخلی login به صفحهٔ شبیه‌ساز برمی‌گردد؛ شروع OAuth فقط از فرم POST دارای زمینهٔ مغازه و antiforgery انجام می‌شود.
- [x] **MENU-228**: منوی پنل بر اساس حوزه‌های کسب‌وکاری Hyper مرتب شد؛ زیرمنوی موجودیت‌ها و گزارش‌ها با تورفتگی متناظر `StartSubMenus/EndSubMenus` تعریف شد. جداول ACT زیر «ابزارهای فنی نئو» قرار گرفتند و منطق یکسان‌سازی تغییری نکرد.
- دسته‌بندی مفهومی و تورفتگی متناظر StartSubMenus/EndSubMenus باید اصلاح شود.
- داشبورد Neo اجرا شد؛ شمارش مغازه و نمودار کالا به تفکیک مغازه داده نشان دادند، ولی چند ویجت «داده در دسترس نیست» داشتند. این خطاها و عنوان قدیمی باشگاه مشتریان در متن جایگزین لوگوی هدر به بک‌لاگ منتقل شدند؛ داشبورد پذیرفته‌شده نیست.
