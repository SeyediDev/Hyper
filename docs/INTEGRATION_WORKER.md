# اجرای پردازش‌گر مستقل هایپریک

پروژه‌های `src/IntegrationWorker/Hyper.IntegrationWorker.Application` و `Hyper.IntegrationWorker.Host` به solution اضافه شده‌اند. Application از قرارداد Domain استفاده می‌کند؛ SQL و HTTP در Infrastructure هستند. الگوی Neo Outbox/ProcessOutboxRecurringJob و OutboxMessageProcessor مرور شد، ولی جدول‌های پایه Neo و repository قدیمی Club در این میزبان ثبت نمی‌شوند.

## تنظیمات

ConnectionStrings__Domain را در محیط اجرای سرویس تنظیم کنید، یا HYPER_SETTINGS_FILE را به فایل تنظیمات امنِ موجود دارای ConnectionStrings.Domain اشاره دهید. رمز در مستندات یا appsettings جدید Worker تکثیر نشده است. میزبان باید به همان Hyperyek و جدول جدید IntegrationOutbox دسترسی داشته باشد.

```powershell
$env:HYPER_SETTINGS_FILE = 'E:\SJVS\Projects\Hyper\Backend\src\CustomerPortal\Hyper.CustomerPortal.Api\appsettings.json'
dotnet run --project src/IntegrationWorker/Hyper.IntegrationWorker.Host
```

این فرمان سرویس را واقعاً اجرا می‌کند و پیام آماده را به provider ارسال می‌کند. اجرای تست‌های SQL از آداپتر کنترل‌شده استفاده می‌کند و فرمان بالا را اجرا نمی‌کند.

## ورودی و وضعیت

`POST connections/{id}/inventory` زیر مسیر integration موجود، MappingId، SourceVersion و Quantity مطلق دریافت می‌کند. هویت و tenant/shop اتصال از مجوز سمت سرور محدود می‌شود. SourceVersion باید برای همان mapping افزایشی و پایدار باشد. تکرار همان نسخه/بدنه همان MessageId را برمی‌گرداند؛ نسخه قدیمی یا بدنه متفاوت رد می‌شود. این API باید از مقدار قطعی هایپریک تغذیه شود.

`GET connections/{id}/outbox` وضعیت را بدون payload یا credential می‌دهد. `POST connections/{id}/outbox/{messageId}/retry` تنها خطای نهاییِ آخرین نسخه را دوباره صف می‌کند؛ پیام قدیمی پس از نسخه جدید قابل replay نیست.

وضعیت‌ها: ۰ انتظار/بازسعی، ۱ lease ارسال، ۲ ارسال‌شده، ۳ خطای نهایی. داشبورد ادمین آمار و ۳۰ پیام آخر را در زمینه مغازهٔ انتخابی نمایش می‌دهد. شمارش روابط مستقل است.

claim با lease، قفل session در SQL به تفکیک اتصال و ترتیب پیام‌ها انجام می‌شود. قفل در طول HTTP نگه داشته می‌شود تا lease منقضی‌شده باعث سبقت پردازش‌گر زنده نشود. lease دو دقیقه و مهلت ارسال یک دقیقه است. پس از قطع پردازش، lease قابل بازیابی است. تأیید پایان به LeaseId محدود است. تضمین تحویل حداقل یک‌بار است؛ قطع SQL/شبکه ممکن است ارسال مطلق را تکرار کند. موفقیت ارسال، اتمام تراکنش حسابداری یا جلوگیری کامل از overselling را ثابت نمی‌کند.

## خواندن خودکار SQL

طبق DEC-005 مرجع، کل موجودی مغازه پس از کسر رزروهاست. پیاده‌سازی capture کاندید `TBL_Product.ACCOUNTINGSTOCK_` را با مجموع رزروهای Status=0 و ReleasedAtUtc=NULL مقایسه می‌کند. کالای غیرفعال/غیرقابل‌فروش آنلاین/خدمت، موجودی قابل ارسال صفر دارد. فیلتر مغازه و tenant کالا و مغازه الزامی است. snapshot تغییر و outbox با transaction واحد ثبت می‌شوند؛ آخرین payload و SourceVersion همان checkpoint است. تغییر کم‌شدن و افزایش‌یافتن دوباره به مقدار قبلی، نسخه جدید ایجاد می‌کند.

