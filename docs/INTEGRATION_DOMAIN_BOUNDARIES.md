# به نام خدا

# مرزهای دامین یکپارچه‌سازی هایپریک

## هدف

دامین Integration باید مستقل از دامین‌های کسب‌وکار Hyperyek، دامین پنل ادمین و دامین providerهای خارجی توسعه پیدا کند. هدف این استقلال، جلوگیری از قاطی‌شدن مدل‌ها و امکان واگذاری API به تیم پلتفرم در آینده است.

## مالکیت‌ها

### Integration

مالک این مفاهیم است:

- اتصال خارجی و provider
- credential و چرخه token
- دریافت و پردازش event
- webhook inbox و idempotency
- mapping موجودیت خارجی
- sync run، retry، dead-letter و replay
- سناریو و simulation
- audit و وضعیت عملیات integration

### Hyperyek business domains

مالک کالا، موجودی، سفارش، مشتری، فاکتور، حسابداری و سایر مفاهیم عملیاتی Hyperyek هستند. Integration نباید entity یا repository این دامین‌ها را در مدل خودش کپی یا مستقیماً مصرف کند.

### Admin UI

پنل در معماری هدف فقط مسئول نمایش dashboard، CRUD تنظیمات، اجرای simulation و ارسال command از طریق API است و مالک منطق Integration و persistence آن نیست. برای جلوگیری از شکستن داشبورد و CRUD فعلی، دسترسی به entity/menu/UI definitionهای legacy فعلاً حفظ شده است؛ این یک سازگاری موقت و کنترل‌شده است، نه مجوز افزودن feature جدید یا برگشت مالکیت persistence به دامین اصلی.

## ساختار مجاز

```text
Hyper.Integration.Contracts
Hyper.Integration.Domain
Hyper.Integration.Application       (مرحله بعد)
Hyper.Integration.Infrastructure    (استخراج شده؛ adapterهای مرزی به Hyperyek را مصرف می‌کند)
Hyper.Integration.Api
Hyper.Integration.Worker
Hyper.AdminPanel.Web  --> فقط API/contractهای Integration
```

`Hyper.Integration.Contracts`، `Hyper.Integration.Domain`، `Hyper.Integration.Api` و `Hyper.Integration.Infrastructure` اکنون پروژه‌های مستقل هستند. پروژه‌ی Infrastructure به contextهای دیتابیس و adapterهای Hyperyek از assembly مشترک دسترسی می‌گیرد، اما implementationهای Integration به‌صورت فیزیکی در پروژه‌ی مستقل نگهداری می‌شوند و دیگر در `Hyper.Infrastructure` compile نمی‌شوند.

تمام جدول‌های Integration، از جمله token و customer mapping، در `ConnectionStrings:IntegrationConnection` مالکیت دارند. `HyperSqlServerContext` آن‌ها را map یا expose نمی‌کند؛ شناسه‌های حسابداری مانند `PersonId` فقط scalar contract هستند.

برای عملیات طرف حساب، `IIntegrationAccountingPort` در Domain تعریف شده و `HyperyekAccountingCustomerAdapter` در Infrastructure آن را پیاده می‌کند. بنابراین orchestrator ثبت mapping، entity یا repository حسابداری را نمی‌شناسد.

## قوانین ارتباط

