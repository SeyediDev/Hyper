# 🧪 راهنمای تست Customer Portal

## 🎯 **مرحله 1: بررسی سرویس‌ها**

### روش اول: اسکریپت خودکار
```powershell
cd D:\Projects\Hyper\Backend
.\test-services.ps1
```

### روش دوم: دستی با cURL
```powershell
# Test Customer Portal API
curl http://localhost:5000/health

# Test Admin Panel
curl http://localhost:5001

# Test Frontend
curl http://localhost:3000
```

### روش سوم: Browser
- باز کردن: http://localhost:5000/swagger
- باید Swagger UI نمایش داده شود

---

## 🔧 **مرحله 2: تست API با Swagger**

### 1. باز کردن Swagger UI
```
http://localhost:5000/swagger
```

### 2. تست endpoint های اصلی:

#### ✅ **Health Check**
```http
GET /health
```
انتظار: Status 200 + "Healthy"

#### ✅ **دریافت لیست محصولات**
```http
GET /api/products
```

#### ✅ **دریافت اطلاعات مشتری**
```http
GET /api/customer/profile
```
نیاز به Authentication: Bearer Token

---

## 🌐 **مرحله 3: تست Frontend**

### 1. باز کردن صفحه اصلی
```
http://localhost:3000
```

### 2. چک‌لیست تست UI:

- [ ] صفحه Home لود می‌شود
- [ ] لوگو و Header نمایش داده می‌شود
- [ ] دکمه‌های Navigation کار می‌کنند
- [ ] فرم Login/Register نمایش داده می‌شود

### 3. تست Developer Tools:

**باز کردن Chrome DevTools (F12):**

1. **Console Tab:**
   - نباید error قرمز وجود داشته باشد
   - اگر warning زرد هست، مهم نیست

2. **Network Tab:**
   - رفرش صفحه (F5)
   - بررسی API calls:
     - باید درخواست‌ها به `http://localhost:5000` بروند
     - Status Code باید 200 باشد

---

## 🔐 **مرحله 4: تست Authentication**

### با Swagger:

1. در Swagger UI، دکمه **Authorize** را کلیک کنید
2. یک JWT Token test وارد کنید (یا از endpoint Login دریافت کنید)

### دریافت Token:

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test@123"
}
```

انتظار: دریافت `accessToken`

### استفاده از Token:

```http
GET /api/customer/profile
Authorization: Bearer {your-token}
```

---

## 📮 **مرحله 5: تست با Postman**

### نصب Collection:

1. باز کردن Postman
2. Import → Link
3. وارد کردن: `http://localhost:5000/swagger/v1/swagger.json`

### تست Basic Endpoints:

#### 1. Health Check
```
GET http://localhost:5000/health
```

#### 2. Get Products (بدون Auth)
```
GET http://localhost:5000/api/products
```

#### 3. Login
```
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "mobileNumber": "09123456789",
  "password": "Test@123"
}
```

#### 4. Get Profile (با Auth)
```
GET http://localhost:5000/api/customer/profile
Authorization: Bearer {{token}}
```

---

## 🎨 **مرحله 6: تست E2E در Frontend**

### سناریو 1: مشاهده محصولات

1. باز کردن http://localhost:3000
2. رفتن به صفحه Products
3. بررسی نمایش لیست محصولات
4. کلیک روی یک محصول
5. بررسی صفحه جزئیات

### سناریو 2: ثبت‌نام و ورود

1. کلیک روی "ثبت‌نام"
2. پر کردن فرم:
   ```
   نام: علی
   شماره موبایل: 09123456789
   رمز عبور: Test@123
   ```
3. Submit فرم
4. بررسی پیام موفقیت
5. ورود با همان اطلاعات

### سناریو 3: مشاهده امتیازات

1. ورود به سیستم
2. رفتن به "امتیازات من"
3. بررسی نمایش موجودی امتیاز
4. بررسی لیست تراکنش‌ها

---

## 🐛 **Troubleshooting**

### ❌ سرویس بالا نمی‌آید

