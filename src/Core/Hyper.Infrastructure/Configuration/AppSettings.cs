namespace Hyper.Infrastructure.Configuration;
public class AppSettings
{
    public Ipg Ipg { get; set; } = null!;
}

public class Ipg
{
    public Zarrinpal Zarrinpal { get; set; } = null!;
}

public class Zarrinpal
{
    public string BaseUrl { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string CallbackUrl { get; set; } = null!;
}