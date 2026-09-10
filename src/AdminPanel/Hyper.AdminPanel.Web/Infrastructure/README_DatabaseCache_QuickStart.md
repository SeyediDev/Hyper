# 🚀 Database Cache - راهنمای سریع فعال/غیرفعال‌سازی

## ❌ غیرفعال کردن کش (پیش‌فرض)

کش دیتابیس به صورت **پیش‌فرض غیرفعال** است.

در تمام فایل‌های `appsettings.*.json`:

```json
{
  "DatabaseCache": {
    "Enabled": false,  // ❌ کش غیرفعال
    "DefaultExpiration": 30
  }
}
```

**نتیجه**: تمام queryها مستقیماً به دیتابیس ارسال می‌شوند. هیچ caching نمی‌شود.

---

## ✅ فعال کردن کش

### مرحله 1: تغییر تنظیمات

فقط `Enabled` را به `true` تغییر دهید:

```json
{
  "DatabaseCache": {
    "Enabled": true,  // ✅ کش فعال
    "DefaultExpiration": 30  // مدت کش (دقیقه)
  }
}
```

### مرحله 2: Restart برنامه

```bash
# Development
docker-compose -f docker-compose.develop.yml restart

# یا
dotnet run
```

**همین!** 🎉

---

## 📊 بررسی وضعیت

### لاگ‌ها را چک کنید:

#### کش غیرفعال:
```
[Debug] Database Cache is DISABLED. Executing query directly: Product_1
```

#### کش فعال:
```
[Debug] Cache MISS for key: Product_1
[Debug] Cache HIT for key: Product_1
```

---

## 🔧 تنظیمات پیشرفته

```json
{
  "DatabaseCache": {
    "Enabled": true,
    "DefaultExpiration": 30,      // مدت پیش‌فرض کش (دقیقه)
    "SlidingExpiration": 10,      // مدت Sliding (دقیقه)
    "MaxCacheSize": 1024          // حداکثر حجم کش (MB)
  }
}
```

---

## 💡 نحوه استفاده در کد

### روش 1: با Extension Method

```csharp
// Cache کردن یک query
var product = await _context.Products
    .Where(p => p.Id == id)
    .FirstOrDefaultAsync()
    .CachedAsync(_cache, $"Product_{id}");

// Cache کردن لیست
var products = await _context.Products
    .Where(p => p.IsActive)
    .CachedListAsync(_cache, "Products_Active");
```

### روش 2: با IDatabaseCache مستقیم

```csharp
public class ProductService
{
    private readonly IDatabaseCache _cache;
    private readonly AppDbContext _context;

    public ProductService(IDatabaseCache cache, AppDbContext context)
    {
        _cache = cache;
        _context = context;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        return await _cache.GetOrSetAsync(
            key: $"Product_{id}",
            factory: async () => await _context.Products.FindAsync(id),
            absoluteExpiration: TimeSpan.FromMinutes(30)
        );
    }
}
```

---

## 🔄 Invalidation خودکار

کش به صورت **خودکار** invalidate می‌شود:

```csharp
// Insert
_context.Products.Add(newProduct);
await _context.SaveChangesAsync();
// → Cache for Product_{id} automatically invalidated ✅

// Update
product.Name = "New Name";
await _context.SaveChangesAsync();
// → Cache for Product_{id} automatically invalidated ✅

// Delete
_context.Products.Remove(product);
await _context.SaveChangesAsync();
// → Cache for Product_{id} automatically invalidated ✅
```

**هیچ کد اضافی لازم نیست!** `CacheInvalidationInterceptor` این کار را خودکار انجام می‌دهد.

---

## 🧪 تست عملکرد

### قبل از فعال کردن کش:

```bash
# Query اول: 50ms
# Query دوم: 50ms
# Query سوم: 50ms
```

### بعد از فعال کردن کش:

```bash
# Query اول: 50ms (Cache MISS)
# Query دوم: 0.1ms (Cache HIT) ⚡
# Query سوم: 0.1ms (Cache HIT) ⚡
```

**500x سریع‌تر!** 🚀

---

## ⚠️ نکات مهم

### 1. کی باید کش رو فعال کنم؟

✅ **فعال کنید**:
- Query‌های تکراری زیاد
- Lookup tables که کم تغییر می‌کنند
- Traffic بالا
- Performance مهم است

❌ **فعال نکنید**:
- داده‌های Real-time
- تغییرات مکرر
- Memory محدود
- هنوز در حال توسعه و تست

### 2. چه زمانی غیرفعال کنم؟

- در حال Debug کردن مشکل
- می‌خواهید Stale Data نباشد
- تست عملکرد بدون کش
- Memory کم است

### 3. محیط‌های مختلف

```json
// Development - معمولاً غیرفعال
{
  "DatabaseCache": { "Enabled": false }
}

// Staging - برای تست
{
  "DatabaseCache": { "Enabled": true, "DefaultExpiration": 10 }
}

// Production - برای Performance
{
  "DatabaseCache": { "Enabled": true, "DefaultExpiration": 60 }
}
```

---

## 🆘 Troubleshooting

### مشکل: داده قدیمی نمایش می‌دهد

**راه‌حل 1**: کش را Clear کنید
```csharp
_cache.InvalidateByPattern("Product_*");
```

**راه‌حل 2**: کش را موقتاً غیرفعال کنید
```json
{ "DatabaseCache": { "Enabled": false } }
```

**راه‌حل 3**: مدت کش را کوتاه‌تر کنید
```json
{ "DatabaseCache": { "DefaultExpiration": 5 } }
```

### مشکل: Memory زیاد مصرف می‌کند

**راه‌حل**: محدودیت حجم کش
```json
{
  "DatabaseCache": {
    "Enabled": true,
    "MaxCacheSize": 512  // MB
  }
}
```

---

## 📚 مستندات کامل

- [README_DatabaseCaching.md](./README_DatabaseCaching.md) - راهنمای کامل
- [README_DistributedCaching.md](./README_DistributedCaching.md) - Distributed Cache برای Docker

---

## 🎯 خلاصه

| عمل | دستور |
|-----|-------|
| **غیرفعال کردن** | `"DatabaseCache": { "Enabled": false }` |
| **فعال کردن** | `"DatabaseCache": { "Enabled": true }` |
| **بررسی وضعیت** | لاگ‌ها را چک کنید |
| **Clear کردن** | `_cache.InvalidateByPattern("*")` |

---

**موفق باشید!** 🚀


