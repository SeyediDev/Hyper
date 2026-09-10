# SMS Feature - MediatR Implementation

این Feature شامل پیاده‌سازی MediatR برای ارسال پیامک OTP است.

## 📦 ساختار

```
Sms/
├── Commands/
│   └── SendOtpSms/
│       ├── SendOtpSmsCommand.cs         # Command برای Request/Response pattern
│       └── SendOtpSmsCommandHandler.cs  # Handler برای پردازش Command
└── Notifications/
    └── OtpSmsNotificationHandler.cs     # Handler برای OtpSmsNotification (در Domain)

Note: OtpSmsNotification در لایه Domain قرار دارد (Neo.Domain.Features.Sms.Dto)
```

## 🚀 نحوه استفاده

### 1️⃣ Command Pattern (Request/Response)

برای زمانی که نیاز به دریافت نتیجه ارسال داریم:

```csharp
// در Controller یا Service
public class SomeController(IMediator mediator)
{
    public async Task<IActionResult> SendOtp(string mobile, string message)
    {
        var command = new SendOtpSmsCommand(mobile, message);
        var result = await mediator.Send(command);
        
        return result ? Ok() : BadRequest();
    }
}
```

### 2️⃣ Notification Pattern (Publish/Subscribe)

برای زمانی که نیاز به Fire-and-Forget داریم:

```csharp
using Neo.Domain.Features.Sms.Dto; // Import for OtpSmsNotification

// در Controller یا Service
public class SomeController(IMediator mediator)
{
    public async Task<IActionResult> SendOtpNotification(string mobile, string message)
    {
        var notification = new OtpSmsNotification(mobile, message);
        await mediator.Publish(notification);
        
        return Accepted(); // بدون منتظر ماندن برای نتیجه
    }
}
```

## 🔄 Integration با MassTransit

این Handler ها می‌توانند همزمان با MassTransit Consumer استفاده شوند:

- **MassTransit Consumer** (`OtpSmsSentConsumer`): برای ارتباط بین سرویس‌ها (Microservices)
- **MediatR Handler**: برای ارتباط درون یک سرویس (In-Process)

## 📝 نکات مهم

1. **Command Handler**: 
   - Exception رو throw می‌کنه
   - Result برمی‌گردونه (bool)
   - برای Synchronous calls

2. **Notification Handler**:
   - Exception رو catch می‌کنه و log می‌کنه
   - Result برنمی‌گردونه
   - برای Asynchronous/Fire-and-Forget calls
   - نباید جلوی handlers دیگه رو بگیره

## 🧪 مثال واقعی

```csharp
using Hyper.Application.Features.Sms.Commands.SendOtpSms;
using Neo.Domain.Features.Sms.Dto;

// ارسال OTP با Command
var otpCommand = new SendOtpSmsCommand("09121234567", "کد تایید شما: 123456");
var success = await mediator.Send(otpCommand);
if (!success)
{
    logger.LogError("Failed to send OTP");
}

// ارسال OTP با Notification (Fire-and-Forget)
var otpNotification = new OtpSmsNotification("09121234567", "کد تایید شما: 123456");
await mediator.Publish(otpNotification);
// ادامه کار بدون منتظر ماندن
```

## 📊 Dependencies

- `ISmsProvidorService`: برای ارسال واقعی پیامک
- `ILogger`: برای logging
- `MediatR`: برای CQRS pattern


