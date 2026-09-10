# 🐳 راهنمای Docker و CI/CD برای Hyper Backend

این راهنما نحوه استفاده از فایل‌های Docker و CI/CD را برای پروژه Hyper Backend توضیح می‌دهد.

## 📋 فهرست

- [ساختار پروژه](#ساختار-پروژه)
- [Docker](#docker)
  - [Build کردن Image ها](#build-کردن-image-ها)
  - [اجرای با Docker Compose](#اجرای-با-docker-compose)
- [CI/CD](#cicd)
  - [GitLab CI/CD](#gitlab-cicd)
  - [GitHub Actions](#github-actions)
- [تنظیمات](#تنظیمات)

---

## 📂 ساختار پروژه

```
D:\Projects\
├── Hyper\
│   └── Backend\
│       ├── src\
│       │   ├── AdminPanel\
│       │   │   └── Hyper.AdminPanel.Web\
│       │   │       └── Dockerfile.new
│       │   ├── Channel\
│       │   │   └── Hyper.Channel.Api\
│       │   │       └── Dockerfile.new
│       │   └── CustomerPortal\
│       │       └── Hyper.CustomerPortal.Api\
│       │           └── Dockerfile.new
│       ├── .gitlab-ci.yml
│       └── .github\workflows\build-and-deploy.yml
├── Neo\
│   └── src\
├── Neo.Bpms\
│   └── src\
├── docker-compose.yml
└── .dockerignore
```

---

## 🐋 Docker

### Build کردن Image ها

#### روش 1: Build تک به تک

```powershell
# از دایرکتوری D:\Projects اجرا کنید

# Admin Panel
docker build -f Hyper/Backend/src/AdminPanel/Hyper.AdminPanel.Web/Dockerfile.new -t Hyper-admin-web:latest .

# Channel API
docker build -f Hyper/Backend/src/Channel/Hyper.Channel.Api/Dockerfile.new -t Hyper-channel-api:latest .

# Customer Portal API
docker build -f Hyper/Backend/src/CustomerPortal/Hyper.CustomerPortal.Api/Dockerfile.new -t Hyper-customer-api:latest .
```

#### روش 2: Build با Docker Compose

```powershell
cd D:\Projects
docker-compose build
```

### اجرای با Docker Compose

```powershell
# Start همه سرویس‌ها
cd D:\Projects
docker-compose up -d

# مشاهده logs
docker-compose logs -f

# Stop کردن سرویس‌ها
docker-compose down

# Stop و حذف volumes
docker-compose down -v
```

### دسترسی به سرویس‌ها

بعد از اجرای docker-compose:

- **Admin Panel**: http://localhost:5001
- **Channel API**: http://localhost:5003
- **Customer Portal API**: http://localhost:5005
- **RabbitMQ Management**: http://localhost:15672 (admin/admin123)
- **SQL Server**: localhost:1433 (sa/YourStrong@Password123)
- **Redis**: localhost:6379

---

## 🔄 CI/CD

### GitLab CI/CD

#### تنظیمات اولیه

1. **متغیرهای محیطی را در GitLab تنظیم کنید:**
   - Settings → CI/CD → Variables
   - اضافه کردن متغیرها:
     ```
     CI_REGISTRY_PASSWORD
     CI_REGISTRY_USER
     NEO_REPO (مثال: your-group/neo)
     NEO_BPMS_REPO (مثال: your-group/neo-bpms)
     ```

2. **فایل `.gitlab-ci.yml` را ویرایش کنید:**
   ```yaml
   variables:
     PROJECT_PATH: your-group/Hyper-backend
     NEO_REPO: your-group/neo
     NEO_BPMS_REPO: your-group/neo-bpms
   ```

#### مراحل Pipeline

Pipeline شامل مراحل زیر است:

1. **Prepare**: Clone کردن repository های Neo و Neo.Bpms
2. **Build**: Restore و Build کردن solution
3. **Test**: اجرای Unit Tests
4. **Docker Build**: ساخت و Push کردن Docker Images
5. **Deploy**: Deploy به staging/production (Manual)

#### اجرای Pipeline

```bash
# Push کردن کد
git add .
git commit -m "Your commit message"
git push origin develop  # برای staging
git push origin master   # برای production
```

### GitHub Actions

#### تنظیمات اولیه

1. **Secrets را در GitHub تنظیم کنید:**
   - Settings → Secrets and variables → Actions
   - اضافه کردن secrets:
     ```
     PAT_TOKEN (Personal Access Token برای دسترسی به Neo و Neo.Bpms)
     NEO_REPO (مثال: your-org/neo)
     NEO_BPMS_REPO (مثال: your-org/neo-bpms)
     ```

2. **GitHub Container Registry را فعال کنید:**
   - Settings → Packages
   - اطمینان از دسترسی به ghcr.io

#### مراحل Workflow

Workflow شامل jobs زیر است:

1. **Prepare**: Clone سه repository
2. **Build**: Build کردن solution
3. **Test**: اجرای تست‌ها
4. **Docker Build**: ساخت و Push به ghcr.io
5. **Deploy**: Deploy خودکار یا دستی
6. **Code Quality**: تحلیل کیفیت کد
7. **Security Scan**: اسکن امنیتی با Trivy

#### Trigger کردن Workflow

```bash
# Push به branch اصلی
git push origin main

# ایجاد tag برای production
git tag -a v1.0.0 -m "Version 1.0.0"
git push origin v1.0.0

# Manual trigger از GitHub UI
# Actions → Build and Deploy → Run workflow
```

---

## ⚙️ تنظیمات

### تنظیمات Docker Compose

فایل `docker-compose.yml` را ویرایش کنید:

```yaml
environment:
  # Connection String
  - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=Hyperyek;...
  
  # Redis
  - Redis__Configuration=redis:6379
  
  # RabbitMQ
  - RabbitMQ__Host=rabbitmq
  - RabbitMQ__Username=admin
  - RabbitMQ__Password=admin123
```

### تنظیمات Production

برای production:

1. **رمزهای عبور را تغییر دهید**
2. **TLS/SSL را فعال کنید**
3. **Health checks را تنظیم کنید**
4. **Resource limits را اضافه کنید:**

```yaml
services:
  admin-panel:
    deploy:
      resources:
        limits:
          cpus: '2'
          memory: 2G
        reservations:
          cpus: '1'
          memory: 1G
```

### متغیرهای محیطی

متغیرهای مهم:

| متغیر | توضیحات | مثال |
|-------|---------|------|
| `ASPNETCORE_ENVIRONMENT` | محیط اجرا | Development, Staging, Production |
| `ConnectionStrings__DefaultConnection` | Connection string دیتابیس | Server=...;Database=... |
| `Redis__Configuration` | آدرس Redis | redis:6379 |
| `RabbitMQ__Host` | آدرس RabbitMQ | rabbitmq |

---

## 🔧 Troubleshooting

### مشکلات رایج

#### 1. خطای Build در Docker

```powershell
# پاک کردن cache
docker builder prune -a

# Build مجدد بدون cache
docker build --no-cache -f ... -t ... .
```

#### 2. خطای Permission در Linux

```bash
# اضافه کردن user به گروه docker
sudo usermod -aG docker $USER
```

#### 3. خطای Network در Docker Compose

```powershell
# پاک کردن networks
docker network prune

# اجرای مجدد
docker-compose up -d
```

#### 4. مشکل در CI/CD - Clone Dependencies

اطمینان حاصل کنید که:
- Token های دسترسی معتبر هستند
- Repository ها در دسترس هستند
- Branch های مشخص شده موجود هستند

---

## 📊 Monitoring

### Docker Stats

```powershell
# مشاهده استفاده از منابع
docker stats

# مشاهده logs
docker-compose logs -f [service-name]
```

### Health Checks

```powershell
# بررسی health status
docker ps
# ستون STATUS را بررسی کنید: healthy/unhealthy
```

---

## 🚀 بهبودهای آینده

- [ ] استفاده از NuGet packages به جای ProjectReference
- [ ] اضافه کردن Kubernetes manifests
- [ ] راه‌اندازی monitoring با Prometheus/Grafana
- [ ] اضافه کردن Integration Tests به CI/CD
- [ ] پیاده‌سازی Blue-Green Deployment

---

## 📝 نکات مهم

⚠️ **توجه:** 

1. Build context باید از `D:\Projects` باشد
2. همیشه structure دایرکتوری را حفظ کنید
3. فایل `.dockerignore` را بررسی کنید
4. برای production حتماً از NuGet packages استفاده کنید

---

## 📞 پشتیبانی

اگر مشکلی داشتید:

1. ابتدا logs را بررسی کنید
2. Documentation Docker را مطالعه کنید
3. با تیم DevOps تماس بگیرید

---

**تاریخ ایجاد:** نوامبر 2025  
**نسخه:** 1.0.0


