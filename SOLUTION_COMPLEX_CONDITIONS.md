# راه‌حل: شرط‌های پیچیده با Customer, Tenant, Event و توابع C#

## سوال
چگونه شرط پیچیده زیر را پیاده‌سازی کنیم؟
```
((Customer.Age > Tenant.MaxRegularAge) && (Event.Cost > Tenant.AverageCost)) || (Event.Force == true)
```

---

## راه‌حل پیشنهادی: Formula با Context Provider

### استراتژی
استفاده از **FilterKind = Formula** با یک **Context Provider** قوی که:
1. ✅ `Customer.*` را از Customer entity resolve می‌کند
2. ✅ `Tenant.*` را از Tenant configuration resolve می‌کند
3. ✅ `Event.*` را از Event parameters resolve می‌کند
4. ✅ توابع C# را با NCalc Custom Functions پشتیبانی می‌کند

---

## معماری

### 1. ساختار داده

```csharp
public class PromotionTriggerCondition
{
    // موجود
    public PromotionTriggerFilterKind FilterKind { get; set; }
    public string? FilterConstraint { get; set; } // برای Formula
    
    // استفاده:
    // FilterKind = Formula
    // FilterConstraint = "((Customer.Age > Tenant.MaxRegularAge) && (Event.Cost > Tenant.AverageCost)) || (Event.Force == true)"
}
```

### 2. Context Provider

```csharp
public interface IFormulaContextProvider
{
    /// <summary>
    /// ساخت Context برای ارزیابی فرمول
    /// </summary>
    Task<Dictionary<string, object>> BuildContextAsync(
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default);
}
```

### 3. سرویس ارزیابی فرمول پیشرفته

```csharp
public interface IAdvancedFormulaEvaluator
{
    /// <summary>
    /// ارزیابی فرمول با پشتیبانی از Customer, Tenant, Event
    /// </summary>
    Task<bool> EvaluateAsync(
        string formula,
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default);
}
```

---

## پیاده‌سازی

### Phase 1: Context Provider

```csharp
internal class FormulaContextProvider : IFormulaContextProvider
{
    private readonly ICustomerService _customerService;
    private readonly ITenantConfigService _tenantConfigService;
    private readonly IEventService _eventService;
    
    public async Task<Dictionary<string, object>> BuildContextAsync(
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        var context = new Dictionary<string, object>();
        
        // 1. Customer Context
        context["Customer"] = await BuildCustomerContextAsync(request, cancellationToken);
        
        // 2. Tenant Context
        context["Tenant"] = await BuildTenantContextAsync(request.TenantId, cancellationToken);
        
        // 3. Event Context
        context["Event"] = await BuildEventContextAsync(request, cancellationToken);
        
        return context;
    }
    
    private async Task<Dictionary<string, object>> BuildCustomerContextAsync(
        PromotionProcessingRequest request,
        CancellationToken cancellationToken)
    {
        var customer = request.Customer;
        var customerContext = new Dictionary<string, object>();
        
        // فیلدهای مستقیم Customer
        customerContext["Id"] = customer.Id;
        customerContext["Age"] = CalculateAge(customer.BirthDate); // محاسبه سن
        customerContext["FirstName"] = customer.FirstName ?? "";
        customerContext["LastName"] = customer.LastName ?? "";
        customerContext["NationalCode"] = customer.NationalCode ?? "";
        customerContext["Mobile"] = customer.Mobile ?? "";
        customerContext["Email"] = customer.Email ?? "";
        customerContext["BirthDate"] = customer.BirthDate;
        
        // فیلدهای محاسباتی
        customerContext["FullName"] = $"{customer.FirstName} {customer.LastName}";
        
        // Point Balances (اگر نیاز باشد)
        // customerContext["PointBalance"] = await GetPointBalanceAsync(...);
        
        return customerContext;
    }
    
    private async Task<Dictionary<string, object>> BuildTenantContextAsync(
        int tenantId,
        CancellationToken cancellationToken)
    {
        var tenantContext = new Dictionary<string, object>();
        
        // مقادیر Configuration Tenant
        tenantContext["MaxRegularAge"] = await _tenantConfigService.GetConfigValueAsync<int>(
            tenantId, "MaxRegularAge", 65, cancellationToken);
        tenantContext["AverageCost"] = await _tenantConfigService.GetConfigValueAsync<decimal>(
            tenantId, "AverageCost", 1000000, cancellationToken);
        tenantContext["MinPurchaseAmount"] = await _tenantConfigService.GetConfigValueAsync<decimal>(
            tenantId, "MinPurchaseAmount", 500000, cancellationToken);
        
        // سایر مقادیر Tenant
        // tenantContext["CustomValue"] = await GetCustomValueAsync(...);
        
        return tenantContext;
    }
    
    private async Task<Dictionary<string, object>> BuildEventContextAsync(
        PromotionProcessingRequest request,
        CancellationToken cancellationToken)
    {
        var eventContext = new Dictionary<string, object>();
        
        // پارامترهای رویداد
        if (request.Parameters != null)
        {
            // تبدیل پارامترها به نوع مناسب
            foreach (var param in request.Parameters)
            {
                var eventParam = await _eventService.GetParameterAsync(
                    request.EventTypeId, param.Key, false, cancellationToken);
                
                if (eventParam != null)
                {
                    eventContext[param.Key] = ConvertParameterValue(param.Value, eventParam.ParameterType);
                }
                else
                {
                    // اگر پارامتر تعریف نشده، به صورت string استفاده می‌شود
                    eventContext[param.Key] = param.Value;
                }
            }
        }
        
        // نام‌های مستعار برای راحتی
        if (request.Parameters != null)
        {
            eventContext["Cost"] = GetParameterValue(request.Parameters, "Cost", "Amount", "Price");
            eventContext["Amount"] = GetParameterValue(request.Parameters, "Amount", "Cost", "Price");
            eventContext["Force"] = GetParameterValue(request.Parameters, "Force", "IsForced", "Required");
        }
        
        return eventContext;
    }
    
    private int CalculateAge(DateTime? birthDate)
    {
        if (!birthDate.HasValue) return 0;
        var today = DateTime.Today;
        var age = today.Year - birthDate.Value.Year;
        if (birthDate.Value.Date > today.AddYears(-age)) age--;
        return age;
    }
    
    private object ConvertParameterValue(string value, ParameterType type)
    {
        return type switch
        {
            ParameterType.Long => value.ToInt64OrDefault(),
            ParameterType.Float => value.ToFloatOrDefault(),
            ParameterType.DateOnly => value.ToDateTimeOrDefault().Date,
            ParameterType.DateTime => value.ToDateTimeOrDefault(),
            ParameterType.TimeOnly => value.ToTimeSpan(),
            _ => value
        };
    }
    
    private object? GetParameterValue(
        Dictionary<string, string> parameters,
        params string[] possibleKeys)
    {
        foreach (var key in possibleKeys)
        {
            if (parameters.TryGetValue(key, out var value))
                return value;
        }
        return null;
    }
}
```

