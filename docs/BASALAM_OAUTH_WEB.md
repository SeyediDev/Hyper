# اتصال مغازه به باسلام از پنل — 2026-09-14

این سند مرجع سناریوی وب است. گزارش‌های OAuth در ریشه Backend و راهنمای قدیمی کنار سرویس، گزارش تاریخی هستند؛ ادعای «production-ready» در آن‌ها تأیید نشده است.

## مسیر کاربر

1. پروژه Hyper.AdminPanel.Web را اجرا کنید؛ در پروفایل Hyper.Bpms.Web آدرس http://localhost:5000 است.
2. با ادمین وارد شوید و از منوی «مغازه‌دار و اتصال» به /MerchantSimulation/Index بروید.
3. مغازه را جست‌وجو کنید و «فعال‌کردن این زمینه» را بزنید. User ادمین صاحب مغازه فرض نمی‌شود.
4. نام مغازه را بررسی کنید و «ورود به باسلام و اعطای مجوز» را بزنید.
5. در باسلام با حساب صاحب غرفه وارد شوید و مجوزها را تأیید کنید.
6. باسلام مرورگر را به /api/auth/basalam/callback با code و state برمی‌گرداند.
7. سرور code را با توکن معاوضه می‌کند؛ توکن در URL مرورگر نیست. شناسه غرفه از GET https://core.basalam.com/v3/users/me و vendor.id خوانده می‌شود.
8. توکن رمز‌شده و اتصال برای همان ShopId/TenantId ذخیره می‌شود. پنل نام غرفه، شناسه و زمان دریافت/انقضا را نشان می‌دهد؛ خود توکن نمایش داده نمی‌شود.

## مسئولیت فایل‌ها

- Views/MerchantSimulation/Index.cshtml: انتخاب زمینه، دکمه POST و نمایش نتیجه و کمبود تنظیمات.
- MerchantSimulationController.Index: گرفتن زمینه معتبر ادمین و وضعیت توکن همان مغازه.
- OAuthCallbackController.Login: GetUser/IsAdmin مطابق Neo، antiforgery، تطبیق بلیت فرم با cookie انتخاب فعلی، ثبت IntegrationTokenRequests و redirect واقعی.
- BasalamOAuthService: قرارداد HTTP باسلام، state حفاظت‌شده با عمر ۱۰ دقیقه، تبادل code، تشخیص غرفه و حفاظت access/refresh token.
- OAuthCallbackController.Callback: بررسی cookie همبستگی مرورگر و state، بررسی اعتبار زمینه، مصرف یک‌باره درخواست، ذخیره و redirect به صفحه شبیه‌ساز.
- BasalamOAuthStore: عملیات SQL و EF روی HyperContextCommand موجود Neo؛ DbContext جدیدی اضافه نشده است. تراکنش اتصال/توکن/نتیجه یک‌جا commit می‌شود.
- ExternalOAuthTokenConfiguration: نگاشت جدول جدید dbo.ExternalOAuthTokens.

وضعیت درخواست‌ها: 0 آماده، 1 در حال پردازش و مصرف‌شده، 2 ذخیره موفق، 3 عدم اعطای مجوز، 4 شکست.
بازگشت تکراری، state دستکاری‌شده، مرورگر دیگر و زمینه پایان‌یافته قابل مصرف نیستند. شروع جدید cookie درخواست قبلی همان مرورگر را جایگزین می‌کند.
Session یا IMemoryCache در این جریان استفاده نمی‌شود. state حفاظت‌شده است و رکورد SQL درخواست، مصرف یک‌باره را کنترل می‌کند.
callback به cookie ورود ادمین وابسته نیست؛ درخواست اولیه احراز شده و callback به cookie مخصوص مرورگر و زمینه معتبر ثبت‌شده محدود است. بعد از بازگشت اگر نشست ادمین منقضی باشد، مشاهده صفحه نیاز به ورود مجدد دارد.

## تنظیمات لازم

بخش Basalam در پیکربندی میزبان:
- ClientId و ClientSecret: مقادیر واقعی برنامه ثبت‌شده در باسلام؛ مقادیر your-client-* نمونه‌اند. secret را در user-secrets یا متغیر محیطی Basalam__ClientSecret بگذارید، نه در Git.
- AuthorizationEndpoint: https://basalam.com/accounts/sso
- TokenEndpoint: https://auth.basalam.com/oauth/token
- RedirectUri: آدرس دقیق ثبت‌شده برای برنامه؛ برای پروفایل محلی http://localhost:5000/api/auth/basalam/callback. پذیرش localhost را در تنظیمات برنامه باسلام بررسی کنید؛ در صورت نیاز از دامنه HTTPS قابل دسترس استفاده کنید.
- Scopes: این اتصال فقط `vendor.profile.read` را درخواست می‌کند؛ همین مجوز برای خواندن `vendor.id` از `GET /v3/users/me` کافی است. مجوزهای محصول/سفارش موردنیاز را پس از تأیید در برنامه، مانند `vendor.product.read vendor.product.write vendor.parcel.read` اضافه کنید. نام‌های `inventory.read`/`orders.read`/`products.read` در تنظیمات قدیمی معتبر فرض نمی‌شوند.
- UsePkce: پیش‌فرض false مطابق جریان confidential-client در SDK رسمی. روشن‌کردن منوط به تأیید پشتیبانی برنامه باسلام است.
- تعویض code با token طبق مستند رسمی باسلام با POST و `Content-Type: application/json` به `https://auth.basalam.com/oauth/token` انجام می‌شود؛ secret فقط روی سرور ارسال می‌شود.
- در چند نمونه یا بعد از تعویض سرور، key ring مشترک و پایدار ASP.NET Data Protection با دسترسی محدود لازم است؛ کلیدها برای بازکردن توکن ذخیره‌شده ضروری‌اند.
- اجرای پشت reverse proxy باید HTTPS و forwarded headers صحیح داشته باشد تا cookie امن تولید شود.

