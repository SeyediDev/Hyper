# راهنمای اجرای اسکریپت اصلاح کدهای ISO استان‌ها

این اسکریپت کدهای ISO استان‌های ایران را در پایگاه داده با نقشه jVectorMap هماهنگ می‌کند.

## مشکل
- در نقشه ایران، برای قزوین اطلاعات قم نمایش داده می‌شود
- برای قم اطلاعات همدان نمایش داده می‌شود
- و سایر استان‌ها نیز به اشتباه نمایش داده می‌شوند

## راه حل
اسکریپت SQL کدهای ISO استان‌ها را بر اساس نام فارسی استان‌ها اصلاح می‌کند.

## روش اجرا

### روش 1: SQL Server Management Studio (SSMS)

1. SQL Server Management Studio را باز کنید
2. به پایگاه داده `HyperBpmsDev` متصل شوید
3. فایل `21_fix_province_iso2_mapping.sql` را باز کنید
4. کلید F5 را بزنید یا دکمه Execute را کلیک کنید
5. نتایج را بررسی کنید

### روش 2: Azure Data Studio

1. Azure Data Studio را باز کنید
2. به پایگاه داده `HyperBpmsDev` متصل شوید
3. فایل `21_fix_province_iso2_mapping.sql` را باز کنید
4. کلید F5 را بزنید یا دکمه Run را کلیک کنید

### روش 3: Command Line (با رمز عبور صحیح)

```powershell
cd "D:\Projects\Hyper\Backend\scripts"
sqlcmd -S localhost,1433 -d HyperBpmsDev -U sa -P "رمز_عبور_صحیح" -C -i "21_fix_province_iso2_mapping.sql"
```

### روش 4: PowerShell Script (نیاز به دسترسی)

```powershell
cd "D:\Projects\Hyper\Backend\scripts"
powershell -ExecutionPolicy Bypass -File "21_fix_province_iso2_mapping.ps1"
```

## خروجی اسکریپت

اسکریپت سه گزارش نمایش می‌دهد:

1. **وضعیت استان‌ها**: لیست تمام استان‌ها با کد ISO فعلی و صحیح
2. **استان‌های بدون mapping**: استان‌هایی که در mapping وجود ندارند
3. **کدهای ISO تکراری**: در صورت وجود کدهای تکراری

## پس از اجرا

پس از اجرای موفقیت‌آمیز اسکریپت:
- قزوین: IR-28 ✓
- قم: IR-26 ✓
- همدان: IR-24 ✓
- سایر استان‌ها نیز با نقشه هماهنگ می‌شوند

## نکات مهم

- قبل از اجرا، از پایگاه داده backup بگیرید
- اسکریپت فقط کدهای ISO را به‌روزرسانی می‌کند و داده‌های دیگر را تغییر نمی‌دهد
- در صورت وجود خطا، اسکریپت متوقف می‌شود و تغییرات rollback می‌شوند


