# سند تحلیل و طراحی یکپارچه‌سازی

## دامنه

هایپریک مالک داده‌های حسابداری، کالا، موجودی و سفارش است. سامانه‌های فروش خارجی فقط کانال ارائه و دریافت رویداد هستند. اتصال به فروشگاه پس از ثبت token و تأیید مالک فروشگاه فعال می‌شود.

## جریان ثبت اتصال

`POST /api/v{version}/Hyper/integrations/connections` اتصال را با provider، shop، account identifier و credential ثبت می‌کند. هدف، رمزنگاری credential پیش از ذخیره است و هنوز اجرا نشده. `GET` فعلی فاقد فیلتر مالکیت tenant است و باید اصلاح شود و `DELETE/disable` اتصال را غیرفعال می‌کند.

## جریان یکسان‌سازی

تغییرات Hyperyek به event تبدیل می‌شوند. worker با توجه به mapping، payload provider را تولید و ارسال می‌کند. پاسخ و checkpoint در `IntegrationSyncRuns` ثبت می‌شود. خطای موقت retry و خطای دائمی به dead-letter منتقل می‌شود.

## جریان webhook

درخواست در inbox ذخیره، امضا و timestamp بررسی، سپس با idempotency key پردازش می‌شود. رویداد سفارش/لغو/پرداخت/موجودی به command داخلی هایپریک تبدیل و با transaction اعمال می‌شود.

## امنیت و مشاهده‌پذیری

هدف طراحی tenant scoped بودن تمام endpointهاست؛ پیاده‌سازی فعلی این تضمین را ندارد. token در log و response نمایش داده نمی‌شود. correlation id، provider، shop و sync run در log ثبت می‌شوند. داشبورد از view `vw_IntegrationDashboard` استفاده می‌کند.

## فرآیندهای مصوب

رویدادهای اشتراک (A1/A2)، catalog (B1/B2)، parcel (C1/C2)، فروش غرفه (D1/D2)، خرید مشتری (E1/E2)، اصلاح اقلام (F1/F2 و G1/G2)، review (H1/H2) و chat (I1/I2 و J1/J2) باید در inbox و outbox پردازش شوند. هر رویداد دارای provider، shop، tenant، external id، event type، occurred at و idempotency key است.

برای فروش غرفه، Hyperyek مرجع رزرو و کاهش موجودی است. برای فروش در Hyperyek، outbox تغییر موجودی/قیمت به آداپتر provider ارسال می‌شود. mapping کالا بر اساس SKU/بارکد و در صورت تنوع بر اساس variant انجام می‌شود. همه تغییرات با source و correlation id ثبت می‌شوند تا حلقه همگام‌سازی ایجاد نشود.

APIهای باسلام طبق Sheet شامل subscription، vendor products، product patch، parcel، customer order، chat و file upload هستند؛ پیاده‌سازی آن‌ها باید فقط در Infrastructure adapter باشد و orchestration با strategy بر اساس Provider و CredentialType آداپتر را انتخاب کند.
## پیاده‌سازی فعلی

Endpointهای GET /api/v{version}/Hyper/integrations/connections، POST /api/v{version}/Hyper/integrations/connections/{id}/sync و POST /api/v{version}/Hyper/integrations/webhooks/{id} اضافه شده‌اند. webhook در IntegrationWebhookInbox با کلید یکتا ذخیره می‌شود تا رویداد تکراری اثر تکراری نداشته باشد. مرحله بعدی اعتبارسنجی امضای provider و تبدیل inbox به commandهای فروش، رزرو، مرجوعی، review و chat است.

## ماتریس ردیابی بک‌لاگ