## دیتابیس

ساختار legacy تغییر نمی‌کند. اسکریپت docs/schema/ensure-external-oauth-tokens.sql فقط جدول جدید توکن را اگر وجود نداشته باشد ایجاد می‌کند.
اسکریپت را روی Hyperyek اجرا کنید؛ روی جدول موجود ALTER/DROP اجرا نمی‌کند.
Migration و snapshot دستی قدیمی با configuration اختلاف نوع Provider، TenantId و UpdatedAtUtc داشتند. برای این تحویل از اسکریپت محدود استفاده کنید؛ snapshot کامل Neo قبل از migration عمومی باید تطبیق داده شود.
نسخه مرکزی EF فعلی 10.0.8 است؛ ادعای قدیمی نیاز به ارتقای EF از 8 مبنای تغییر بسته‌ها نیست.

## مرز این تحویل

دریافت و ذخیره توکن با فعال‌سازی worker یکی نیست. اتصال OAuth جدید/تمدیدشده غیرفعال می‌ماند؛ هنگام اجرای اتصال فعال، adapter توکن رمز‌شده را از `ExternalOAuthTokens` می‌خواند و در آستانهٔ انقضا refresh می‌کند.
جدول فعلی تنها یک توکن به ازای ShopId/Provider می‌پذیرد؛ اتصال غرفه دوم برای همان مغازه رد می‌شود تا مدل چندغرفه‌ای جداگانه تصمیم‌گیری شود.
تجدید خودکار، لغو دسترسی و تست واقعی با حساب باسلام هنوز معیار پذیرش جداگانه دارند.

## منابع رسمی مرورشده

- https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/auth.py
- https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/config.py
- https://github.com/basalam/python-sdk/blob/main/openapi_data/core.json

## اعتبارسنجی

وضعیت build و آزمون‌های نهایی در BACKLOG.md ثبت می‌شود. وجود کد یا موفقیت build به‌تنهایی به معنی دریافت واقعی توکن نیست.

### نتیجه اعتبارسنجی OAUTH-WEB-215
- build پس از اصلاح: موفق، صفر خطا و صفر هشدار.
- ابزار tools/OAuthChecks: تعداد ۱۹ آزمون موفق با HTTP ساختگی؛ state مخدوش، منقضی، مرورگر دیگر، تغییر redirect، PKCE اختیاری، رمزنگاری، شناسایی غرفه و پاسخ‌های نامعتبر.
- اتصال واقعی SQL به Hyperyek تأیید شد. جدول ExternalOAuthTokens وجود نداشت؛ با اسکریپت محدود ساخته شد و تعداد توکن واقعی همچنان صفر است.
- میزبان Development روی http://localhost:5000 اجرا شد. ورود حساب تست، صفحه اصلی Neo، بازکردن شبیه‌ساز، انتخاب مغازه و نمایش کمبود تنظیمات در مرورگر تأیید شد. زمینه آزمایشی با Clear پایان یافت.
- درخواست مهمان به شبیه‌ساز به ورود هدایت شد؛ callback فاقد state به شبیه‌ساز هدایت شد. از callback نامعتبر توکنی تولید نشد.
- مانع آزمون واقعی: ClientId و ClientSecret هنوز نمونه‌اند؛ نیاز به برنامه باسلام و ثبت RedirectUri دقیق است.
- این نتایج ادعای دریافت واقعی توکن یا آماده‌بودن worker با توکن رمز‌شده نیست.
- چهار بررسی SQL با query استخراج‌شده از TryClaimAsync موفق شد: ادمین نادرست رد شد، مصرف اول موفق بود، replay رد شد و زمینه پایان‌یافته رد شد. همه رکوردهای آزمایشی rollback شدند. مجموع بررسی‌های کنترل‌شده: ۲۳.
## فرمان اجرای محلی بررسی‌شده

از پوشه Backend در PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project src/AdminPanel/Hyper.AdminPanel.Web/Hyper.AdminPanel.Web.csproj --no-build --no-launch-profile -- --urls http://localhost:5000 --HyperLocalSql:Enabled=true
```

برای تنظیم ClientSecret از Manage User Secrets پروژه وب استفاده کنید یا متغیر محیطی Basalam__ClientSecret را در میزبان قرار دهید. بعد از تغییر تنظیمات، پنل را دوباره اجرا کنید. secret را در چت یا Git قرار ندهید.
اگر آدرس یا پورت دیگری انتخاب می‌کنید، Basalam:RedirectUri و redirect ثبت‌شده در برنامه باسلام را دقیقاً یکسان کنید.
### AUTH-216 — انتخاب پلتفرم
- کاتالوگ Infrastructure مسیر شروع و نوع اعتبارنامه هر پلتفرم را مشخص می‌کند؛ گزینه ناشناخته رد می‌شود.
- شبیه‌ساز انتخاب باسلام/دیجی‌کالا/ترب/سامانه دیگر را دارد؛ مسیر باسلام همان POST با antiforgery و زمینه مغازه است.
- پلتفرم بدون آداپتر مجوز غیرفعال و علت آن نمایش داده می‌شود؛ هیچ گزینه‌ای به اشتباه وارد باسلام نمی‌شود.
- ثبت توکن واقعی باسلام همچنان به ClientId/ClientSecret و redirect ثبت‌شده وابسته است.
