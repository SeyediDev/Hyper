# محیط شبیه‌ساز باسلام

این پروژه فقط برای توسعه و دمو است و هیچ داده‌ای در دیتابیس Hyperyek نمی‌نویسد.
داده‌های غرفه، محصولات، code و token در SQL Server مستقل با نام پایگاه `BasalamSandbox` ذخیره می‌شوند؛ این پایگاه جدا از `Hyperyek` است و هیچ جدول کسب‌وکاری از Hyper را استفاده نمی‌کند. برنامه در شروع، در صورت نبودن database و جدول‌ها، آن‌ها را ایجاد و داده‌های پایه‌ی دمو را seed می‌کند. برای محیط دیگر، مقدار `ConnectionStrings:BasalamSandbox` را با secret manager یا متغیر محیطی `ConnectionStrings__BasalamSandbox` تنظیم کنید.

## اجرا

ترمینال اول:

```powershell
dotnet run --project src/Demo/BasalamSandbox/BasalamSandbox.csproj --urls http://localhost:5217
```

اگر خطای اتصال SQL Server دیدید، connection string `BasalamSandbox` را بررسی کنید؛ اجرای مجدد برنامه داده‌ها را حذف نمی‌کند. اگر خطای `address already in use` دیدید، Sandbox از قبل اجراست؛ همان نمونه را در `http://localhost:5217/sandbox/vendor` استفاده کنید و نمونه‌ی دوم را اجرا نکنید. برای پیدا کردن پردازش مالک پورت:

```powershell
Get-NetTCPConnection -LocalPort 5217 -State Listen |
  Select-Object LocalAddress,LocalPort,OwningProcess
Get-Process -Id <PID>
```

فقط اگر پردازش `BasalamSandbox` است و می‌خواهید آن را restart کنید، همان پردازش را متوقف و دوباره اجرا کنید. اجرای دو نمونه روی یک پورت ممکن نیست.

ترمینال دوم، پنل را در Development اجرا کنید:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project src/AdminPanel/Hyper.AdminPanel.Web/Hyper.AdminPanel.Web.csproj -- --urls https://localhost:44301
```

در Development تنظیمات `Basalam` به Sandbox اشاره می‌کند. در Staging و Production تنظیمات واقعی باسلام بدون تغییر باقی می‌ماند.

## سناریوی دمو

1. به `/sandbox/vendor` بروید و محیط غرفه را ببینید.
2. از پنل Hyper به Merchant Simulation بروید و یک مغازه را انتخاب کنید.
3. روی ورود به باسلام بزنید؛ صفحه با برچسب `SANDBOX / DEMO` نمایش داده می‌شود.
4. غرفه را انتخاب و تأیید کنید؛ callback پنل Hyper همان مسیر واقعی را طی می‌کند.
5. token دمو در Sandbox صادر می‌شود و اطلاعات غرفه از API دمو خوانده می‌شود.
6. در محیط غرفه موجودی را تغییر دهید و اثر آن را در API مشاهده کنید.

## بعد از دریافت توکن

صفحه Merchant Simulation را تازه کنید. وضعیت باید `آماده اجرای سناریوی دمو` باشد. در لحظه‌ی callback، Hyper به‌صورت خودکار اتصال را فعال می‌کند، نگاشت‌های قابل‌تشخیص را می‌سازد و دو job پایدار برای `Product/Initial` و `Inventory/Initial` ثبت می‌کند. ثبت دستی رویداد برای شروع تطبیق لازم نیست.

برای پردازش jobها، worker موجود پروژه را در ترمینال جدا اجرا کنید:

```powershell
$env:HYPER_SETTINGS_FILE = 'src/AdminPanel/Hyper.AdminPanel.Web/appsettings.Development.json'
dotnet run --project src/IntegrationWorker/Hyper.IntegrationWorker.Host/Hyper.IntegrationWorker.Host.csproj
```

هر event یک رکورد مستقل و idempotent در `IntegrationScenarioJobs` است؛ یک event مرکب به چند job تفکیک می‌شود تا خطای موجودی باعث تکرار تطبیق کالا نشود. worker با lease، retry و `sp_getapplock` هم‌زمانی و اجرای دوباره را کنترل می‌کند.

برای آزمایش eventهای بعدی، در بخش «دریافت رویداد آزمایشی»:

1. اتصال غرفه دمو را انتخاب کنید.
2. آیتم `کالا` را انتخاب کنید.
3. محرک `تطبیق اولیه` را انتخاب کنید.
4. «ثبت رویداد در صف» را بزنید.

پردازش کاتالوگ Sandbox را می‌خواند و نتیجه را در «آخرین رویدادها» نشان می‌دهد. نگاشت دمو ابتدا با `SKU ↔ TaxCode` و سپس با عنوان دقیق ساخته می‌شود؛ موردی که تطبیق قطعی ندارد عمداً `NeedsAttention` می‌ماند و هرگز خودکار به کالای اشتباه وصل نمی‌شود. در حالت Demo، بررسی موجودی حسابداری مجاز است و اختلاف موجودی از مسیر outbox به Sandbox منتشر می‌شود.

دامنه‌های داده‌ی Sandbox برای vertical slice شامل غرفه/پروفایل (`/v3/users/me`)، کاتالوگ کالا و SKU، قیمت و موجودی (`/v1/vendors/{vendorId}/products`)، جزئیات کالا، تغییر موجودی و توکن OAuth است. این‌ها به‌ترتیب به اتصال/توکن Hyper، `ExternalProductMappings`، تطبیق محصول و outbox موجودی نگاشت می‌شوند. سفارش، فروش، خرید و طرف‌حساب در قرارداد provider-neutral به‌عنوان eventهای مستقل رزرو شده‌اند و تا زمانی که endpoint و مدل رسمی باسلام برایشان قطعی نشده، موفقیت جعلی گزارش نمی‌شود.

توکن دمو هرگز برای endpointهای واقعی باسلام معتبر نیست. کد سرویس با `Mode=Demo` فقط از endpointهای loopback استفاده می‌کند و در حالت `Real` فقط endpointهای رسمی باسلام مجاز هستند.

RabbitMQ در این مرحله نصب نشده است. صف، outbox و retry فعلی Hyper برای سناریوی دمو کافی است؛ broker زمانی اضافه می‌شود که پردازش مستقل و توزیع‌شده واقعاً لازم باشد.