| نیاز سند | پیاده‌سازی هدف | شناسه بک‌لاگ |
|---|---|---|
| ثبت Vendor/Token و subscription | Registry + Credential Vault + licensing | INT-001, INT-002, FLOW-A |
| catalog و variation دوطرفه | Mapping + provider catalog adapter + loop prevention | FLOW-B, ADP-BASALAM, WRK-004 |
| فروش، parcel و موجودی | Outbox/CDC + reservation + invoice adapter | FLOW-C, FLOW-D, INT-006 |
| خرید، لغو و مرجوعی | reservation release و accounting correction | FLOW-E, FLOW-F/G |
| review، CRM و chat | event processor، ticket، inbox و rate limit | FLOW-H, FLOW-I/J |
| پنل مدیریت و KPI | dashboard query، mapping diff و run health | UI-001..UI-003 |

هر commit باید شناسه بک‌لاگ مربوط را در پیام commit یا توضیح PR درج کند. هر مرحله پس از اجرای تست مربوط، وضعیت همان شناسه را در BACKLOG.md به‌روزرسانی می‌کند.
### API ثبت اتصال (API-004)
مغازه‌دار از POST /api/v{version}/Hyper/integrations/connections برای ثبت provider، شناسه غرفه و credential استفاده می‌کند. پاسخ هرگز CredentialsJson را برنمی‌گرداند. فهرست provider از GET /api/v{version}/Hyper/integrations/providers و غیرفعال‌سازی با POST /connections/{id}/disable انجام می‌شود. ثبت تکراری برای shop/provider/account با HTTP 409 رد می‌شود.

APIهای مشاهده mapping و run نیز اضافه شدند: GET /api/v{version}/Hyper/integrations/connections/{id}/mappings و GET /api/v{version}/Hyper/integrations/connections/{id}/runs.

Endpointهای GET /api/v{version}/Hyper/integrations/connections/{id}/webhooks و POST /api/v{version}/Hyper/integrations/webhooks/{webhookId}/replay برای مشاهده و replay دستی inbox اضافه شدند. پردازش‌گر worker در مرحله WRK-001 باقی است.

## برنامه اجرایی Sprint بعدی

ترتیب اجرا اجباری است: ابتدا WRK-101 تا WRK-105، سپس CMD-101 تا CMD-107، بعد ADP-101 تا ADP-106 و در نهایت API/UI و E2E. هیچ adapter نباید مستقیماً DbContext را تغییر دهد؛ adapter فقط قرارداد provider را اجرا می‌کند و command handler مسئول تغییر Hyperyek است. ثبت سفارش و رزرو موجودی باید در transaction داخلی هایپریک انجام شود و نتیجه‌ی ارسال به provider از outbox منتشر شود.

هر رویداد ورودی باید قبل از dispatch در IntegrationEventAudit hash و نتیجه‌ی signature داشته باشد. هر رویداد خروجی باید source=Hyperyek داشته باشد تا webhook برگشتی دوباره اعمال نشود.
جداول عملیاتی ExternalOrderMappings، InventoryReservationLogs و IntegrationEventAudits ایجاد و در HyperContext ثبت شدند. این جداول مرجع نگاشت سفارش/مرسوله، رزرو و آزادسازی موجودی و audit امنیتی رویدادها هستند.

## اثر تحویل داده‌های عملیاتی

نگاشت سفارش، رزرو موجودی و audit رویداد اکنون موجودیت و جدول مستقل دارند. جریان‌های D1/D2 از این جداول برای پیوند Parcel با فاکتور و رزرو اتمیک استفاده می‌کنند؛ جریان‌های E1/E2 و F/G وضعیت رزرو و اصلاحیه را ثبت می‌کنند. هنوز command handler مالی و worker اجرای inbox/outbox باید تکمیل شود و تا آن زمان آیتم‌های ACC/INV/ORD در بک‌لاگ تکمیل‌نشده هستند.
امنیت webhook ارتقا یافت: payload hash، نتیجه HMAC، correlation و timestamp در audit ثبت می‌شوند و timestamp خارج از بازه پنج دقیقه رد می‌شود.

قرارداد IntegrationEventEnvelope و IIntegrationStrategyResolver اضافه شد؛ orchestration فقط envelope و provider را می‌شناسد و adapter اختصاصی هر پلتفرم در Infrastructure resolve می‌شود.