```powershell
# بررسی port در استفاده است یا نه
netstat -ano | findstr :5000

# Kill کردن process
taskkill /F /PID {PID}

# شروع مجدد
cd D:\Projects\Hyper\Backend\src\CustomerPortal\Hyper.CustomerPortal.Api
dotnet run --urls=http://localhost:5000
```

### ❌ خطای CORS

در browser console اگر دیدید:
```
Access to fetch at 'http://localhost:5000/api/...' from origin 'http://localhost:3000' has been blocked by CORS
```

**راه‌حل:**
بررسی `appsettings.Development.json`:
```json
"CustomerPortal": {
  "AllowedOrigins": "http://localhost:3000"
}
```

### ❌ خطای Database

```
Microsoft.Data.SqlClient.SqlException: Cannot open database
```

**راه‌حل:**
```powershell
# بررسی SQL Server در حال اجرا است
sqlcmd -S localhost -Q "SELECT @@VERSION"

# اجرای Migration
cd D:\Projects\Hyper\Backend\src\CustomerPortal\Hyper.CustomerPortal.Api
dotnet ef database update
```

### ❌ Frontend لود نمی‌شود

```powershell
cd D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web

# پاک کردن cache
rm -rf .next
rm -rf node_modules
npm install
npm run dev
```

---

## 📊 **ابزارهای مفید تست**

### 1. **Browser DevTools** (F12)
- Console: برای خطاها
- Network: برای API calls
- Application: برای localStorage/cookies

### 2. **Postman**
- تست API endpoints
- ذخیره Collections
- Environment variables

### 3. **cURL**
```powershell
# Simple GET
curl http://localhost:5000/api/products

# POST with JSON
curl -X POST http://localhost:5000/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{\"mobileNumber\":\"09123456789\",\"password\":\"Test@123\"}'

# با Authorization
curl http://localhost:5000/api/customer/profile `
  -H "Authorization: Bearer YOUR_TOKEN"
```

### 4. **SQL Server Management Studio**
- بررسی مستقیم Database
- چک کردن data
- اجرای queries

---

## ✅ **چک‌لیست تست کامل**

### Backend (API)
- [ ] Health endpoint پاسخ می‌دهد
- [ ] Swagger UI لود می‌شود
- [ ] Login endpoint کار می‌کند
- [ ] Token دریافت می‌شود
- [ ] Protected endpoints با Token قابل دسترسی هستند
- [ ] CORS برای localhost:3000 فعال است

### Frontend
- [ ] صفحه اصلی لود می‌شود
- [ ] Navigation کار می‌کند
- [ ] API calls به backend می‌روند
- [ ] خطای CORS وجود ندارد
- [ ] فرم‌ها submit می‌شوند
- [ ] پیام‌های خطا/موفقیت نمایش داده می‌شوند

### Database
- [ ] SQL Server در حال اجرا است
- [ ] Database ایجاد شده
- [ ] جداول موجود هستند
- [ ] Seed data وجود دارد (اختیاری)

### Integration
- [ ] Frontend با Backend ارتباط دارد
- [ ] Authentication flow کار می‌کند
- [ ] Data correctness (داده‌ها صحیح نمایش داده می‌شوند)

---

## 🚀 **تست‌های پیشرفته**

### Performance Testing
```powershell
# نصب Apache Bench
choco install apache-httpd

# تست 100 request
ab -n 100 -c 10 http://localhost:5000/api/products
```

### Load Testing
```javascript
// با k6
import http from 'k6/http';

export default function () {
  http.get('http://localhost:5000/api/products');
}
```

---

## 📝 **نمونه Test Data**

### کاربر تست:
```json
{
  "mobileNumber": "09123456789",
  "password": "Test@123",
  "firstName": "علی",
  "lastName": "احمدی"
}
```

### محصول تست:
```json
{
  "name": "محصول تستی",
  "price": 100000,
  "points": 50
}
```

---

## 🎓 **نکات مهم**

1. **همیشه از Development environment استفاده کنید**
2. **Logs را چک کنید** (در console سرویس‌ها)
3. **Browser DevTools باز باشد** (F12)
4. **Port conflicts را بررسی کنید**
5. **Database connection string را چک کنید**

---

**موفق باشید!** 🎉

نیاز به کمک بیشتر؟ به `DEV-START-GUIDE.md` مراجعه کنید.



