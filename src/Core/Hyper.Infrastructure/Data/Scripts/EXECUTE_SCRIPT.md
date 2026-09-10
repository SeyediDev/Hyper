# راهنمای اجرای اسکریپت SeedPointsDistributionData.sql

## روش 1: اجرا از طریق SQL Server Management Studio (SSMS)

1. **باز کردن SSMS** و اتصال به دیتابیس `HyperBpmsDev`

2. **باز کردن فایل SQL:**
   - File → Open → File
   - انتخاب فایل `SeedPointsDistributionData.sql`

3. **تنظیم مقادیر متغیرها:**
   - خط 12: `@TenantId` - شناسه اکوسیستم (معمولاً 1)
   - خط 13: `@CustomerTenantId` - شناسه مشتری (می‌توانید از query زیر استفاده کنید)
   - خط 14: `@EventLogId` - شناسه لاگ رویداد (می‌توانید از query زیر استفاده کنید)

4. **بررسی مقادیر موجود در دیتابیس:**
   ```sql
   -- دریافت TenantId
   SELECT TOP 1 Id, Name FROM Tenants WHERE IsDeleted = 0;
   
   -- دریافت CustomerTenantId
   SELECT TOP 1 Id, CustomerTenantId FROM CustomerTenants WHERE IsDeleted = 0;
   
   -- دریافت EventLogId
   SELECT TOP 1 Id FROM EventLogs ORDER BY Id DESC;
   ```

5. **اجرای اسکریپت:**
   - F5 یا Execute

## روش 2: اجرا از طریق PowerShell (اگر connection string درست باشد)

```powershell
cd D:\Projects\Hyper\Backend\src\Core\Hyper.Infrastructure\Data\Scripts
powershell -ExecutionPolicy Bypass -File RunSeedScript.ps1
```

**نکته:** قبل از اجرا، مطمئن شوید که:
- Connection string در `appsettings.Development.json` درست است
- رمز عبور SQL Server صحیح است
- SQL Server در حال اجرا است

## روش 3: اجرا از طریق Command Line (sqlcmd)

```cmd
sqlcmd -S localhost,1433 -d HyperBpmsDev -U sa -P "YourPassword" -i SeedPointsDistributionData.sql
```

## نتیجه

پس از اجرای موفقیت‌آمیز اسکریپت، نمودار Pie "توزیع امتیازات بر اساس نوع" باید 5 بخش مختلف نمایش دهد:

1. **XP باشگاه**: ~10,000,000 (بزرگترین)
2. **امتیاز خرید**: ~1,800,000
3. **امتیاز معرفی دوست**: ~800,000
4. **امتیاز نظرسنجی**: ~450,000
5. **امتیاز ارزش طول‌عمر**: ~1,200,000

## عیب‌یابی

اگر خطا دریافت کردید:

1. **Login failed**: رمز عبور یا نام کاربری اشتباه است
2. **Database not found**: نام دیتابیس را بررسی کنید
3. **Table not found**: مطمئن شوید که migration ها اجرا شده‌اند
4. **Foreign key constraint**: مقادیر TenantId, CustomerTenantId, EventLogId باید در جداول مربوطه وجود داشته باشند