پایه HTTP مشترک و adapterهای قابل تزریق برای Basalam/Digikala/Torob ایجاد شد؛ endpointهای واقعی provider در مرحله ADP-101..105 تکمیل می‌شوند.

## اصلاح تحلیل بر اساس کد و آزمون — 2026-09-10

آداپتر با زوج Provider و CredentialType انتخاب می‌شود. آداپتر باسلام/دیجی‌کالا/ترب تا زمان پیاده‌سازی مستند، unavailable است و نباید sync موفق گزارش کند. اتصال غیرفعال، منقضی یا فاقد مشخصات مالک/credential پیش از ایجاد run رد می‌شود؛ این بررسی جایگزین احراز مالکیت نیست. تطبیق catalog فقط snapshot نگاشت معتبر موجود را به‌روز می‌کند؛ کالای فاقد نگاشت معتبر به‌عنوان نیازمند mapping ثبت می‌شود. موجودی حسابداری باید از outbox هایپریک استخراج شود.

وب‌هوک عمومی با verifier در Infrastructure مرزبندی شد. پروتکل `hyper-hmac-v1` فقط برای Provider.Custom تعریف شده و پروتکل باسلام نیست. credential باید شامل webhookSignatureScheme و webhookSecret حداقل 32 بایت UTF8 باشد. متن امضا به ترتیب scheme، connection id، timestamp، event id، event type است که هر کدام با LF پایان می‌یابد و سپس بایت خام body می‌آید. هدرهای X-Timestamp، X-Event-Id، X-Event-Type و X-Signature الزامی‌اند؛ امضا HMAC-SHA256 به صورت 64 نویسه hex، پنجره زمانی ±300 ثانیه و سقف بدنه 1MiB است. timestamp و هویت رویداد نیز امضا می‌شوند. تکرار معتبر با کلید یکتای connection/event در inbox مدیریت می‌شود؛ آزمون همزمانی SQL باقی است.

52 بررسی مستقل قرارداد/Strategy/verifier پاس شد. احراز مالکیت API، حفاظت credential، پروتکل provider واقعی، worker، حسابداری و تطبیق migration هنوز آماده انتشار نیستند. گزارش‌های پیشین آماده‌بودن همه لایه‌ها یا همه منابع گوگل، با شواهد فعلی تأیید نمی‌شوند. FIX-MODEL-001 باید از metadata واقعی SQL و قرارداد IEntity در Neo استفاده کند تا ستون Id ساختگی به جداول دارای کلید نامتعارف یا مرکب تحمیل نشود.

## نتیجه تحلیل مدل SQL و مرز Context — 2026-09-10

دیتابیس مرجع 150 جدول و 3 نما دارد؛ مدل جدید تمامی 2011 ستون و 227 FK را پوشش می‌دهد. ID از نام ستون حدس زده نمی‌شود. پایه SqlServerEntity قرارداد marker و domain event نئو را اجرا می‌کند؛ نوع generic فقط جداول با کلید واقعی Id را به IEntity<TKey> متصل می‌کند. کلیدهای مرکب و جداول/نماهای بدون کلید از generic repository تک‌کلیدی استفاده نمی‌کنند. این تصمیم نیاز کاربر برای رفع بنیادی مغایرت BaseEntity را بدون تغییر schema و بدون تغییر سورس مشترک Neo پوشش می‌دهد.

10822 بررسی مدل پاس شد و خواندن یک رکورد از هر 153 شیء SQL با EF موفق بود؛ 113 شیء داده داشتند. خواندن از shared memory محلی و بدون نوشتن انجام شد. دو entity اختصاصی قدیمی باسلام و پیکربندی/DbSet آنها حذف شدند، چون هیچ جدول متناظر در SQL فعلی ندارند.