1. Domain/Application و persistence Integration به entity یا DbContext دامین دیگر reference مستقیم ندارند. Adapterهای موقت لایه Infrastructure مانند `IntegrationCustomerRegistration` فقط برای عملیات Hyperyek هستند و باید از طریق port/API مستقل جایگزین شوند.
2. شناسه کسب‌وکاری طرف مقابل به‌صورت scalar و در قالب contract نگهداری می‌شود؛ navigation و foreign key بین دامین‌ها ایجاد نمی‌شود.
3. تغییرات Hyperyek از event/outbox یا API قراردادشده دریافت می‌شود؛ خواندن مستقیم جدول‌های owned-by-other-team فقط با تصمیم موقت و ثبت‌شده مجاز است.
4. providerها فقط adapter زیرساختی هستند و API باسلام/هایپریک نباید وارد Domain شود.
5. API عمومی webhook فقط event را دریافت و ثبت می‌کند؛ پردازش business در Application/Worker انجام می‌شود.
6. tokenها فقط در مرز Integration مدیریت می‌شوند و هرگز در DTO، log، dashboard یا event خام نمایش داده نمی‌شوند.
7. API مدیریت Integration versioned و مستقل است؛ Controller فقط contract/authorization را انجام می‌دهد و persistence در Infrastructure باقی می‌ماند.
8. پنل برای قابلیت‌های جدید dashboard، CRUD و simulation فقط API/Contract را مصرف می‌کند؛ query مستقیم `HyperIntegrationContext` در UI ممنوع است. metadata و CRUDهای legacy پنل فعلاً برای backward compatibility باقی می‌مانند و در migration مرحله‌ای حذف/redirect خواهند شد.
9. scope مجاز از claim مورداعتماد `integration_scope` می‌آید؛ headerهای shop/tenant هرگز به‌تنهایی authorization محسوب نمی‌شوند.

## ترتیب اجرای بک‌لاگ

ابتدا `ARCH-DOMAIN-001` تا `ARCH-DOMAIN-006` تعیین تکلیف و بررسی می‌شوند. پس از آن، تکمیل `INT-001`، `INT-002`، `INT-005` و `WRK-001` انجام می‌شود. featureهای flow مانند سفارش، موجودی و حسابداری فقط پس از تعریف contract بین دامین‌ها قابل شروع هستند.

## معیار بازبینی هر تغییر

- owner دامین مشخص است.
- dependency به دامین دیگر ندارد یا فقط از contract استفاده می‌کند.
- DbContext و migration صحیح انتخاب شده‌اند.
- endpoint و DTO در API مستقل قرار دارند.
- پردازش asynchronous، idempotent و قابل replay است.
- تست معماری عدم reference مستقیم را اثبات می‌کند.
# مرز Neo و Neo.Bpms

## قرارداد API پلتفرم حسابداری

نام قراردادی API مالک عملیات حسابداری، سفارش و موجودی Hyperyek:

```text
Hyperyek.Accounting.Api
```

این API متعلق به دامین پلتفرم Hyperyek است و در آینده می‌تواند به تیم پلتفرم
تحویل شود. `Hyper.Integration.Api` مالک webhook و orchestration یکپارچه‌سازی است
و فقط از قرارداد versioned این API یا پیام‌های رسمی استفاده می‌کند؛ دسترسی مستقیم
به جدول‌های `TBL_*` حسابداری از Integration ممنوع است.

مرز هدف:

```text
Hyper.Integration.Api -> Hyperyek.Accounting.Api -> Hyperyek accounting application/domain -> database
```

`Neo.Bpms` فقط زیرساخت پنل ادمین است. پروژه‌های Integration (Domain، Contracts، API،
Infrastructure و Worker) باید فقط به Neo و قراردادهای خودشان وابسته باشند و نباید
به هیچ assembly از `Neo.Bpms.*` reference مستقیم یا غیرمستقیم داشته باشند.

در وضعیت فعلی، reference مستقیم Integration به `Neo.Bpms` مشاهده نشد؛ اما یک مسیر
غیرمستقیم باقی است:

`Hyper.Integration.Infrastructure` → `Hyper.Infrastructure` → `Hyper.Domain` → `Neo.Bpms.Domain`

این مسیر از نظر معماری مطلوب نیست و باید با خارج‌کردن attributeها و انواع Neo.Bpms
از `Hyper.Domain` و انتقال قراردادهای metadata موردنیاز به لایه پنل/adapter حذف شود.
استفاده مستقیم `AdminPanel.Domain` و `AdminPanel.Web` از Neo.Bpms برای حفظ داشبورد،
CRUD و موجودیت‌های legacy فعلاً مجاز است.

## تصمیم تغییر

حذف مسیر غیرمستقیم فوق یک تغییر ساختاری است و تا تأیید مالک پروژه انجام نمی‌شود؛
زیرا ممکن است metadata و UI موجود را تحت تأثیر قرار دهد. تا آن زمان این مورد به‌عنوان
مانع معماری ثبت‌شده و قابل پیگیری باقی می‌ماند.
