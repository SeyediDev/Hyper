using System.Text.Json;
using System.Text.RegularExpressions;
using Hyper.Domain.Entities.Lotteries;
using Neo.Domain.Features.Integrations;
using Neo.Domain.Features.Sms;

namespace Hyper.Domain.Features.Promotions;


/// <summary>
/// سرویس اجرای اقدامات پویش
/// </summary>
internal class PromotionActionService(
    IRewardAssetInternalService rewardAssetInternalService,
    IPromotionBudgetService promotionBudgetService,
    ISmsService smsService,
    IEvaluateFormulaService evaluateFormulaService,
    IPointLevelService pointLevelService,
    IExternalApiService externalApiService,
    ICustomerSegmentService? customerSegmentService,
    ICommandRepositoryL<CustomerTransaction> customerTransactionCmdRepo,
    ICommandRepositoryL<TenantAttributeValue> customerParameterValueCmdRepo,
    ICommandRepository<CustomerSegmentMembership> customerSegmentMembershipCmdRepo,
    ICommandRepository<CustomerReferrer> customerReferrerCmdRepo,
    ICommandRepository<CustomerPointLevel> customerPointLevelCmdRepo,
    ICommandRepository<CustomerTenant> customerTenantCmdRepo,
    ICommandRepository<LotteryParticipant> lotteryParticipantCmdRepo,
    IQueryRepository<ReferrerCode> referrerCodeRepo,
    IQueryRepository<CustomerPlan> customerPlanQueryRepo,
    IQueryRepository<Lottery> lotteryRepo,
    IQueryRepository<CustomerTenant> customerTenantQueryRepo,
    ILogger<PromotionActionService> logger
    ) : IPromotionActionService
{
    private static readonly Regex TemplateTokenRegex = new(@"\{\{([\w\.]+)\}\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public async Task DoActionAsync(
        PromotionProcessingRequest request, PromotionAction action, CancellationToken cancellationToken = default)
    {
        // دریافت CustomerTenant از EventResponse - این هویت مشتری در اکوسیستم است
        int customerTenantId = request.EventResponse.CustomerTenantId;

        // دریافت CustomerTenant با شامل کردن Customer
        CustomerTenant? customerTenant = await customerTenantQueryRepo
            .FirstOrDefaultWithIncludeAsync(
                ct => ct.Customer,
                ct => ct.Id == customerTenantId,
                cancellationToken);

        if (customerTenant == null)
        {
            logger.LogWarning("CustomerTenant not found for ID: {CustomerTenantId}", customerTenantId);
            return;
        }

        // بررسی بودجه قبل از اجرای اقدام
        if (!await CheckBudgetAsync(request, action, customerTenant, cancellationToken))
        {
            logger.LogWarning("Action {ActionId} of type {ActionKind} skipped due to budget exhaustion",
                action.Id, action.ActionKind);
            return;
        }

        string? value = ExtractValue(request, action);

        switch (action.ActionOnWho)
        {
            case PromotionActionOnWho.Customer:
                await DoActionPerCustomerAsync(request, action, customerTenant, value, cancellationToken);
                break;
            case PromotionActionOnWho.Referrer:
            case PromotionActionOnWho.Both:
                Customer? referrer = await GetReferrer(customerTenant.Id, cancellationToken);
                if (referrer != null)
                {
                    CustomerTenant? referrerCustomerTenant = await customerTenantQueryRepo
                        .FirstOrDefaultAsync(
                            ct => ct.CustomerId == referrer.Id && ct.TenantId == request.EventRequest.TenantId,
                            cancellationToken);
                    if (referrerCustomerTenant != null)
                    {
                        await DoActionPerCustomerAsync(request, action, referrerCustomerTenant, value, cancellationToken);
                    }
                }
                if (action.ActionOnWho == PromotionActionOnWho.Both)
                {
                    await DoActionPerCustomerAsync(request, action, customerTenant, value, cancellationToken);
                }
                break;
        }
    }

    /// <summary>
    /// بررسی بودجه قبل از اجرای اقدام
    /// </summary>
    private async Task<bool> CheckBudgetAsync(
        PromotionProcessingRequest request, PromotionAction action, CustomerTenant customerTenant,
        CancellationToken cancellationToken)
    {
        switch (action.ActionKind)
        {
            case PromotionActionKind.CreditPoint:
            case PromotionActionKind.DebitPoint:
            case PromotionActionKind.SetPointBalance:
                if (action.PointId == null) return true;
                string? value = ExtractValue(request, action);
                long amount = value?.ToLongOrDefault() ?? 0;
                return await promotionBudgetService.HasPointBudgetAsync(
                    action.PointId.Value, amount, customerTenant.Id,
                    cancellationToken);

            case PromotionActionKind.GrantReward:
                return await promotionBudgetService.HasRewardBudgetAsync(
                    action.PromotionId, action.RewardId, 1, cancellationToken);

            case PromotionActionKind.JoinInCustomerSegment:
                return await promotionBudgetService.HasCustomerSegmentBudgetAsync(
                    action.PromotionId, action.CustomerSegmentId, 1, cancellationToken);

            case PromotionActionKind.JoinLottery:
                return await promotionBudgetService.HasLotteryBudgetAsync(
                    action.PromotionId, action.LotteryId, 1, cancellationToken);

            case PromotionActionKind.CallExternalApi:
                return await promotionBudgetService.HasExternalApiBudgetAsync(
                    action.PromotionId, action.ExternalApiId, 1, cancellationToken);

            default:
                return true;
        }
    }

    private async Task<Customer?> GetReferrer(int customerTenantId, CancellationToken cancellationToken)
    {
        var r = await customerReferrerCmdRepo.FirstOrDefaultWithIncludeAsync(
            x => x.ReferrerCustomerTenant.Customer,
            x => x.ReferredCustomerTenantId == customerTenantId, cancellationToken);
        return r?.ReferrerCustomerTenant.Customer;
    }

    private async Task DoActionPerCustomerAsync(
        PromotionProcessingRequest request,
        PromotionAction action,
        CustomerTenant customerTenant,
        string? value,
        CancellationToken cancellationToken)
    {
        long longValue = value?.ToLongOrDefault() ?? 0;

        switch (action.ActionKind)
        {
            case PromotionActionKind.CreditPoint or PromotionActionKind.DebitPoint or PromotionActionKind.SetPointBalance:
                await SetPointBaseAction(request, action, customerTenant, longValue, cancellationToken);
                break;
            case PromotionActionKind.SetPointLevel:
                await SetPointLevelAction(request, action, customerTenant, cancellationToken);
                break;
            case PromotionActionKind.SetCustomerParameterValue:
                if (value != null)
                {
                    await SetCustomerParameterValue(request, action, customerTenant, value, cancellationToken);
                }
                break;
            case PromotionActionKind.JoinInCustomerSegment:
                await JoinInCustomerSegment(request, action, customerTenant, cancellationToken);
                break;
            case PromotionActionKind.ReferrerRegistration:
                if (value != null)
                {
                    await ProcessReferrerRegistration(request, action, customerTenant, value, cancellationToken);
                }
                break;
            case PromotionActionKind.GrantReward:
                await GrantReward(request, action, customerTenant, longValue, cancellationToken);
                break;
            case PromotionActionKind.JoinLottery:
                await JoinLottery(request, action, customerTenant, longValue, cancellationToken);
                break;
            case PromotionActionKind.CallExternalApi:
                await CallExternalApiAsync(request, action, customerTenant, value, cancellationToken);
                break;
        }

        // بررسی بودجه اطلاع‌رسانی قبل از ارسال
        if (action.NotificationSendMethod != PromotionNotificationSendMethod.None)
        {
            await CheckNotificationBudgetAndSendAsync(request, action, customerTenant, cancellationToken);
        }
    }

    private async Task CheckNotificationBudgetAndSendAsync(
        PromotionProcessingRequest request,
        PromotionAction action,
        CustomerTenant customerTenant,
        CancellationToken cancellationToken)
    {
        NotificationBudgetSubType subType = action.NotificationSendMethod switch
        {
            PromotionNotificationSendMethod.SendSms => NotificationBudgetSubType.Sms,
            PromotionNotificationSendMethod.SendEmail => NotificationBudgetSubType.Email,
            PromotionNotificationSendMethod.PushNotification => NotificationBudgetSubType.PushNotification,
            _ => NotificationBudgetSubType.All
        };

        if (!await promotionBudgetService.HasNotificationBudgetAsync(
            action.PromotionId, subType, 1, cancellationToken))
        {
            logger.LogWarning("Notification budget exhausted for promotion {PromotionId}, method {Method}",
                action.PromotionId, action.NotificationSendMethod);
            return;
        }

        switch (action.NotificationSendMethod)
        {
            case PromotionNotificationSendMethod.SendSms:
                await SendSms(request, action, customerTenant, cancellationToken);
                break;
            case PromotionNotificationSendMethod.SendEmail:
                await SendEmail(request, action, customerTenant, cancellationToken);
                break;
            case PromotionNotificationSendMethod.PushNotification:
                await SendNotification(request, action, customerTenant, cancellationToken);
                break;
        }
    }

    private async Task SendNotification(PromotionProcessingRequest request, PromotionAction action,
        CustomerTenant customerTenant, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(action.MessageTemplate)) return;


        if (customerTenant.Customer.MobileNo != null)
        {
            var notification = evaluateFormulaService.Evaluate(
                action.MessageTemplate, request.EventResponse.AttributeValues);
            if (notification != null)
            {
                await smsService.SendAsync(new(customerTenant.Customer.MobileNo, notification.ToString()!));
            }
        }

        logger.LogInformation("SendNotification {customer} {template}",
            customerTenant.Customer.NationalCode, action.MessageTemplate);
    }

    private async Task SendSms(PromotionProcessingRequest request, PromotionAction action, CustomerTenant customerTenant, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(action.MessageTemplate)) return;

        var sms = evaluateFormulaService.Evaluate(action.MessageTemplate, request.EventResponse.AttributeValues);

        if (customerTenant.Customer.MobileNo != null && sms != null)
        {
            await smsService.SendAsync(new(customerTenant.Customer.MobileNo, sms.ToString()!));
        }

        logger.LogInformation("SendSms {customer} {template}",
            customerTenant.Customer.NationalCode, action.MessageTemplate);
    }

    private Task SendEmail(PromotionProcessingRequest request, PromotionAction action, CustomerTenant customerTenant, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(action.MessageTemplate)) return Task.CompletedTask;

        // TODO: Implement email sending
        logger.LogInformation("SendEmail {customer} {template}",
            customerTenant.Customer.NationalCode, action.MessageTemplate);
        return Task.CompletedTask;
    }

    private async Task SetPointBaseAction(
        PromotionProcessingRequest request,
        PromotionAction action,
        CustomerTenant customerTenant,
        long value,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("SetPointBaseAction({ActionId}, {CustomerTenantId}, {Value})", action.Id, customerTenant.Id, value);

        if (action.PointId == null)
        {
            throw new ArgumentNullException(nameof(action.PointId));
        }

        CustomerTransaction? lastCustomerTransaction = await customerTransactionCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenant.Id &&
                 x.PointId == action.PointId, cancellationToken);

        CustomerTransaction customerTransaction = new()
        {
            CustomerTenantId = customerTenant.Id,
            PointId = action.PointId.Value,
            Point = action.Point!,
            EventLogId = request.EventResponse.EventLogId,
            PromotionId = action.PromotionId,
            PromotionActionId = action.Id,
            EventChannelId = request.EventRequest.ChannelId
        };

        CustomerPlan? activeCustomerPlan = await GetActiveCustomerPlanAsync(customerTenant.Id, cancellationToken);
        if (activeCustomerPlan != null)
        {
            customerTransaction.ActivePlanId = activeCustomerPlan.PlanId;
            customerTransaction.ActivePlan = activeCustomerPlan.Plan;
        }

        switch (action.ActionKind)
        {
            case PromotionActionKind.CreditPoint:
                customerTransaction.TransactionType = CustomerTransactionType.Credit;
                customerTransaction.Credit = value;
                if (action.Point != null && action.Point.HasExpiration && action.Point.ExpirationDays.HasValue)
                {
                    customerTransaction.ExpirationDate = DateTime.UtcNow.AddDays(action.Point.ExpirationDays.Value);
                }
                await SetNewPointBalance(customerTransaction, customerTenant, (lastCustomerTransaction?.Balance ?? 0) + value, cancellationToken);
                break;

            case PromotionActionKind.DebitPoint:
                long currentBalance = lastCustomerTransaction?.Balance ?? 0;
                long newBalance = currentBalance - value;

                if (newBalance < 0)
                {
                    logger.LogWarning(
                        "DebitPoint action {ActionId} would result in negative balance. Action rejected.",
                        action.Id);
                    return;
                }

                customerTransaction.TransactionType = CustomerTransactionType.Debit;
                customerTransaction.Debit = value;
                await SetNewPointBalance(customerTransaction, customerTenant, newBalance, cancellationToken);
                break;

            case PromotionActionKind.SetPointBalance:
                long oldBalance = lastCustomerTransaction?.Balance ?? 0;
                if (value - oldBalance >= 0)
                {
                    customerTransaction.TransactionType = CustomerTransactionType.Credit;
                    customerTransaction.Credit = value - oldBalance;
                    if (action.Point != null && action.Point.HasExpiration && action.Point.ExpirationDays.HasValue)
                    {
                        customerTransaction.ExpirationDate = DateTime.UtcNow.AddDays(action.Point.ExpirationDays.Value);
                    }
                }
                else
                {
                    customerTransaction.TransactionType = CustomerTransactionType.Debit;
                    customerTransaction.Debit = oldBalance - value;
                }
                await SetNewPointBalance(customerTransaction, customerTenant, value, cancellationToken);
                break;
        }

        customerTransactionCmdRepo.Add(customerTransaction);
    }

    private async Task CallExternalApiAsync(PromotionProcessingRequest request, PromotionAction action, CustomerTenant customerTenant, string? value, CancellationToken cancellationToken)
    {
        if (action.ExternalApi == null)
        {
            logger.LogWarning("External API configuration is missing for action {ActionId}", action.Id);
            return;
        }

        ExternalApiRequest apiRequest = new();
        Customer customer = customerTenant.Customer;

        foreach ((string key, string resolved) in ResolveMappings(action.PathParameterMappingsJson, request, action, customer, value))
        {
            apiRequest.PathParameters[key] = resolved;
        }

        foreach ((string key, string resolved) in ResolveMappings(action.QueryParameterMappingsJson, request, action, customer, value))
        {
            apiRequest.QueryParameters[key] = resolved;
        }

        foreach ((string key, string resolved) in ResolveMappings(action.HeaderMappingsJson, request, action, customer, value))
        {
            apiRequest.Headers[key] = resolved;
        }

        string? body = ResolveTemplate(action.BodyTemplate, request, action, customer, value);
        if (!string.IsNullOrWhiteSpace(body))
        {
            apiRequest.Body = new ExternalApiRequestBody(body, action.BodyContentType ?? "application/json");
        }

        ExternalApiInvocationResult response = await externalApiService.InvokeAsync(action.ExternalApi, apiRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("External API call failed for action {ActionId} with status {Status}", action.Id, response.StatusCode);
            return;
        }

        if (string.IsNullOrWhiteSpace(action.ResponseMappingsJson) || string.IsNullOrWhiteSpace(response.Content))
        {
            return;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(response.Content);
            Dictionary<string, string>? mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(action.ResponseMappingsJson) ?? [];
            if (mappings.Count == 0)
            {
                return;
            }

            foreach ((string targetKey, string jsonPath) in mappings)
            {
                string? extracted = ExtractJsonValue(document.RootElement, jsonPath);
                if (extracted != null)
                {
                    // TODO: Add to request.Attributes if it supports dictionary-like operations
                }
            }
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to process external API response mapping for action {ActionId}", action.Id);
        }
    }

    private IEnumerable<KeyValuePair<string, string>> ResolveMappings(string? jsonMappings, PromotionProcessingRequest request, PromotionAction action, Customer customer, string? value)
    {
        if (string.IsNullOrWhiteSpace(jsonMappings)) yield break;

        Dictionary<string, string>? mappings;
        try
        {
            mappings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonMappings);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Invalid mapping configuration for action {ActionId}", action.Id);
            yield break;
        }

        if (mappings == null)
        {
            yield break;
        }

        foreach ((string key, string template) in mappings)
        {
            string? resolved = ResolveTemplate(template, request, action, customer, value);
            if (!string.IsNullOrWhiteSpace(resolved))
            {
                yield return new KeyValuePair<string, string>(key, resolved);
            }
        }
    }

    private string? ResolveTemplate(string? template, PromotionProcessingRequest request, PromotionAction action, Customer customer, string? value)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            return template;
        }

        return TemplateTokenRegex.Replace(template, match =>
        {
            string token = match.Groups[1].Value.Trim();
            return LookupTokenValue(token, request, action, customer, value) ?? string.Empty;
        });
    }

    private string? LookupTokenValue(string token, PromotionProcessingRequest request, PromotionAction action, Customer customer, string? value)
    {
        return token switch
        {
            "Value" or "value" => value,
            "Customer.Id" => customer.Id.ToString(),
            "Customer.NationalCode" => customer.NationalCode?.ToString(),
            "Customer.MobileNo" => customer.MobileNo,
            "Promotion.Id" => action.PromotionId.ToString(),
            "Promotion.TenantId" => request.EventRequest.TenantId.ToString(),
            "Action.Id" => action.Id.ToString(),
            "Action.Kind" => action.ActionKind.ToString(),
            "EventLog.Id" => request.EventResponse.EventLogId.ToString(),
            "EventType.Id" => request.EventRequest.EventTypeId?.ToString(),
            _ => null
        };
    }

    private static string? ExtractJsonValue(JsonElement element, string jsonPath)
    {
        if (string.IsNullOrWhiteSpace(jsonPath))
        {
            return null;
        }

        string path = jsonPath.Trim();
        if (path.StartsWith("$", StringComparison.Ordinal))
        {
            path = path[1..];
        }
        if (path.StartsWith(".", StringComparison.Ordinal))
        {
            path = path[1..];
        }

        foreach (string segment in path.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                if (!element.TryGetProperty(segment, out element))
                {
                    return null;
                }
                continue;
            }

            if (element.ValueKind == JsonValueKind.Array && int.TryParse(segment, out int index))
            {
                if (index < 0 || index >= element.GetArrayLength())
                {
                    return null;
                }

                element = element[index];
                continue;
            }

            return null;
        }

        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.ToString(),
            JsonValueKind.Null => null,
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => element.GetRawText()
        };
    }

    private async Task SetNewPointBalance(
        CustomerTransaction customerTransaction, CustomerTenant customerTenant, long balance, CancellationToken cancellationToken)
    {
        if (customerTransaction.Balance != balance && customerTransaction.Point.PointType == PointType.Xp)
        {
            var r = await pointLevelService.CheckAndUpdateLevel(
                new CheckPointLevelRequest(
                    customerTransaction.PointId,
                    customerTenant.Id,
                    balance,
                    customerTransaction.EventLogId
                ), cancellationToken);
        }
        customerTransaction.Balance = balance;
    }

    private async Task SetPointLevelAction(
        PromotionProcessingRequest request,
        PromotionAction action,
        CustomerTenant customerTenant,
        CancellationToken cancellationToken)
    {
        if (action.PointLevelId == null)
        {
            logger.LogWarning("PointLevelId is missing for SetPointLevel action {ActionId}", action.Id);
            return;
        }

        var pointLevel = action.PointLevel ?? throw new InvalidOperationException("PointLevel not loaded");

        CustomerPointLevel? oldCustomerPointLevel = await customerPointLevelCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenant.Id && x.PointLevel.PointId == pointLevel.PointId, cancellationToken);

        if (oldCustomerPointLevel != null && oldCustomerPointLevel.PointLevelId == action.PointLevelId.Value)
        {
            logger.LogInformation("CustomerTenant {CustomerTenantId} already has PointLevel {PointLevelId}", customerTenant.Id, action.PointLevelId.Value);
            return;
        }

        if (oldCustomerPointLevel != null)
        {
            oldCustomerPointLevel.ExpireDate = DateTime.UtcNow;
            customerPointLevelCmdRepo.Update(oldCustomerPointLevel);
        }

        CustomerPointLevel newCustomerPointLevel = new()
        {
            CustomerTenantId = customerTenant.Id,
            PointLevelId = action.PointLevelId.Value,
            EventLogId = request.EventResponse.EventLogId
        };
        customerPointLevelCmdRepo.Add(newCustomerPointLevel);

        await customerPointLevelCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Set PointLevel {PointLevelId} for customerTenant {CustomerTenantId}", action.PointLevelId.Value, customerTenant.Id);
    }

    private async Task SetCustomerParameterValue(
        PromotionProcessingRequest request, PromotionAction action, CustomerTenant customerTenant,
        string value, CancellationToken cancellationToken)
    {
        if (action.CustomerAttributeId == null)
        {
            throw new ArgumentNullException(nameof(action.CustomerAttributeId));
        }

        TenantAttributeValue? customerParameterValue = await customerParameterValueCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenant.Id && x.AttributeId == action.CustomerAttributeId, cancellationToken);

        bool valueChanged = false;
        if (customerParameterValue != null)
        {
            if (customerParameterValue.Value == value)
            {
                return;
            }
            valueChanged = true;
            customerParameterValue.ExpireDate = DateTime.UtcNow;
            customerParameterValue.IsDeleted = true;
            customerParameterValueCmdRepo.Update(customerParameterValue);
            await customerParameterValueCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            valueChanged = true;
        }

        customerParameterValue = new()
        {
            CustomerTenantId = customerTenant.Id,
            AttributeId = action.CustomerAttributeId.Value,
            EventLogId = request.EventResponse.EventLogId,
            Value = value,
        };
        customerParameterValueCmdRepo.Add(customerParameterValue);
        await customerParameterValueCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (valueChanged && customerSegmentService != null)
        {
            try
            {
                await customerSegmentService.RecheckSegmentMembershipAfterParameterChangeAsync(
                    customerTenant.CustomerId, request.EventResponse.EventLogId,
                    request.EventResponse.AttributeValues, cancellationToken);
            }
            catch (Exception)
            {
                // لاگ خطا اما عدم مسدود کردن جریان اصلی
            }
        }
    }

    private async Task JoinInCustomerSegment(
        PromotionProcessingRequest request,
        PromotionAction action,
        CustomerTenant customerTenant,
        CancellationToken cancellationToken)
    {
        if (action.CustomerSegmentId == null)
        {
            throw new ArgumentNullException(nameof(action.CustomerSegmentId));
        }

        CustomerSegmentMembership? customerSegmentMembership = await customerSegmentMembershipCmdRepo.FirstOrDefaultAsync(
            x => x.SegmentId == action.CustomerSegmentId && x.CustomerTenantId == customerTenant.Id, cancellationToken);

        if (customerSegmentMembership == null)
        {
            customerSegmentMembership = new()
            {
                CustomerTenantId = customerTenant.Id,
                SegmentId = action.CustomerSegmentId.Value,
                EventLogId = request.EventResponse.EventLogId
            };
            customerSegmentMembershipCmdRepo.Add(customerSegmentMembership);
            await customerSegmentMembershipCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<CustomerPlan?> GetActiveCustomerPlanAsync(int customerTenantId, CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow;
        return await customerPlanQueryRepo.FirstOrDefaultWithIncludeAsync(
            cp => cp.Plan,
            cp => cp.CustomerTenantId == customerTenantId &&
                  cp.Status == CustomerPlanStatus.Active &&
                  cp.IsActive &&
                  cp.StartDate <= utcNow &&
                  cp.ExpiryDate >= utcNow,
            cancellationToken);
    }

    private async Task<CustomerTenant> EnsureCustomerTenantAsync(Customer customer, int tenantId, CancellationToken cancellationToken)
    {
        var customerTenant = await customerTenantCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customer.Id && x.TenantId == tenantId, cancellationToken);
        if (customerTenant == null)
        {
            customerTenant = new CustomerTenant
            {
                CustomerId = customer.Id,
                TenantId = tenantId,
                CreateDate = DateTime.UtcNow
            };
            customerTenantCmdRepo.Add(customerTenant);
            await customerTenantCmdRepo.SaveChangesAsync(cancellationToken);
        }
        return customerTenant;
    }

    private async Task GrantReward(PromotionProcessingRequest request, PromotionAction action,
        CustomerTenant customerTenant, long value, CancellationToken cancellationToken)
    {
        if (action.RewardId == null || action.Reward == null)
        {
            throw new ArgumentNullException(nameof(action.RewardId));
        }
        if (value == 0)
        {
            value = 1;
        }

        await rewardAssetInternalService.Create(
            action.Reward, customerTenant, request.EventResponse.EventLogId, action.PromotionId, action.Id, (int)value, cancellationToken);
    }

    private string? ExtractValue(PromotionProcessingRequest request, PromotionAction action)
    {
        if (string.IsNullOrWhiteSpace(action.AmountFormula))
        {
            return null;
        }

        object? result = evaluateFormulaService.Evaluate(action.AmountFormula, request.EventResponse.AttributeValues);

        return result?.ToString();
    }

    private async Task ProcessReferrerRegistration(
        PromotionProcessingRequest request, PromotionAction action,
        CustomerTenant referredCustomerTenant, string code, CancellationToken cancellationToken)
    {
        ReferrerCode? referrerCode = await referrerCodeRepo.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        if (referrerCode != null)
        {
            CustomerTenant referrerCustomerTenant = referrerCode.CustomerTenant
                ?? await customerTenantCmdRepo.FirstOrDefaultAsync(
                    ct => ct.Id == referrerCode.CustomerTenantId,
                    cancellationToken)
                ?? throw new InvalidOperationException("رابطه مشتری معرف یافت نشد");

            CustomerReferrer? customerReferrer = await customerReferrerCmdRepo.FirstOrDefaultAsync(
                x => x.ReferredCustomerTenantId == referredCustomerTenant.Id &&
                     x.ReferrerCustomerTenantId == referrerCustomerTenant.Id
                , cancellationToken);

            if (customerReferrer != null)
            {
                return;
            }

            customerReferrer = new()
            {
                ReferrerCustomerTenantId = referrerCustomerTenant.Id,
                ReferredCustomerTenantId = referredCustomerTenant.Id,
                ReferrerCodeId = referrerCode.Id,
                EventLogId = request.EventResponse.EventLogId,
                PromotionActionId = action.Id,
                PromotionId = action.PromotionId,
            };
            customerReferrerCmdRepo.Add(customerReferrer);
            await customerReferrerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task JoinLottery(
        PromotionProcessingRequest request,
        PromotionAction action,
        CustomerTenant customerTenant,
        long chanceIncrease,
        CancellationToken cancellationToken)
    {
        if (action.LotteryId == null || action.Lottery == null)
        {
            throw new ArgumentNullException(nameof(action.LotteryId), "LotteryId must be specified for JoinLottery action");
        }

        Lottery? lottery = await lotteryRepo.FirstOrDefaultAsync(
            x => x.Id == action.LotteryId.Value, cancellationToken)
            ?? throw new InvalidOperationException($"قرعه‌کشی با شناسه {action.LotteryId.Value} یافت نشد");

        if (!lottery.IsActive)
        {
            logger.LogWarning("Lottery {LotteryId} is not active", lottery.Id);
            return;
        }

        DateTime now = DateTime.UtcNow;
        if (lottery.FromDate.HasValue && lottery.FromDate > now)
        {
            logger.LogWarning("Lottery {LotteryId} has not started yet", lottery.Id);
            return;
        }

        if (lottery.ToDate.HasValue && lottery.ToDate < now)
        {
            logger.LogWarning("Lottery {LotteryId} has ended", lottery.Id);
            return;
        }

        LotteryParticipant? participant = await lotteryParticipantCmdRepo.FirstOrDefaultAsync(
            x => x.LotteryId == lottery.Id && x.CustomerTenantId == customerTenant.Id,
            cancellationToken);

        int chanceIncreaseValue = chanceIncrease > 0 ? (int)chanceIncrease : 1;

        if (participant == null)
        {
            participant = new LotteryParticipant
            {
                LotteryId = lottery.Id,
                CustomerTenantId = customerTenant.Id,
                Chance = chanceIncreaseValue,
                IsWinner = false,
                ParticipatedAt = now
            };
            lotteryParticipantCmdRepo.Add(participant);
            logger.LogInformation("Created new lottery participant for customerTenant {CustomerTenantId} in lottery {LotteryId} with chance {Chance}",
                customerTenant.Id, lottery.Id, chanceIncreaseValue);
        }
        else
        {
            participant.Chance += chanceIncreaseValue;
            lotteryParticipantCmdRepo.Update(participant);
            logger.LogInformation("Increased chance for lottery participant {ParticipantId} by {ChanceIncrease} (new chance: {NewChance})",
                participant.Id, chanceIncreaseValue, participant.Chance);
        }

        await lotteryParticipantCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>
/// Extension methods for string conversion
/// </summary>
internal static class StringExtensions
{
    public static long ToLongOrDefault(this string? value, long defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;

        if (long.TryParse(value, out long result))
            return result;

        return defaultValue;
    }
}
