# Performance Optimizations

این فایل شامل بهینه‌سازی‌های عملکردی برای بهبود سرعت لود صفحات است.

## مشکل قبلی:
```
"لود صفحات خیلی کند شده. نمی تونه کش کنه ؟"
```

## راه‌حل‌های پیاده‌سازی شده:

### 1️⃣ **Response Compression** (فشرده‌سازی پاسخ)

#### Brotli Compression:
- الگوریتم فشرده‌سازی مدرن و کارآمد
- تا 20-25% بهتر از Gzip
- پشتیبانی در تمام مرورگرهای مدرن

#### Gzip Compression:
- Fallback برای مرورگرهای قدیمی
- فشرده‌سازی سریع و مؤثر

#### فایل‌های فشرده شده:
- CSS, JavaScript
- JSON, XML, HTML
- SVG, Fonts (WOFF, WOFF2)

**مزایا:**
- کاهش 70-80% حجم فایل‌های CSS/JS
- کاهش زمان انتقال داده
- صرفه‌جویی در bandwidth

### 2️⃣ **Static Files Caching** (کش کردن فایل‌های استاتیک)

#### Cache Headers:
```http
Cache-Control: public, max-age=31536000, immutable
Expires: [1 year from now]
ETag: "timestamp-filesize"
Vary: Accept-Encoding
```

#### استراتژی Caching:

**Production:**
- Cache duration: **365 روز** (1 سال)
- `immutable`: مرورگر هرگز revalidate نمی‌کند

**Development:**
- Cache duration: **10 دقیقه**
- برای تست و توسعه مناسب

**HTML Files:**
- `Cache-Control: no-cache, no-store, must-revalidate`
- همیشه fresh content

#### ETag Support:
- Conditional requests (If-None-Match)
- برگرداندن `304 Not Modified` اگر فایل تغییر نکرده
- کاهش bandwidth و زمان لود

### 3️⃣ **Security Headers**

```http
X-Content-Type-Options: nosniff
```
- جلوگیری از MIME type sniffing attacks

## نتایج قابل انتظار:

### بدون Optimization:
- **JS Bundle**: 500 KB (بدون فشرده‌سازی)
- **CSS Bundle**: 200 KB (بدون فشرده‌سازی)
- **هر بار**: دانلود کامل فایل‌ها
- **زمان لود**: ~2-3 ثانیه (بسته به سرعت اینترنت)

### با Optimization:
- **JS Bundle**: ~150 KB (70% کاهش با Brotli)
- **CSS Bundle**: ~50 KB (75% کاهش با Brotli)
- **بار اول**: دانلود فایل‌های فشرده
- **بارهای بعدی**: `304 Not Modified` (0 byte download!)
- **زمان لود**: ~0.3-0.5 ثانیه (بار اول)، ~0.1 ثانیه (بارهای بعدی)

## نکات مهم:

### ⚠️ Cache Busting:
فایل‌های استاتیک با `asp-append-version="true"` به صورت خودکار versioning می‌شوند:

```html
<!-- قبل -->
<script src="/Scripts/app.js"></script>

<!-- بعد -->
<script src="/Scripts/app.js?v=abc123xyz"></script>
```

وقتی فایل تغییر کند، `v=` جدید می‌شود و مرورگر فایل جدید را دانلود می‌کند.

### 🔧 تنظیمات مرورگر:
اگر در Development تغییراتی ایجاد کردید و نمی‌بینید:

**Chrome/Edge:**
1. `Ctrl + Shift + R` (Hard Reload)
2. یا Developer Tools → Network → Disable cache

**Firefox:**
1. `Ctrl + F5` (Hard Reload)
2. یا Developer Tools → Network → Disable cache

### 📊 بررسی عملکرد:

**1. بررسی Compression:**
```bash
curl -H "Accept-Encoding: br,gzip" https://localhost:44301/Scripts/app.js -I
```

باید ببینید:
```
Content-Encoding: br  (یا gzip)
```

**2. بررسی Caching:**
```bash
curl https://localhost:44301/Scripts/app.js -I
```

باید ببینید:
```
Cache-Control: public,max-age=31536000,immutable
ETag: "..."
```

**3. بررسی 304 Response:**
بار دوم با همان فایل:
```bash
curl -H "If-None-Match: [etag از response قبلی]" https://localhost:44301/Scripts/app.js -I
```

باید ببینید:
```
HTTP/1.1 304 Not Modified
```

## ترتیب Middleware (مهم!):

```csharp
// 1. Compression (ابتدا فشرده‌سازی)
app.UseResponseCompression();

// 2. Static Files (با caching headers)
app.UseStaticFiles(/* با headers */);

// 3. Routing
app.UseRouting();

// 4. Authentication
app.UseAuthentication();

// 5. Response Caching (برای dynamic content)
app.UseResponseCaching();

// 6. Endpoints
app.UseEndpoints(...);
```

## مانیتورینگ:

### Chrome DevTools:
1. Network tab → Disable cache را خاموش کنید
2. Refresh کنید و ببینید:
   - **Size**: حجم فایل (مثلاً `50 KB`)
   - **Transferred**: حجم انتقال داده (مثلاً `15 KB` با compression)
3. Refresh دوم را بزنید:
   - **Status**: `304 Not Modified`
   - **Transferred**: `0 B (from disk cache)`

### Browser Performance:
1. F12 → Performance tab
2. Record → Refresh → Stop
3. بررسی کنید:
   - **Load time**: باید < 1 second باشد
   - **DOMContentLoaded**: باید < 500ms باشد

## مزایای کلی:

✅ **سرعت بارگذاری**: تا 80% سریعتر  
✅ **Bandwidth**: تا 75% کاهش مصرف  
✅ **Server Load**: کاهش چشمگیر با 304 responses  
✅ **User Experience**: لود فوری صفحات بعدی  
✅ **SEO**: بهبود رتبه در Google (Page Speed Score)  
✅ **Mobile**: بهبود قابل توجه در شبکه‌های کند  


