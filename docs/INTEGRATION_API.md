# به نام خدا

# قرارداد API مستقل Integration — v1

مالک این API دامین Integration است. پنل ادمین و هر مصرف‌کننده‌ی دیگر فقط قراردادهای این سند را مصرف می‌کنند و نباید به `HyperIntegrationContext` یا entityهای Integration reference مستقیم داشته باشند.

## احراز هویت و scope

همه endpointهای مدیریتی `[Authorize]` هستند. مصرف‌کننده باید claim زیر را از issuer مورداعتماد دریافت کند:

```text
integration_scope=shop:{ShopId};tenant:{TenantId}
```

`X-Shop-Id` و `X-Tenant-Id` فقط پارامتر انتخاب scope هستند و به‌تنهایی مجوز ایجاد نمی‌کنند. وب‌هوک ingress عمومی است، اما با connection، signature، idempotency و audit کنترل می‌شود.

## endpointها

| Method | مسیر | کاربرد |
|---|---|---|
| POST | `/api/integrations/v1/webhooks/{provider}/{connectionKey}` | دریافت وب‌هوک و ثبت inbox |
| GET | `/api/integrations/v1/connections?shopId=...` | فهرست اتصال‌های همان scope |
| POST | `/api/integrations/v1/connections` | ثبت اتصال بدون credential |
| POST | `/api/integrations/v1/connections/{id}/enable` | فعال‌سازی |
| POST | `/api/integrations/v1/connections/{id}/disable` | غیرفعال‌سازی |
| POST | `/api/integrations/v1/connections/{id}/webhooks/{inboxId}/replay` | بازپخش وب‌هوک |
| GET | `/api/integrations/v1/connections/{id}/mappings` | فهرست mapping کالا |
| POST | `/api/integrations/v1/connections/{id}/mappings` | ایجاد mapping |
| DELETE | `/api/integrations/v1/connections/{id}/mappings/{mappingId}` | غیرفعال‌سازی mapping |
| POST | `/api/integrations/v1/connections/{id}/sync` | شروع sync |
| GET | `/api/integrations/v1/dashboard?shopId=...` | metrics و runهای Integration |

Credential و token در هیچ DTO این API ارسال یا بازگردانده نمی‌شود. OAuth و Vault مسیر مالک مدیریت credential هستند.

## هویت اتصال و توکن در scope

کلید یکتای اتصال `(ShopId, TenantId, Provider, AccountIdentifier)` است. ثبت همان
غرفه برای tenant دیگر مستقل است؛ ثبت تکراری در همان scope رد می‌شود. اتصال تازه
غیرفعال است و callback موفق OAuth اتصال منطبق را فعال می‌کند.

توکن OAuth با `(ShopId, TenantId, Provider)` یکتا است. callback و تمدید فقط توکن
همان tenant را پیدا و به‌روزرسانی می‌کنند. محدودیت یک غرفه برای هر shop/tenant/provider
حفظ می‌شود؛ اتصال غرفه دیگر در همان scope نیازمند تعیین تکلیف اتصال قبلی است.

`docs/schema/ensure-integration-database.sql` کلیدهای قدیمی اتصال و توکن را داخل
تراکنش ارتقا می‌دهد. `docs/schema/ensure-external-oauth-tokens.sql` همان ارتقای
index توکن را مستقل انجام می‌دهد. اجرای دوباره داده‌ها را حفظ می‌کند. پیش از
استقرار کد جدید، schema دیتابیس مستقل Integration باید ارتقا داده شود.

آزمون SQL و callback این رفتار در `tools/IntegrationRegistryChecks` قرار دارد؛
این ابزار دیتابیس موقت خودش را ایجاد و پس از اجرا حذف می‌کند.

## کدهای پذیرش

- `202`: وب‌هوک یا replay در صف ثبت شد.
- `200`: فهرست، dashboard یا عملیات موفق.
- `201`: اتصال ایجاد شد.
- `400`: قرارداد یا scope نامعتبر.
- `401/403`: احراز هویت یا claim scope معتبر نیست.
- `404`: connection/inbox/mapping متعلق به scope نیست یا وجود ندارد.
- `409`: mapping یا connection تکراری.
- `422`: provider/protocol پشتیبانی نمی‌شود.
# Integration API v1 — مرز ورودی و مدیریت اتصال

`Hyper.Integration.Api` تنها ورودی رویدادهای خارجی و مالک مدیریت اتصال/توکن است.
این API سند حسابداری یا جدول‌های `TBL_*` را مستقیماً تغییر نمی‌دهد.

## جریان رویداد

```text
Provider webhook → Inbox + Audit → IntegrationScenarioJobs → Worker
                                                     ├→ Provider adapter
                                                     └→ Hyperyek.Accounting.Api
```

## ورودی webhook

```text
POST /api/integrations/v1/webhooks/{provider}/{connectionKey}
```

پروایدرهای قابل دریافت شامل `Basalam` و `Hyperyek` هستند. درخواست معتبر در یک
ذخیره‌سازی، هم در `IntegrationWebhookInbox` ثبت می‌شود و هم برای event typeهای
شناخته‌شده job ایجاد می‌کند. کلید یکتای `(ConnectionId, ExternalEventId)` از اثر
تکراری جلوگیری می‌کند.

نگاشت event به صف:

| event type | سناریو |
|---|---|
| `product.*` | Product |
| `inventory.*`, `stock.*` | Inventory |
| `customer.*` | Counterparty |
| `order.vendor.*`, `parcel.created` | Sale |
| `order.customer.*` | Purchase |

## مدیریت توکن

```text
POST   /api/integrations/v1/tokens/requests
GET    /api/integrations/v1/tokens
DELETE /api/integrations/v1/tokens/{connectionId}
```

هیچ endpointی access token یا refresh token را برنمی‌گرداند. درخواست توکن فقط
شناسهٔ درخواست و وضعیت را برمی‌گرداند؛ callback/provider flow در Infrastructure
انجام می‌شود و توکن رمزنگاری‌شده در دیتابیس Integration نگهداری می‌شود.

## مرز حسابداری

اعمال طرف‌حساب، رزرو، سفارش، سند حسابداری، لغو و مرجوعی از طریق قرارداد مستقل
`Hyperyek.Accounting.Api` انجام می‌شود و بخشی از `Hyper.Integration.Api` نیست.