Context حسابداری و Context عملیاتی Integration از هم جدا هستند. API و orchestration به HyperIntegrationContext منتقل می‌شوند تا مدل‌های Club هنگام دریافت وب‌هوک ساخته نشوند. هر هفت جدول integration با metadata واقعی تنظیم می‌شوند؛ از جمله nvarchar/varchar، دقت اعشار، default و FK بدون cascade ساختگی. این جداسازی هنوز به معنی پاک‌سازی همه ویژگی‌ها، منوها و لایه‌های قدیمی برنامه نیست.

همه جداول baseline تحت مالکیت دیتابیس هستند و ExcludeFromMigrations دارند؛ خروجی صفر DDL نتیجه صریح این سیاست است، نه ادعای برابری snapshot قبلی. پنج constraint که nullable یا در جدول keyless هستند در EF با unique index و annotation حفظ معنایی می‌شوند؛ تفاوت نمایش constraint و index در snapshot تحلیل ثبت است. تغییرات ساختاری آینده integration باید migration صریح و مرورشده داشته باشند. MODEL-MIGRATION-001 تا تأیید کامل migration context اجرایی باز می‌ماند.

تأیید CTX-INTEGRATION-001: کد تولیدی Controller، SynchronizationService و HyperIntegrationContext همراه با قراردادهای واقعی Neo در پروژه بررسی کامپایل شد؛ هر 68 ستون هفت جدول عملیاتی با baseline برابر بود و FKها تطبیق داشتند. این تست احراز هویت، مالکیت یا همزمانی دریافت webhook را پوشش نمی‌دهد. Restore کامل API نیز با cache محلی و artifacts مسیر workspace موفق شد؛ وضعیت build کامل جداگانه ثبت می‌شود.


## نتیجه build کامل — 2026-09-10

BUILD-001 تکمیل شد: restore آفلاین با cache محلی و build کامل Hyper.CustomerPortal.Api با وابستگی‌های Neo و Neo.Bpms موفق بود؛ صفر خطا و هشدار. خروجی در workspace تولید شد و سورس چارچوب‌ها تغییر نکرد. این نتیجه به معنی اجرای HTTP، صحت مالکیت کاربران، نوشتن حسابداری یا پیاده‌سازی provider نیست. اولویت امنیت مالکیت و credential vault پیش از فعال‌سازی عملیاتی حفظ می‌شود.

## تصمیم هویت و عدم استفاده از جداول پایه Neo — درخواست کاربر 2026-09-10

جداول موجود SQL تغییر نمی‌کنند. User ادمین پنل فقط برای مدیریت پنل است؛ هویت مغازه‌دار از آن استخراج نمی‌شود. هیچ جدول پایه Neo برای مدل عملیاتی جدید استفاده نمی‌شود. طرح دسترسی بر اساس JOIN به ACT_ID_USER لغو شد و به نگاشت جدید IntegrationMerchantAccess میان هویت حساب هایپریک (issuer/subject معتبر) و ShopId منتقل می‌شود. خواندن TBL_Shop فقط برای وجود مغازه و tenant واقعی است، بدون استفاده از OWNERID_ یا User. هیچ کاربر عادی از API ثبت اتصال، مجوز جدید برای خودش ایجاد نمی‌کند. فراهم‌سازی مجوز معتبر و چرخه لغو/انقضای آن مستقل از جدول کاربران ادمین تعریف می‌شود.

## زمینه ادمین برای شبیه‌سازی درخواست مغازه‌دار

طبق توضیح کاربر، ادمین ابتدا در MerchantSimulation/Index مغازه‌دار و مغازه را انتخاب می‌کند. شناسه مغازه‌دار از شناسه تجاری درج‌شده در TBL_Shop نمایش داده می‌شود، نه از User ادمین. انتخاب در جدول جدید IntegrationAdminSimulations با AdminUserId مستقل ثبت می‌شود و بلیت حفاظت‌شده فقط برای همان ادمین معتبر است. زمان اعتبار 30 دقیقه، پایان صریح و ابطال هنگام تغییر زمینه اعمال می‌شود. تمام POSTها antiforgery دارند. درخواست توکن با بلیت فرم و ShopId نمایشی تطبیق داده می‌شود و در جدول جدید IntegrationTokenRequests با وضعیت Prepared ذخیره می‌شود؛ هیچ token ساختگی و هیچ ارسال واقعی provider انجام نمی‌شود. دسترسی این صفحه از GetUser/IsAdmin مطابق ساختار Neo.Bpms گرفته می‌شود.