### Phase 2: Advanced Formula Evaluator

```csharp
internal class AdvancedFormulaEvaluator : IAdvancedFormulaEvaluator
{
    private readonly IFormulaContextProvider _contextProvider;
    private readonly IEvaluateFormulaService _formulaService;
    
    public async Task<bool> EvaluateAsync(
        string formula,
        PromotionProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. ساخت Context
        var context = await _contextProvider.BuildContextAsync(request, cancellationToken);
        
        // 2. تبدیل فرمول به فرم قابل ارزیابی NCalc
        var ncalcFormula = ConvertToNCalcFormula(formula, context);
        
        // 3. ارزیابی با NCalc
        var expression = new NCalc.Expression(ncalcFormula);
        
        // 4. اضافه کردن Context به Parameters
        AddContextToExpression(expression, context);
        
        // 5. اضافه کردن Custom Functions
        AddCustomFunctions(expression);
        
        // 6. ارزیابی
        var result = expression.Evaluate();
        return Convert.ToBoolean(result);
    }
    
    /// <summary>
    /// تبدیل فرمول به فرم NCalc
    /// Customer.Age -> Customer_Age
    /// Tenant.MaxRegularAge -> Tenant_MaxRegularAge
    /// Event.Cost -> Event_Cost
    /// </summary>
    private string ConvertToNCalcFormula(string formula, Dictionary<string, object> context)
    {
        var converted = formula;
        
        // تبدیل Customer.*
        converted = System.Text.RegularExpressions.Regex.Replace(
            converted,
            @"Customer\.(\w+)",
            match => $"Customer_{match.Groups[1].Value}");
        
        // تبدیل Tenant.*
        converted = System.Text.RegularExpressions.Regex.Replace(
            converted,
            @"Tenant\.(\w+)",
            match => $"Tenant_{match.Groups[1].Value}");
        
        // تبدیل Event.*
        converted = System.Text.RegularExpressions.Regex.Replace(
            converted,
            @"Event\.(\w+)",
            match => $"Event_{match.Groups[1].Value}");
        
        return converted;
    }
    
    /// <summary>
    /// اضافه کردن Context به Expression Parameters
    /// </summary>
    private void AddContextToExpression(NCalc.Expression expression, Dictionary<string, object> context)
    {
        // Customer Context
        if (context.TryGetValue("Customer", out var customerDict) && customerDict is Dictionary<string, object> customer)
        {
            foreach (var kvp in customer)
            {
                expression.Parameters[$"Customer_{kvp.Key}"] = kvp.Value;
            }
        }
        
        // Tenant Context
        if (context.TryGetValue("Tenant", out var tenantDict) && tenantDict is Dictionary<string, object> tenant)
        {
            foreach (var kvp in tenant)
            {
                expression.Parameters[$"Tenant_{kvp.Key}"] = kvp.Value;
            }
        }
        
        // Event Context
        if (context.TryGetValue("Event", out var eventDict) && eventDict is Dictionary<string, object> eventCtx)
        {
            foreach (var kvp in eventCtx)
            {
                expression.Parameters[$"Event_{kvp.Key}"] = kvp.Value;
            }
        }
    }
    
    /// <summary>
    /// اضافه کردن Custom Functions برای توابع C#
    /// </summary>
    private void AddCustomFunctions(NCalc.Expression expression)
    {
        // تابع Math
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToLower())
            {
                case "abs":
                    args.Result = Math.Abs(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
                case "max":
                    args.Result = Math.Max(
                        Convert.ToDouble(args.Parameters[0].Evaluate()),
                        Convert.ToDouble(args.Parameters[1].Evaluate()));
                    break;
                case "min":
                    args.Result = Math.Min(
                        Convert.ToDouble(args.Parameters[0].Evaluate()),
                        Convert.ToDouble(args.Parameters[1].Evaluate()));
                    break;
                case "round":
                    args.Result = Math.Round(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
                case "floor":
                    args.Result = Math.Floor(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
                case "ceiling":
                    args.Result = Math.Ceiling(Convert.ToDouble(args.Parameters[0].Evaluate()));
                    break;
            }
        };
        
        // تابع String
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToLower())
            {
                case "contains":
                    var str = args.Parameters[0].Evaluate()?.ToString() ?? "";
                    var substr = args.Parameters[1].Evaluate()?.ToString() ?? "";
                    args.Result = str.Contains(substr);
                    break;
                case "startswith":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "")
                        .StartsWith(args.Parameters[1].Evaluate()?.ToString() ?? "");
                    break;
                case "endswith":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "")
                        .EndsWith(args.Parameters[1].Evaluate()?.ToString() ?? "");
                    break;
                case "tolower":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "").ToLower();
                    break;
                case "toupper":
                    args.Result = (args.Parameters[0].Evaluate()?.ToString() ?? "").ToUpper();
                    break;
            }
        };
        
        // تابع DateTime
        expression.EvaluateFunction += (name, args) =>
        {
            switch (name.ToLower())
            {
                case "now":
                    args.Result = DateTime.Now;
                    break;
                case "today":
                    args.Result = DateTime.Today;
                    break;
                case "year":
                    args.Result = ((DateTime)args.Parameters[0].Evaluate()).Year;
                    break;
                case "month":
                    args.Result = ((DateTime)args.Parameters[0].Evaluate()).Month;
                    break;
                case "day":
                    args.Result = ((DateTime)args.Parameters[0].Evaluate()).Day;
                    break;
                case "datediff":
                    var date1 = (DateTime)args.Parameters[0].Evaluate();
                    var date2 = (DateTime)args.Parameters[1].Evaluate();
                    args.Result = (date2 - date1).TotalDays;
                    break;
            }
        };
    }
}
```

