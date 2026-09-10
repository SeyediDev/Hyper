namespace Hyper.Domain.Enums;

public enum TicketStatus 
{
    [Description("در انتظار پاسخ")]
    Waiting = 1,

    [Description("پاسخ داده شده")]
    Replied = 2,

    [Description("بسته شده")]
    Close = 3,
}


public static class TicketStatusLocalizer
{
    private static readonly Dictionary<TicketStatus, (string English, string Persian)> _mappings = new()
    {
        { TicketStatus.Waiting, ("Waiting", "در انتظار پاسخ") },
        { TicketStatus.Replied, ("Replied", "پاسخ داده شده") },
        { TicketStatus.Close, ("Close", "بسته شده") },
    };

    public static string Localize(this TicketStatus status, string? languageCode = "fa")
    {
        if (_mappings.TryGetValue(status, out var texts))
        {
            return languageCode == "en" ? texts.English : texts.Persian;
        }
        return status.ToString();
    }
}