MerchantSimulation/Dashboard از همین زمینه معتبر استفاده می‌کند؛ پارامتر ShopId ورودی URL مرجع انتخاب نیست. آمار ارتباط‌ها، نگاشت‌ها، runها و inbox به صورت مستقل و در محدوده shop/tenant جمع می‌شوند تا fan-out و اختلاط آمار رخ ندهد. سوابق واقعی نمایش داده می‌شوند؛ کامل‌شدن worker و ثبت مالی شرط آماده‌شدن یکسان‌سازی واقعی است.

اعتبارسنجی مرحله داشبورد و شبیه‌سازی: 79 بررسی قرارداد/امنیت موفق است. آزمون SQL داشبورد با چند رابطه همزمان، شمارش مستقل و جداسازی tenant را تأیید کرد. ثبت درخواست شبیه‌سازی با INSERT...SELECT شرطی انجام می‌شود تا EndedAtUtc و Expiry در خود statement درج کنترل شود. همه سوابق آزمایشی rollback شدند و جداول قبلی تغییری نکردند. صف پردازش واقعی و عملیات حسابداری هنوز پیاده‌سازی نشده‌اند.

Build نهایی پنل ادمین و Razor موفق بود (0 warning/error). مسیرهای MerchantSimulation/Index و MerchantSimulation/Dashboard و منوی Neo.Bpms اضافه شدند. قالب JSON نشانگر زمینه با نام‌های صریح camelCase تعریف شد، زیرا تنظیمات Neo.Bpms naming policy را null می‌کند. نشانگر هنگام بازگشت به صفحه و هر دقیقه تازه می‌شود؛ بررسی اعتبار عملیات همیشه در سرور انجام می‌شود.

## تصمیم جدید کاربر — 2026-09-10

DEC-005: موجودی مرجع غرفه، موجودی قابل‌فروش کل مغازه پس از کسر رزروهاست؛ پاسخ «۱» کاربر ثبت شد. فرض فنی درباره منبع SQL جدا از این تصمیم است و در DECISIONS.md با وضعیت مشخص نگهداری می‌شود. از این مرحله، همه فرض‌ها و تغییرات خواسته‌شده باید در DECISIONS.md، تحلیل و بک‌لاگ اثر داشته باشند.

- [ ] **INV-204**: خواندن کل موجودی مغازه و کسر رزروهای فعال بدون تغییر legacy SQL؛ تأیید رابطه AccountingStock و رزرو بومی/خرید/فروش، سپس capture اتمیک تغییر و outbox. منبع موجودی بیرونی مرجع ارسال نیست.
- [ ] **DEC-TRACK-001**: نگهداری دائمی تصمیم‌ها و فرض‌ها با وضعیت قطعی/موقت و اثر روی کد؛ سند DECISIONS.md ایجاد شد و DEC-001 تا DEC-005 ثبت شدند.

## معماری اجرایی فعلی و شواهد منابع — 2026-09-10

مطابق SOURCE_REVIEW_2026-09-10.md متن کامل سند و سه تب شیت خوانده شدند. تصمیم‌های قطعی و فرض‌های فنی با شناسه در DECISIONS.md ثبت می‌شوند. پاسخ کاربر، موجودی قابل‌فروش کل مغازه منهای رزرو را مرجع قرار داد (DEC-005)؛ انتخاب فنی ACCOUNTINGSTOCK_ هنوز فرض ASM-001 است و با این پاسخ خودکار تأیید نمی‌شود.

