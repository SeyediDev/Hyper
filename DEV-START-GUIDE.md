# 🚀 راهنمای سریع شروع Development

## روش‌های اجرای سرویس‌ها

### 🎯 روش 1: Batch File (ساده‌ترین)

```cmd
# دابل کلیک روی این فایل:
D:\Projects\Hyper\Backend\start-services.bat
```

### 🎯 روش 2: PowerShell (پیشرفته)

```powershell
cd D:\Projects\Hyper\Backend
.\start-dev.ps1
```

### 🎯 روش 3: دستی (تک تک)

**Terminal 1 - Customer Portal API:**
```powershell
cd D:\Projects\Hyper\Backend\src\CustomerPortal\Hyper.CustomerPortal.Api
dotnet run --urls=http://localhost:5000
```

**Terminal 2 - Admin Panel:**
```powershell
cd D:\Projects\Hyper\Backend\src\AdminPanel\Hyper.AdminPanel.Web
dotnet run --urls=http://localhost:5001
```

**Terminal 3 - Frontend:**
```powershell
cd D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web
npm run dev
```

---

## 🌐 URL های سرویس‌ها

| سرویس | URL | توضیحات |
|-------|-----|---------|
| **Customer Portal API** | http://localhost:5000 | REST API |
| **Swagger UI** | http://localhost:5000/swagger | مستندات API |
| **Admin Panel** | http://localhost:5001 | پنل مدیریت |
| **Frontend** | http://localhost:3000 | صفحه مشتریان |

---

## 📋 پیش‌نیازها

### ✅ نصب شده:
- ✅ .NET 8.0 SDK
- ✅ Node.js 18+
- ✅ SQL Server (LocalDB یا Express)

### ⚙️ تنظیمات:

**1. Database:**
- Connection String در `appsettings.Development.json`:
```json
"HyperCommandConnection": "Server=localhost;Database=Hyperyek;Trusted_Connection=True;TrustServerCertificate=True;"
```

**2. CORS:**
- Frontend Origin تنظیم شده: `http://localhost:3000`

**3. JWT:**
- SecretKey موجود در appsettings

---

## 🔧 Troubleshooting

### ❌ خطای Port Already in Use

```powershell
# پیدا کردن process که port را اشغال کرده
netstat -ano | findstr :5000

# Kill کردن process
taskkill /PID <PID> /F
```

### ❌ خطای Database Connection

```powershell
# بررسی SQL Server
sqlcmd -S localhost -Q "SELECT @@VERSION"

# یا از SQL Server Management Studio متصل شوید
```

### ❌ خطای npm در Frontend

```powershell
cd D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web

# پاک کردن و نصب مجدد
rm -rf node_modules
rm package-lock.json
npm install
```

### ❌ خطای Build در Backend

```powershell
cd D:\Projects\Hyper\Backend

# Clean و Build مجدد
dotnet clean
dotnet build
```

---

## 🛑 Stop کردن سرویس‌ها

**روش 1:** بستن پنجره‌های CMD/PowerShell که باز شدند

**روش 2:** Ctrl+C در هر terminal

**روش 3:** Kill کردن تمام dotnet processes:
```powershell
Get-Process dotnet | Stop-Process -Force
Get-Process node | Stop-Process -Force
```

---

## 🔍 بررسی سلامت سرویس‌ها

```powershell
# Customer Portal API
curl http://localhost:5000/health

# Admin Panel
curl http://localhost:5001/health
```

---

## 📊 Logs

Logs در console هر سرویس نمایش داده می‌شوند.

برای ذخیره logs در فایل:

```powershell
# مثال
cd D:\Projects\Hyper\Backend\src\CustomerPortal\Hyper.CustomerPortal.Api
dotnet run > logs.txt 2>&1
```

---

## 🎨 Frontend Development

### Hot Reload
Frontend با Next.js از hot reload پشتیبانی می‌کند.
تغییرات بلافاصله اعمال می‌شوند.

### Build Production
```powershell
cd D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web
npm run build
npm start
```

---

## 💡 نکات مفید

### سریع‌ترین راه شروع:
1. دابل کلیک `start-services.bat`
2. منتظر بمانید تا سرویس‌ها بالا بیایند (30-60 ثانیه)
3. باز کردن http://localhost:3000

### Development بدون Frontend:
اگر فقط API می‌خواهید:
```powershell
cd D:\Projects\Hyper\Backend\src\CustomerPortal\Hyper.CustomerPortal.Api
dotnet watch run --urls=http://localhost:5000
```

### Hot Reload برای Backend:
استفاده از `dotnet watch`:
```powershell
dotnet watch run --urls=http://localhost:5000
```

---

## 📝 چک‌لیست قبل از شروع

- [ ] SQL Server در حال اجرا است
- [ ] Port های 5000, 5001, 3000 آزاد هستند
- [ ] npm packages نصب شده‌اند (`node_modules` وجود دارد)
- [ ] .NET 8 SDK نصب شده
- [ ] Database وجود دارد (یا Migration ها اجرا شده‌اند)

---

## 🚀 اولین بار (First Time Setup)

```powershell
# 1. نصب dependencies
cd D:\Projects\Hyper\Frontend\CustomerPortal\Hyper.CustomerPortal.Web
npm install

# 2. Build backend
cd D:\Projects\Hyper\Backend
dotnet build

# 3. اجرای migrations (اگر نیاز است)
cd src\CustomerPortal\Hyper.CustomerPortal.Api
dotnet ef database update

# 4. شروع سرویس‌ها
cd ..\..\..\
.\start-services.bat
```

---

**آماده است! سرویس‌ها در حال اجرا هستند** 🎉

- 🌐 Frontend: http://localhost:3000
- 🔧 API: http://localhost:5000
- 📊 Swagger: http://localhost:5000/swagger
- 🎛️ Admin: http://localhost:5001