---

## استفاده

### مثال 1: شرط پیچیده

```csharp
// در PromotionTriggerCondition
FilterKind = PromotionTriggerFilterKind.Formula
FilterConstraint = "((Customer.Age > Tenant.MaxRegularAge) && (Event.Cost > Tenant.AverageCost)) || (Event.Force == true)"

// ارزیابی
var result = await _advancedFormulaEvaluator.EvaluateAsync(
    condition.FilterConstraint,
    request,
    cancellationToken);
```

### مثال 2: شرط با توابع

```csharp
FilterConstraint = "Math.Max(Customer.Age, 18) >= Tenant.MinAge && String.Contains(Customer.Mobile, '09')"
```

### مثال 3: شرط با تاریخ

```csharp
FilterConstraint = "DateDiff(Customer.BirthDate, Today()) / 365.25 >= Tenant.MinAge"
```

---

## مزایا

✅ **انعطاف‌پذیری بالا:** هر شرط پیچیده قابل پیاده‌سازی  
✅ **خوانایی:** فرمول به صورت خوانا ذخیره می‌شود  
✅ **قابلیت تبدیل:** می‌توان به Visual Builder تبدیل کرد  
✅ **پشتیبانی از توابع:** توابع C# قابل استفاده  
✅ **Performance:** NCalc بهینه و سریع  
✅ **Type Safety:** تبدیل خودکار نوع‌ها  

---

## بهبودهای آینده

1. **Visual Builder:** ابزار UI برای ساخت فرمول
2. **Syntax Highlighting:** در UI
3. **Validation:** بررسی صحت فرمول قبل از ذخیره
4. **IntelliSense:** پیشنهاد فیلدها و توابع
5. **Template Library:** قالب‌های آماده شرط‌ها

---

## نتیجه‌گیری

**راه‌حل:** استفاده از **FilterKind = Formula** با **Context Provider** و **Advanced Formula Evaluator**

این راه‌حل:
- ✅ شرط‌های پیچیده را پشتیبانی می‌کند
- ✅ Customer, Tenant, Event را resolve می‌کند
- ✅ توابع C# را پشتیبانی می‌کند
- ✅ خوانا و قابل نگهداری است
- ✅ قابل تبدیل به Visual Builder است