مرز لایه‌ها: Worker.Application فقط IIntegrationOutbox/IIntegrationInventoryCapture دامنه را فراخوانی می‌کند؛ Infrastructure مالک SQL queue، capture و adapter HTTP است. میزبان worker بدون repository یا جدول پایه Neo از همان DI integration API/پنل استفاده می‌کند. الگوهای Neo Outbox مطالعه شدند؛ persistence مستقل از OutboxMessage پایه Neo است.

حفظ تغییر خروجی: IntegrationOutbox به connection و mapping عمومی متصل است و Operation، PayloadJson، SourceVersion، attempts، زمان بازسعی، lease و نتیجه را نگه می‌دارد. نسخهٔ افزایشی و قفل mapping مانع ثبت تکراری/قدیمی می‌شوند. قفل session اتصال در طول ارسال از سبقت worker همزمان حتی پس از انقضای lease جلوگیری می‌کند. ادامه پس از crash حداقل یک‌بار است و SET موجودی مطلق تکرارپذیر است. این روش به معنی exactly-once یا تأیید مالی نیست.

Capture کاندید SQL در transaction واحد، کل موجودی محصول مغازه را می‌خواند و رزروهای فعال جدید را کم می‌کند؛ checkpoint همان آخرین payload/version نگاشت در صف است. فقط در صورت تأیید ASM-001 فعال می‌شود. حالت API producer و SQL capture باید تنظیم مشترک داشته باشند تا دو منبع رقابت نکنند. Polling مقدار نهایی را مقایسه می‌کند و تضمین ثبت تک‌تک رخدادهای میانی StockCard نیست.

باسلام اکنون GET catalog و PATCH stock واقعی را بر اساس SDK/OpenAPI رسمی در Infrastructure دارد؛ HTTP contract با تست کنترل‌شده تأیید شد، ولی تماس واقعی با غرفه انجام نشده است. Customer/order/accounting، webhook رسمی، OAuth و حفاظت credential هنوز مستقل باز هستند. داشبورد آمار صف و آخرین پیام‌ها را کنار catalog run و webhookهای ذخیره‌شده نشان می‌دهد و به shop/tenant زمینه ادمین محدود است.

### نتیجهٔ آزمون capture و replay

آزمون SQL نهایی capture با خواندن کالای واقعی و fixture فقط در جدول‌های integration پاس شد: منبع تأییدنشده فعال نمی‌شود؛ اولین snapshot ثبت می‌شود؛ مقدار یکسان پیام جدید نمی‌سازد؛ رزرو فعال کسر و آزادسازی بازتاب داده می‌شود. هیچ legacy row نوشته نشد. آزمون replay فقط آخرین خطای نهایی، رد اتصال دیگر و رد نسخه قدیمی پس از نسخه جدید نیز پاس شد. تمام fixtureهای اتصال، mapping، outbox و رزرو پاک شدند. INV-CAPTURE-201 از نظر کد و تست SQL تأیید شد، ولی تأیید ASM-001 و فعال‌سازی روی writer اصلی همچنان در INV-204 باز است.

### تأیید نهایی ساخت این مرحله

build نهایی هر سه پروژه Hyper.IntegrationWorker.Host، Hyper.CustomerPortal.Api و Hyper.AdminPanel.Web شامل Razor با ۰ warning و ۰ error موفق شد. ۹۵ بررسی قرارداد/امنیت/HTTP کنترل‌شده و ۱۱۰۳۲ assertion مدل پاس شدند. تست SQL outbox/capture/replay با provider کنترل‌شده، جداسازی tenant و پاک‌سازی fixtureها موفق است. تأیید ظاهری پنل با ورود واقعی و اتصال واقعی provider همچنان باز است.

- [x] **INV-CAPTURE-CODE-201**: کد و آزمون SQL capture موجودی/رزرو تکمیل شد؛ فعال‌سازی ASM-001 همچنان باز است.
- [x] **DASH-OUTBOX-SQL-201**: query واقعی آمار outbox و Razor داشبورد ساخته و SQL query با tenant دیگر تست شد؛ بررسی مرورگر جداست.
- [x] **WRK-BUILD-202**: Worker، API و پنل پس از تغییرات این مرحله build شدند.
- [x] **DEC-TRACK-INIT-001**: سند تصمیم‌ها و فرض‌ها ایجاد و تصمیم قطعی موجودی کاربر در تحلیل/بک‌لاگ اعمال شد؛ نگهداری آن وظیفه مستمر هر مرحله است.

