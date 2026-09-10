namespace Hyper.Domain.Entities.Points.Enums;

public enum PointType : int
{
    [Description("امتیاز عادی")]
    Normal,
    [Description("امتیاز تجربه")]
    Xp,
    [Description("امتیاز ارزش ‌مشتری")]
    Value
}