فرض ASM-001 هنوز نیازمند تأیید است: عملیات واقعی خرید/فروش باید ACCOUNTINGSTOCK_ را به‌روز کند و رزروها قبلاً از آن کم نشده باشند. ازاین‌رو `IntegrationInventoryCapture:AccountingStockSourceVerified` پیش‌فرض false است. فعال‌سازی پس از تأیید این فرض، capture را هر ۳۰ ثانیه اجرا می‌کند؛ trigger/CDC یا تغییر ساختار جدول قدیمی ایجاد نمی‌شود. همان تنظیم باید بین API و Worker مشترک باشد؛ در حالت capture، API ثبت مستقیم مقدار را رد می‌کند تا دو منبع نسخه‌گذاری رقابت نکنند.

این polling مقدار نهایی را همگام می‌کند و event log همهٔ تغییرات میانی نیست. تا تأیید منبع، producer خرید/فروش به‌عنوان تکمیل‌شده گزارش نمی‌شود. رزرو/صدور فاکتور و همگام‌سازی مشتریان همچنان کار جدا هستند.

- [x] **WRK-STARTUP-201**: خود میزبان ساخته‌شده Worker با SQL محلی و صف خالی اجرا شد؛ startup و polling بدون خطا تأیید شد. capture صریحاً غیرفعال بود و فقط همان پردازش آزمایشی پایان داده شد. تنظیمات اصلی اتصال و TLS تغییر نکردند؛ shared-memory بدون encryption صرفاً override محلی آزمون بود.

## راه‌اندازی صف سناریو و تست رویداد

1. اسکریپت محدود `docs/schema/ensure-integration-scenario-jobs.sql` را روی دیتابیس اتصال پروژه اجرا کنید. این جدول جدید است و جدول legacy را تغییر نمی‌دهد. در محیط محلی Hyperyek اجرا شده است.
2. Worker.Host باید با همان دیتابیس پنل اجرا شود (`ConnectionStrings__Domain`). پنل همچنان از تنظیمات اتصال خودش استفاده می‌کند. رمز در مستندات یا command ثبت نشود.
3. وارد `/merchantsimulation` شوید، مغازه را انتخاب کنید و در بخش «دریافت رویداد آزمایشی» اتصال، آیتم و محرک را انتخاب کنید. فقط اتصال‌های همین مغازه نمایش داده می‌شوند؛ بدون اتصال، فرم ارسال فعال نیست.
4. POST `/MerchantSimulation/ReceiveScenario` درخواست را در صف ثبت می‌کند؛ refresh صفحه، وضعیت/خطا/خلاصه مغایرت‌ها را نشان می‌دهد. Worker به ازای هر دور یک پیام Outbox و یک درخواست سناریو پردازش می‌کند تا هیچ صفی دیگری را گرسنه نگذارد.
5. دریافت و ثبت رویداد موفقیت همگام‌سازی نیست. کالا فعلاً عنوان و نگاشت را مقایسه می‌کند؛ موجودی با منبع تأییدشده اصلاح را به Outbox می‌سپارد. توکن واقعی و `AccountingStockSourceVerified` برای ارسال واقعی لازم‌اند. پردازش حسابداری طرف حساب/فروش/خرید هنوز آماده نیست.

آزمون: `dotnet run --project tools/ScenarioChecks/ScenarioChecks.csproj -- src/AdminPanel/Hyper.AdminPanel.Web/appsettings.json` شامل SQL واقعی با rollback است. در این آزمون آداپتر fake استفاده می‌شود و هیچ تماس HTTP خارجی انجام نمی‌شود. ۲۳ بررسی در 2026-09-15 موفق بود.