## طراحی نمای مدیریتی — 2026-09-10

طبق اولویت جدید کاربر، صفحهٔ اول پنل به AdminDashboard منتقل می‌شود. پوسته RTL با فونت محلی، منوی سه‌مسیره، کارت‌های شاخص، نمودار قابل مشاهده به صورت جدول، صف خروجی و آخرین عملیات طراحی شد. صفحات انتخاب زمینه و گزارش یکسان‌سازی از همین پوسته استفاده می‌کنند. Neo.Bpms همچنان قراردادهای هویت ادمین، کنترلر، DI و تعریف منو را فراهم می‌کند؛ اطلاعات کسب‌وکار از مدل دقیق SQL هایپریک و جداول عمومی integration خوانده می‌شود.

ادمین بدون زمینهٔ انتخاب‌شده نمای همهٔ مغازه‌ها را دارد. با زمینه معتبر، query به ShopId و TenantId همان مغازه محدود است. انتخاب زمینه، توکن واقعی یا مالکیت حساب مشتری ایجاد نمی‌کند. پیش‌نمایش فقط برای بررسی طراحی در Development و با دادهٔ نمونه است؛ مجوزی برای خواندن SQL به کاربر مهمان نمی‌دهد.

در نمودار، فاکتورهای همهٔ وضعیت‌ها و اجراهای موفق دریافت کاتالوگ جدا هستند. صف خروجی کل سوابق محدوده را نمایش می‌دهد و با بازهٔ روند اشتباه گرفته نمی‌شود. «اتصال فعال» فقط پرچم ثبت‌شده است و سلامت توکن یا قابلیت فعال adapter پلتفرم را اثبات نمی‌کند. تصمیم‌های DEC-006 تا DEC-009 و فرض‌های ASM-005/006 جزئیات را مشخص می‌کنند.

رفع مانع startup: فراخوانی jobهای کپی‌شدهٔ کلاب حذف و نصب خودکار schema Hangfire غیرفعال شد. بررسی اجرای واقعی، SQL و UI در ADMIN-RUN/OVERVIEW/PREVIEW-202 ثبت می‌شود.

## اصلاح جریان احراز هویت پنل — 2026-09-12

جریان ورود دو مرحله‌ای Neo حفظ شد. returnUrl در Login→Verify و Verify→Destination منتقل می‌شود. کوکی موقت نام کاربری در HTTP توسعه‌ای Secure نیست و در HTTPS Secure است. کد ثابت، ثبت خودکار کاربر و JWT ساختگی حذف شد.


## اصلاح معماری صفحه اصلی — 2026-09-12

HomeController به DesktopController استاندارد Neo بازگردانده شد؛ داشبوردهای UiDefinitions از HomePageEntity ساخته می‌شوند و AdminDashboard نمای عملیاتی integration است. اسکن عمومی اسمبلی‌های کپی‌شده Club حذف شد. پاکسازی فایل‌های باقی‌مانده در CLUB-CLEANUP-206 ادامه دارد.

## پاکسازی Runtime موجودیت‌های Club — 2026-09-12

ثبت سرویس‌های Club و jobهای Lottery/Promotion از DI حذف شد و configurationهای EF آن‌ها compile نمی‌شوند. جریان رسمی Hyper اکنون فقط حسابداری legacy، موجودیت‌های عمومی اتصال/نگاشت و worker یکسان‌سازی را فعال می‌کند. حذف فیزیکی فایل entityها به CLUB-ENTITY-210 موکول است تا referenceهای باقی‌مانده در HyperContext و پروژه‌های قدیمی ابتدا حذف شوند.
