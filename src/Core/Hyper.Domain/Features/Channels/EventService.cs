namespace Hyper.Domain.Features.Channels;

public interface IEventService
{
    [Telemetry]
    Task<EventResponse> ReceiveEventAsync(EventRequest request, CancellationToken cancellationToken);
}

internal class EventService(
          IAttributeService attributeService
        , IAttributeValueService attributeValueService
        , ICustomerTenantService customerTenantService
        , ICustomerSegmentService customerSegmentService
        , IReferrerCodeValidationService referrerCodeValidationService
        , IPromotionService promotionService
        , ICommandRepository<EventLog, long> eventLogCommand
        , ICommandRepository<EventLogAttribute, long> eventLogAttributeCommand
        , ILogger<EventService> logger
    ) : IEventService
{
    public async Task<EventResponse> ReceiveEventAsync(EventRequest request, CancellationToken cancellationToken)
    {
        int? referrerCodeId = await ValidateEventRequestAsync(request);
        CustomerTenant customerTenant = await customerTenantService.GetOrCreateAndSetReservedAttributesAsync(
            request.TenantId, request.CustomerMobile, request.Attributes?.Customer, cancellationToken) ??
            throw new ArgumentException("اطلاعات مشتری معتبر نمی باشد");

        var response = new EventResponse()
        {
            CustomerTenantId = customerTenant.Id
        };
        await eventLogCommand.UnitOfWork.DoTransaction(async () =>
        {
            response.EventLogId = await LogEvent(request, customerTenant, referrerCodeId, cancellationToken);

            await RegisterReferrer(request, response, referrerCodeId, cancellationToken);

            await GetAndSetAttributesValuesAsync(request, response, cancellationToken);

            _ = await eventLogCommand.SaveChangesAsync(cancellationToken);
        });

        await GetPointAttributesValues(customerTenant, response, cancellationToken);
        var promotionProcessingRequest = new PromotionProcessingRequest(request, response);
        response.PromotionProcessingResponse = await promotionService.ProcessEventAsync(promotionProcessingRequest, cancellationToken);
        return response;
    }

    private async Task GetPointAttributesValues(
        CustomerTenant customerTenant, EventResponse response, CancellationToken cancellationToken)
    {
        var balances = await customerTenantService.GetPointBalancesAsync(customerTenant.Id, true, cancellationToken);
        foreach (var balance in balances)
        {
            Add(balance.Point.Key, balance.Balance);
        }
        var levels = await customerTenantService.GetPointLevelsAsync(customerTenant.Id, true, cancellationToken);
        foreach (var level in levels)
        {
            Add($"{level.PointLevel.Point.Key}_LevelId", level.PointLevel.Level);
            Add($"{level.PointLevel.Point.Key}_Level", level.PointLevel.Key);
        }
        void Add(string suffix, object value)
        {
            response.AttributeValues.Add($"CustomerPoint_{suffix}", value);
        }
    }

    private async Task GetAndSetAttributesValuesAsync(
        EventRequest request, EventResponse response, CancellationToken cancellationToken)
    {
        await GetAndSet(AttributeArea.Tenant, null, null, request.Attributes?.Customer, true);
        await GetAndSet(AttributeArea.Product, request.ProductId, request.ProductCategoryId, request.Attributes?.Product, true);
        await GetAndSet(AttributeArea.Event, request.EventTypeId, null, request.Attributes?.Event, true);
        await GetAndSet(AttributeArea.Channel, request.ChannelId, null, request.Attributes?.Channel, true);

        List<int> segments = await AutoJoinCustomerToEligibleSegmentsAsync(request, response, cancellationToken);
        foreach (var segmentId in segments)
        {
            //TODO شبهه : به ازای هر سنجه 
            await GetAndSet(AttributeArea.Segment, segmentId, null, request.Attributes?.Customer, false);
        }

        async Task GetAndSet(AttributeArea area, int? paramId, int? paramCategoryId, AttributesValues? attributesValues, bool addAttributeIfNotDefined)
        {
            await GetAndSetAttributeValues(area, paramId, paramCategoryId, attributesValues, addAttributeIfNotDefined,
                request, response, cancellationToken);
        }
    }

    private async Task GetAndSetAttributeValues(
        AttributeArea area, int? paramId, int? paramCategoryId,
        AttributesValues? attributesValues, bool addAttributeIfNotDefined,
        EventRequest request, EventResponse response, CancellationToken cancellationToken)
    {
        if (attributesValues == null)
            return;
        
        List<EventAttributeValueDto> eventAttributeValueDtos = [];
        
        // Phase 1: Collect all attribute value DTOs without individual GetParamKey calls
        foreach (var attributeValue in attributesValues)
        {
            AttributeDto? attributeDto =
                await attributeService.GetAttributeAsync(
                    request.TenantId, area, attributeValue.Key, paramId, paramCategoryId, cancellationToken)
                ?? (addAttributeIfNotDefined ?
                await attributeService.AddAttributeAsync(
                    request.TenantId, area, attributeValue.Key, paramId, paramCategoryId, attributeValue.Key, attributeValue.Value, cancellationToken)
                : null);
            if (attributeDto == null) { continue; }
            
            EventLogAttribute eventLogAttribute = new()
            {
                EventLogId = response.EventLogId,
                AttributeId = attributeDto?.Id ?? 0,
                Value = attributeValue.Value?.ToString() ?? ""
            };
            eventLogAttributeCommand.Add(eventLogAttribute);

            EventAttributeValueDto attributeValueDto = new(attributeDto!)
            {
                SegmentId = area == AttributeArea.Segment ? paramId : null,
                ProductCategoryId = request.ProductCategoryId,
                ProductId = request.ProductId,
                ChannelId = request.ChannelId,
                EventTypeId = request.EventTypeId,
                Value = attributeValue.Value!,
            };
            
            // ✅ DO NOT CALL GetParamKey here individually (causes N+1 queries)
            // ParamKey will be populated by BatchPopulateParamKeysAsync() below
            
            eventAttributeValueDtos.Add(attributeValueDto);
        }

        // Phase 2: ✅ Batch-populate all ParamKeys in ONE operation (prevents N+1 queries)
        // Instead of N individual queries for GetParamKey(), batch-load all params
        if (eventAttributeValueDtos.Count > 0)
        {
            await attributeValueService.BatchPopulateParamKeysAsync(eventAttributeValueDtos, cancellationToken);
        }

        // Phase 3: Get aggregated attribute values (DATABASE-LEVEL GROUP BY)
        AttributesValues values = await attributeValueService.GetAndSetAttributesValuesActionsAsync(
            request.TenantId, response.CustomerTenantId,
            area, paramId, paramCategoryId,
            eventAttributeValueDtos, cancellationToken);

        response.AttributeValues.AddRange(values);
    }

    private async Task<List<int>> AutoJoinCustomerToEligibleSegmentsAsync(
        EventRequest request, EventResponse response, CancellationToken cancellationToken)
    {
        // بررسی خودکار و عضویت در جامعه‌ها/بازارهای مناسب (فقط برای ارسال واقعی)
        List<int> segments = [];
        try
        {
            List<CustomerSegmentInfo> segmentsInfo = await customerSegmentService.AutoJoinCustomerToEligibleSegmentsAsync(
                request, response, cancellationToken);
            response.Segments = segmentsInfo;
            segments = segmentsInfo.Where(s => s.Type is CustomerSegmentInfoType.Member or CustomerSegmentInfoType.Join).Select(s => s.Id).ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "خطا در بررسی خودکار عضویت جامعه/بازار برای مشتری {customerTenant}", response.CustomerTenantId);
        }

        return segments;
    }

    private async Task<int?> ValidateEventRequestAsync(EventRequest request)
    {
        if (request.TenantId <= 0)
        {
            throw new ArgumentException("شناسه اکوسیستم معتبر نمی‌باشد");
        }

        if (request.ReceiveEventType == ReceiveEventType.DynamicEvent)
        {
            if (request.ChannelId == null)
            {
                throw new ArgumentException("منبع تولید رویداد معتبر نمی باشد");
            }
            if (request.EventTypeId == null)
            {
                throw new ArgumentException("رویداد معتبر نمی باشد");
            }
        }

        request.Attributes ??= new();
        request.Attributes.Customer ??= [];

        // اعتبارسنجی و دریافت ReferrerCodeId اگر referrerCode موجود باشد
        int? referrerCodeId = null;
        if (!string.IsNullOrWhiteSpace(request.ReferrerCode))
        {
            var referrerCodeResult = await referrerCodeValidationService.ValidateReferrerCodeAsync(
                request.ReferrerCode, request.TenantId);

            if (referrerCodeResult.IsSuccess && referrerCodeResult.Data != null)
            {
                referrerCodeId = referrerCodeResult.Data.Id;
            }
            else
            {
                logger.LogError("کد معرف معتبر نیست: {ReferrerCode}", request.ReferrerCode);
                // اگر استعلام نیست و کد معرف معتبر نیست، خطا بده
                throw new ArgumentException($"کد معرف معتبر نیست: {request.ReferrerCode}");
            }
            // اگر استعلام است و کد معرف معتبر نیست، فقط warning می‌دهیم و ادامه می‌دهیم
        }

        return referrerCodeId;
    }

    private async Task RegisterReferrer(
        EventRequest request, EventResponse response,
        int? referrerCodeId, CancellationToken cancellationToken)
    {
        // ثبت معرف اگر کد معرف معتبر باشد و استعلام نباشد
        if (!string.IsNullOrWhiteSpace(request.ReferrerCode) && !request.IsInquiry && referrerCodeId.HasValue)
        {
            try
            {
                var registrationResult = await referrerCodeValidationService.RegisterReferralAsync(
                    referrerCodeId.Value, response.CustomerTenantId, request.TenantId, response.EventLogId, cancellationToken);
                if (!registrationResult.IsSuccess)
                {
                    logger.LogWarning("خطا در ثبت معرف: {Error}", registrationResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در ثبت معرف برای مشتری {customerTenant}", response.CustomerTenantId);
            }
        }
    }

    private async Task<long> LogEvent(
        EventRequest request, CustomerTenant customerTenant, int? referrerCodeId, CancellationToken cancellationToken)
    {
        EventLog eventLog = new()
        {
            TenantId = request.TenantId,
            CustomerTenantId = customerTenant.Id,
            ReceiveEventType = request.ReceiveEventType,
            EventChannelId = request.ChannelId,
            EventTypeId = request.EventTypeId,
            ProductCategoryId = request.ProductCategoryId,
            ProductId = request.ProductId,

            PromotionId = request.PromotionId,//TODO
            PointLevelId = request.PointLevelId,
            RewardId = request.RewardId,
            AssetId = request.AssetId,
            ReferrerCodeId = referrerCodeId
        };
        eventLogCommand.Add(eventLog);
        _ = await eventLogCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
        return eventLog.Id;
    }
}