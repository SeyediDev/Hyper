# Hyper Platform Icons

این دایرکتوری شامل آیکن‌های اختصاصی پلتفرم باشگاه مشتریان می‌باشد.

## 📁 فایل‌ها

- **`custom-sprite.svg`**: فایل SVG Sprite حاوی تمام آیکن‌های پلتفرم
- **`custom-menu-icons.css`**: استایل‌های رنگی و انیمیشن آیکن‌ها
- **`README.md`**: این فایل (راهنمای استفاده)

## 🎨 رنگ‌بندی آیکن‌ها

آیکن‌ها بر اساس دسته‌بندی عملکردی رنگ‌بندی شده‌اند:

| دسته | رنگ | کد HEX | کاربرد |
|------|-----|--------|--------|
| **Dashboard** | Indigo | `#6366F1` | داشبورد و صفحه اصلی |
| **Customer** | Emerald | `#10B981` | مدیریت مشتریان |
| **Product** | Amber | `#F59E0B` | محصولات و دارایی‌ها |
| **Scoring** | Red | `#EF4444` | سیستم امتیازدهی |
| **Event** | Purple | `#8B5CF6` | مدیریت رویدادها |
| **Promotion** | Pink | `#EC4899` | پویش‌ها و قرعه‌کشی |
| **Settings** | Slate | `#64748B` | تنظیمات سامانه |
| **Report** | Sky | `#0EA5E9` | گزارشات |

## 📋 لیست آیکن‌ها

### Dashboard & Home
- `home-dashboard` - آیکن خانه/داشبورد

### Customer Management (Emerald)
- `building-organization` - اکوسیستم
- `user-circle` - مشتری
- `users-group` - جامعه مشتریان
- `user-settings` - پارامترهای مشتری
- `user-badge` - عضویت/نشان مشتری
- `sliders-h` - تنظیمات/پارامترها
- `wallet-money` - کیف پول/تراکنش
- `trophy-star` - سطوح امتیاز

### Products & Assets (Amber)
- `box-package` - محصول/بسته
- `coins-money` - هزینه/پول
- `cube-3d` - دارایی/شیء سه‌بعدی
- `grid-layout` - طبقه‌بندی/شبکه
- `store-shop` - فروشگاه/ارائه‌دهنده

### Scoring System (Red)
- `star-badge` - ستاره/امتیاز
- `medal-reward` - مدال/جایزه
- `piggy-bank` - قلک/بودجه
- `rule-checklist` - قوانین/چک‌لیست
- `flag-trigger` - پرچم/تریگر
- `bolt-lightning` - صاعقه/اکشن

### Events (Purple)
- `calendar-event` - تقویم/رویداد
- `cog-settings` - چرخ‌دنده/تنظیمات
- `rss-signal` - سیگنال/منبع
- `list-checklist` - لیست/لاگ

### Promotions (Pink)
- `gift-present` - هدیه/پویش
- `ticket-lottery` - بلیط/قرعه‌کشی

### Settings (Slate)
- `user-admin` - مدیر/کاربر
- `question-circle` - سوال/راهنما
- `info-circle` - اطلاعات/کمک
- `file-document` - فایل/سند
- `cog-wheel` - چرخ‌دنده/تنظیمات

### Reports (Sky)
- `chart-bar` - نمودار میله‌ای/گزارش

### Default
- `default-icon` - آیکن پیش‌فرض (دایره‌ای با علامت تعجب)

## 🔧 نحوه استفاده

### در کد C# (Menu_Hyper.cs)
```csharp
// استفاده مستقیم با نام آیکن
AddMenu("داشبورد", "home-dashboard", "Dashboard", "");
AddMenu<Customer>("مشتریان", "user-circle");
AddReport<Product>("گزارش محصولات", "box-package");
```

### در HTML
```html
<svg class="sidemenu-icon" viewBox="0 0 24 24">
    <use xlink:href="/Content/custom-icons/custom-sprite.svg#user-circle"></use>
</svg>
```

### در CSS
```css
/* رنگ‌بندی خودکار بر اساس href */
.sidemenu-icon use[href*="user-circle"] {
    stroke: #10B981; /* Emerald */
}
```

## 🎯 اصول طراحی

1. **سازگاری**: تمام آیکن‌ها با `viewBox="0 0 24 24"` طراحی شده‌اند
2. **خط‌کشی**: استایل Outline با `stroke-width="2"`
3. **انیمیشن**: افکت Hover و Scale با Cubic-bezier
4. **دسترسی‌پذیری**: پشتیبانی از High Contrast و Reduced Motion
5. **پاسخ‌گو**: سایز‌های متفاوت برای موبایل و دسکتاپ
6. **RTL**: موقعیت‌یابی صحیح در راست‌چین

## 🔄 بروزرسانی

برای افزودن آیکن جدید:

1. آیکن SVG را به `custom-sprite.svg` اضافه کنید
2. نام آیکن را به `MenuHelper.UseHyperIcon()` اضافه کنید
3. رنگ مناسب را در `custom-menu-icons.css` تعریف کنید
4. نام و توضیحات را در این فایل README به روز کنید
5. در صورت نیاز، نام را در `IconRegistry.cs` ثبت کنید

## 📚 منابع

- **طراحی**: بر اساس Feather Icons و Lucide Icons
- **رنگ‌ها**: Tailwind CSS Color Palette
- **فونت**: SVG Scalable بدون وابستگی به Font

## 🐛 رفع مشکل

### آیکن نمایش داده نمی‌شود
- مسیر SVG sprite را بررسی کنید
- نام آیکن را با فایل `custom-sprite.svg` مطابقت دهید
- کنسول مرورگر را برای خطاهای 404 چک کنید

### رنگ آیکن درست نیست
- `custom-menu-icons.css` را بارگذاری شده است؟
- Selector های CSS را بررسی کنید
- Cache مرورگر را پاک کنید

### آیکن در سمت چپ نمایش داده می‌شود
- `dir="rtl"` روی container اعمال شده است؟
- `mmenu-rtl-fix.css` بارگذاری شده است؟
- CSS `float: right` در `custom-menu-icons.css` بررسی شود

---

**نسخه**: 1.0.0  
**تاریخ**: 2025-10-14  
**مخزن**: Hyper.Bpms Platform









