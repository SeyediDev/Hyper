-- مقداردهی فیلد ClientId در جدول EventChannel بر اساس فرمول: Key + '-client'
-- این اسکریپت برای به‌روزرسانی داده‌های موجود استفاده می‌شود

UPDATE EventChannel
SET ClientId = [Key] + N'-client'
WHERE ClientId IS NULL 
  AND [Key] IS NOT NULL 
  AND [Key] != N''
  AND IsDeleted = 0;

-- بررسی نتیجه
SELECT 
    Id,
    [Key],
    Title,
    ClientId,
    TenantId,
    IsDeleted
FROM EventChannel
WHERE IsDeleted = 0
ORDER BY Id;

