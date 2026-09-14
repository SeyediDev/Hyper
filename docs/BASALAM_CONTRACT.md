# قرارداد پیاده‌سازی باسلام

منبع رسمی بررسی‌شده در 2026-09-10:

- https://github.com/basalam/python-sdk/blob/main/openapi_data/core.json
- https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/core/client.py
- https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/config.py
- https://github.com/basalam/python-sdk/blob/main/src/basalam_sdk/auth.py
- https://github.com/basalam/python-sdk/blob/main/openapi_data/webhook.json

## محدودهٔ موجود

Strategy باسلام برای OAuth2 access token و BearerToken فعال است. این موضوع به معنی تکمیل دریافت یا refresh توکن OAuth نیست. CredentialsJson در این مرحله ورودی JSON با access_token و token_type اختیاری Bearer می‌گیرد؛ حفاظت پایدار credential هنوز آیتم باز SEC/INT است.

آدرس ثابت HTTPS: `https://openapi.basalam.com`. redirect HTTP خودکار خاموش است. آدرس دلخواه از credential دریافت نمی‌شود. شناسه‌های غرفه، کالا و تنوع باید عدد صحیح مثبت با نمایش canonical باشند.

دریافت: `GET /v1/vendors/{vendor_id}/products` با page، per_page=100، variants_flatting=false و sort=id:asc. data به همراه total_page/page بررسی می‌شود؛ نبود metadata موجب ادامه تا صفحه خالی می‌شود. شناسه تکراری و پاسخ ناقص موفق تلقی نمی‌شوند. variant[] به اقلام جدا با همان product id و variant id تبدیل می‌شود؛ موجودی variant از stock و موجودی کالای ساده از inventory خوانده می‌شود. Price بدون تبدیل ارز به‌عنوان snapshot خارجی نگهداری می‌شود.

ارسال: `PATCH /v1/products/{product_id}` یا `/variations/{variation_id}`، فقط بدنه `{ "stock": N }`. عدد باید صحیح، غیرمنفی و در محدوده int باشد؛ موجودی کسری بدون قرارداد تبدیل واحد گرد نمی‌شود. موجودی صفر ارسال می‌شود. قبل از PATCH، جزئیات محصول GET شده و vendor.id و عضویت تنوع دوباره بررسی می‌شوند؛ نگاشت اشتباه به غرفهٔ دیگر قابل ارسال نیست.

429، 408 و 5xx قابل بازسعی‌اند؛ Retry-After رعایت می‌شود. بدنه خطای provider و credential وارد پیام خطا/داشبورد نمی‌شوند. 401 فعلاً خطای نهایی است؛ تا تکمیل refresh، موفقیت یا بازیابی صوری گزارش نمی‌شود.

## حدود تأیید

آزمون HTTP کنترل‌شده مسیر، paging، variant، صفر، مقدار کسری/منفی، مالکیت غرفه و خطاها را بررسی می‌کند. هیچ PATCH واقعی به غرفه انجام نشده است. برای پذیرش نهایی، حساب تست با توکن مجاز، فروش/خرید واقعی staging، بررسی واحد کالا و رفتار remote لازم است.

OpenAPI مدیریت webhook فیلد request_headers را دارد؛ این شواهد تأیید پروتکل HMAC تحویل نیست. رجیسترکردن webhook، شناخت payload رویدادهای واقعی و امنیت تحویل باسلام هنوز بازند. پروتکل hyper-hmac-v1 صرفاً Custom است.
