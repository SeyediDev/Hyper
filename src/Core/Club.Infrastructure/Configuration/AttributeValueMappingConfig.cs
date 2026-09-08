using Hyper.Domain.Entities.Tenants;
using Hyper.Domain.Entities.Tenants.Data;
using Hyper.Domain.Features.Attributes;
using Mapster;

namespace Hyper.Infrastructure.Configuration;

public static class AttributeValueMappingConfig
{
    /// <summary>
    /// ثبت خودکار configuration در GlobalSettings
    /// </summary>
    static AttributeValueMappingConfig()
    {
        TypeAdapterConfig.GlobalSettings.ConfigureAttributeValueMapping();
    }

    /// <summary>
    /// پیکربندی mapping دو طرفه بین AttributeValueDto و TenantAttributeValue
    /// </summary>
    public static TypeAdapterConfig ConfigureAttributeValueMapping(this TypeAdapterConfig config)
    {
        // Mapping از TenantAttributeValue به AttributeValueDto
        config.NewConfig<TenantAttributeValue, AttributeValueDto>()
            .Map(dest => dest.Attribute, src => src.Attribute.Adapt<AttributeDto>())
            .Map(dest => dest.Key, src => src.Attribute.Key)
            .Map(dest => dest.Area, src => src.Attribute.Area)
            .Map(dest => dest.Value, src => src.Value)
            .Map(dest => dest.CustomerTenantId, src => src.CustomerTenantId)
            .Map(dest => dest.SegmentId, src => src.SegmentId)
            .Map(dest => dest.ProductCategoryId, src => src.ProductCategoryId)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.EventTypeId, src => src.EventTypeId)
            .Map(dest => dest.ChannelId, src => src.ChannelId)
            .Map(dest => dest.ParamKey, src => src.ParamKey)
            ;
        // Mapping از AttributeValueDto به TenantAttributeValue
        config.NewConfig<AttributeValueDto, TenantAttributeValue>()
            .Map(dest => dest.Attribute, src => src.Attribute.Adapt<TenantAttribute>())
            .Map(dest => dest.Area, src => src.Attribute.Area)
            .Map(dest => dest.Key, src => src.Attribute.Key)
            .Map(dest => dest.AttributeId, src => src.Attribute.Id)
            .Map(dest => dest.TenantId, src => src.Attribute.TenantId)
            .Map(dest => dest.Value, src => src.Value != null ? src.Value.ToString()! : string.Empty)
            .Map(dest => dest.CustomerTenantId, src => src.CustomerTenantId)
            .Map(dest => dest.SegmentId, src => src.SegmentId)
            .Map(dest => dest.ProductCategoryId, src => src.ProductCategoryId)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.EventTypeId, src => src.EventTypeId)
            .Map(dest => dest.ChannelId, src => src.ChannelId)
            .Map(dest => dest.ParamKey, src => src.ParamKey)
            ;
        return config;
    }
}
